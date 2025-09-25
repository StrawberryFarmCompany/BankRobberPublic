using NodeDefines;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Profiling.Memory.Experimental;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class NodePlayerController : MonoBehaviour
{
    public EntityData playerData;
    public EntityStats playerStats;

    // [변경됨] 캐릭터 고유 번호 대신, 매니저가 관리하는 ID 사용
    public int playerID { get; private set; }

    private Vector3Int playerVec;
    private bool isHighlightOn = false;

    // [변경됨] GameManager 대신 NodePlayerManager에서 턴 관리
    private bool characterTurn = false;

    [SerializeField] NavMeshAgent agent;
    [SerializeField] Camera mainCamera;

    [SerializeField] private MoveRangeHighlighter highlighter;

    public bool isHide;
    public bool isAiming;

    public bool isEndReady;

    [Header("현재 플레이어의 액션 상태")]
    public bool isMoveMode;
    public bool isRunMode;
    public bool isHideMode;
    public bool isSneakAttackMode;
    public bool isThrowMode;
    public bool isPickPocketMode;
    public bool isAimingMode;
    public bool isRangeAttackMode;
    public bool isPerkActionMode;

    [Header("명중 보정치")]
    public int hitBonus = 0;

    [Header("행동판정")]
    public int diceResult;

    private bool isEndTurn;
    public bool IsEndTurn { get { return isEndTurn; } }


    [Header("창문 넘기")]
    [SerializeField] string wallLayerName = "Wall"; // 충돌 무시할 레이어 이름
    [SerializeField] float vaultSpeed = 8f;         //이동 속도
    [SerializeField] float arriveTolerance = 0.05f; //위치 수정 용

    [SerializeField] bool onlyOverWindow = true;
    [SerializeField] string windowSpecialName = "Window";

    bool _isVaulting;                                // 중복 실행 방지
    int _wallLayer = -1, _selfLayer = -1;

    private Queue<Vector3Int> pathQueue = new Queue<Vector3Int>();
    Vector3Int curTargetPos;
    public bool isMoving;
    public bool canNextMove;

    void EnsureLayers()
    {
        if (_wallLayer < 0) _wallLayer = LayerMask.NameToLayer(wallLayerName);
        if (_selfLayer < 0) _selfLayer = gameObject.layer;
    }

    //대각선 금지
    Vector3Int AxialDir(Vector3Int from, Vector3Int to)
    {
        int dx = Mathf.Clamp(to.x - from.x, -1, 1);
        int dz = Mathf.Clamp(to.z - from.z, -1, 1);
        if (Mathf.Abs(dx) > Mathf.Abs(dz)) dz = 0;
        else if (Mathf.Abs(dz) > Mathf.Abs(dx)) dx = 0;
        else
        {
            dz = 0;
        }
        return new Vector3Int(dx, 0, dz);
    }

    public void VaultTowardMouse()
    {
        if (!IsMyTurn() || _isVaulting) return;

        Vector3 mousePos = Mouse.current.position.ReadValue();  //마우스 위치
        Vector3Int cur = GameManager.GetInstance.GetNode(transform.position).GetCenter; //프레이어 위치
        Vector3Int click = GetNodeVector3ByRay(mousePos);
        if (click.x == -1) return;

        Vector3Int dir = AxialDir(cur, click);  //직선 이동만
        if (dir == Vector3Int.zero) return;

        Vector3Int over = new Vector3Int(cur.x + dir.x, cur.y, cur.z + dir.z);  //창문,벽 있는 칸
        Vector3Int land = new Vector3Int(cur.x + dir.x, cur.y, cur.z + dir.z) + dir;    //착지할 칸
        TryVault(over, land);
    }

    public void TryVault(Vector3Int overCell, Vector3Int landCell)
    {
        if (!IsMyTurn() || _isVaulting) return;

        var gm = GameManager.GetInstance;
        var overNode = gm.GetNode(overCell);
        var landNode = gm.GetNode(landCell);
        if (overNode == null || landNode == null) { Debug.Log("유효하지 않은 위치"); return; }

        if (onlyOverWindow && SpecialNodeManager.GetInstance != null)
        {
            var queryPos = new Vector3Int(overCell.x, overCell.y - 1, overCell.z);
            if (!SpecialNodeManager.GetInstance.TryGetSpecialNodeType(queryPos, out var type) || type.ToString() != windowSpecialName)
            {
                Debug.Log("창문 앞에서만 넘을 수 있습니다.");
                return;
            }
        }

        if (overNode.isWalkable) { Debug.Log("앞칸이 창문,벽이 아님"); return; }
        if (!landNode.isWalkable) { Debug.Log("착지칸이 보행 불가"); return; }

        //창문 너머 칸에 사람이 있을 시 방지 나중에 게임 매니저에서 판정 추가 필요
        //if (gm.IsNodeOccupied(landCell)) { Debug.Log("착지칸 사람 존재"); return; }

        TurnOffHighlighter();
        StartCoroutine(Co_VaultMove((Vector3)landNode.GetCenter, landCell));
    }

    IEnumerator Co_VaultMove(Vector3 targetWorld, Vector3Int landCell)
    {
        _isVaulting = true;
        EnsureLayers();

        //NavMeshAgent 간섭 방지
        if (agent) agent.enabled = false;

        //벽 레이어 충돌 무시
        if (_wallLayer >= 0) Physics.IgnoreLayerCollision(_selfLayer, _wallLayer, true);

        Vector3 start = transform.position;
        float flatDist = Vector3.Distance(new Vector3(start.x, 0, start.z), new Vector3(targetWorld.x, 0, targetWorld.z));
        float duration = Mathf.Max(0.06f, flatDist / Mathf.Max(0.01f, vaultSpeed));

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            transform.position = Vector3.Lerp(start, targetWorld, t);
            yield return null;
        }

        //착지 (위치 보정)
        if (Vector3.Distance(transform.position, targetWorld) > arriveTolerance)
            transform.position = targetWorld;

        //벽 충돌 무시 OFF
        if (_wallLayer >= 0) Physics.IgnoreLayerCollision(_selfLayer, _wallLayer, false);

        // NavMeshAgent 재활성 및 경로 초기화/동기화
        if (agent)
        {
            agent.enabled = true;
            agent.ResetPath();
        }

        // 내부 상태/하이라이트 갱신
        playerVec = landCell;
        TurnOnHighlighter(playerVec, playerStats.moveRange);

        _isVaulting = false;
    }

    private void Awake()
    {
        playerStats = new EntityStats(playerData);
        if (agent == null) agent = GetComponent<NavMeshAgent>();
        isHide = true;
        isEndTurn = false;
        StartMode(ref isMoveMode);
    }

    void Start()
    {
        playerVec = GameManager.GetInstance.GetNode(transform.position).GetCenter;

        // [변경됨] 매니저에 자기 자신 등록
        NodePlayerManager.GetInstance.RegisterPlayer(this);
        GameManager.GetInstance.BattleTurn.AddUnit(false, ResetPlayer, EndAction); //+++++++++++++++++==================================================================================================
    }

    void Update()
    {
        if (IsMyTurn())
        {
        TurnOnHighlighter(playerVec, playerStats.movement);
        }
        else
            {
            TurnOffHighlighter();
        }

        if (isMoving)
        {
            SequentialMove();
        }

    }

    // [변경됨] 매니저가 ID를 할당할 수 있도록 Setter 제공
    public void SetPlayerID(int id)
    {
        playerID = id;
    }

    public void OnCancel(InputAction.CallbackContext context)
    {
        if (context.started && IsMyTurn())
        {
            Debug.Log("취소 버튼 눌림");
            StartMode(ref isMoveMode);
            UIManager.GetInstance.ShowActionPanel(true);
            TurnOnHighlighter(playerStats.movement);
        }
    }

    public void OnClickNode(InputAction.CallbackContext context)
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            // UI 클릭 중이면 실행 안 함
            return;
        }

        if (context.started && IsMyTurn() && isMoveMode)
        {
            Vector3 mousePos = Mouse.current.position.ReadValue();
            Move(mousePos);
        }

        if (context.started && IsMyTurn() && isRunMode)
        {
            Debug.Log("달리기");
            UIManager.GetInstance.ShowActionPanel(true);
            playerStats.ActiveRun();
            isHighlightOn = false;
        }

        if (context.started && IsMyTurn() && isHideMode)
        {
            Debug.Log("숨기");
            HideMode();
            UIManager.GetInstance.ShowActionPanel(true);
        }

        if(context.started && IsMyTurn() && isThrowMode)
        {
            Debug.Log("던지기");
            Vector3 mousePos = Mouse.current.position.ReadValue();
            CheckThrow(mousePos);
            UIManager.GetInstance.ShowActionPanel(true);
        }

        if (context.started && IsMyTurn() && isSneakAttackMode)
        {
            Debug.Log("기습 공격");
            Vector3 mousePos = Mouse.current.position.ReadValue();
            CheckSneakAttack(mousePos);
        }

        if (context.started && IsMyTurn() && isPickPocketMode)
        {
            Debug.Log("훔치기");
            Vector3 mousePos = Mouse.current.position.ReadValue();
            PickPocket(mousePos);
        }

        if (context.started && IsMyTurn() && isAimingMode)
        {
            if (!playerStats.ConsumeActionPoint(1))
            {
                Debug.Log("행동력이 부족함");
                return;
            }
            Debug.Log("조준");
            if (isAiming)
            {
                UIManager.GetInstance.ShowActionPanel(true);
                RemoveAiming();
            }
            else
            {
                UIManager.GetInstance.ShowActionPanel(true);
                Aiming();
            }
        }

        if (context.started && IsMyTurn() && isRangeAttackMode)
        {
            Debug.Log("원거리 공격");
            Vector3 mousePos = Mouse.current.position.ReadValue();
            RangeAttack(mousePos);
        }

        if(context.canceled && IsMyTurn() && (isRunMode || isAimingMode || isHideMode))
        {
            UIManager.GetInstance.ShowActionPanel(true);
            StartMode(ref isMoveMode);
        }
    }

    /// <summary>
    /// Move 함수: 목표 좌표까지 체비셰프 방식으로 경로를 생성하고 이동 시작
    /// </summary>
    /// <param name="targetPos">목표 좌표</param>
    /// <returns>이동을 시작했으면 true, 이동력이 부족하면 false</returns>
    public void Move(Vector3 mouseScreenPos)
    {
        Ray ray = mainCamera.ScreenPointToRay(mouseScreenPos);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            // 현재 좌표 (정수 격자 기준)
            Vector3Int start = GameManager.GetInstance.GetNode(transform.position).GetCenter;
            Vector3Int targetPos = GameManager.GetInstance.GetNode(hit.point).GetCenter;

            // 경로 배열 생성
            List<Vector3Int> path = GenerateChebyshevPath(start, targetPos);

            pathQueue.Clear();

            // 이동력만큼만 큐에 넣기
            foreach (var step in path)
            {
                if (playerStats.ConsumeMovement(1))
                {
                    pathQueue.Enqueue((Vector3Int)step);
                }
                else
                {
                    Debug.Log($"이동 도중 이동력 부족. {step} 여기서 멈춤");
                    break;
                }
            }

            if (pathQueue.Count > 0)
            {
                playerVec = pathQueue.Last();
                TurnOffHighlighter();
                //최종 이동 구현
                isMoving = true;
                canNextMove = true;
            }
        }
    }

    private List<Vector3Int> GenerateChebyshevPath(Vector3Int start, Vector3Int end)
    {
        // BFS 탐색을 위한 큐
        Queue<Vector3Int> open = new Queue<Vector3Int>();
        Dictionary<Vector3Int, Vector3Int> cameFrom = new Dictionary<Vector3Int, Vector3Int>();

        open.Enqueue(start);
        cameFrom[start] = start;

        while (open.Count > 0)
        {
            Vector3Int current = open.Dequeue();

            // 목표에 도달하면 역추적해서 경로 반환
            if (current == end)
            {
                return ReconstructPath(cameFrom, start, end);
            }

            // 인접 노드 탐색 (대각선 포함 체비셰프)
            foreach (var dir in GameManager.GetInstance.nearNode)
            {
                Vector3Int next = current + dir;

                // 1) 노드 존재 여부 확인
                if (!GameManager.GetInstance.Nodes.ContainsKey(next)) continue;

                var node = GameManager.GetInstance.Nodes[next];

                // 2) 이동 가능한지 체크
                if (node == null) continue;
                if (!node.isWalkable) continue;
                if(node.standing != null)
                    if (node.standing.Count > 0) continue;

                // 3) 방문한 적 없는 경우만 추가
                if (!cameFrom.ContainsKey(next))
                {
                    cameFrom[next] = current;
                    open.Enqueue(next);
                }
            }
        }

        // 경로를 찾지 못한 경우
        Debug.Log("경로를 찾지 못했습니다.");
        return new List<Vector3Int>();
    }

    /// <summary>
    /// BFS 탐색 후 start→end까지 역추적
    /// </summary>
    private List<Vector3Int> ReconstructPath(Dictionary<Vector3Int, Vector3Int> cameFrom, Vector3Int start, Vector3Int end)
    {
        List<Vector3Int> path = new List<Vector3Int>();
        Vector3Int current = end;

        while (current != start)
        {
            path.Add(current);
            current = cameFrom[current];
        }

        path.Reverse();
        return path;
    }

    public void SequentialMove()
    {
        // 아직 목표가 없으면 다음 큐 꺼내기
        if (!isMoving) return;

        // 도착 판정 (== 대신 거리로 체크)
        if (Vector3.Distance(transform.position, curTargetPos) < 0.1f)
        {
            canNextMove = true;
        }

        if (canNextMove && pathQueue.Count > 0)
        {
            canNextMove = false;
            curTargetPos = pathQueue.Dequeue();
            agent.SetDestination(curTargetPos);
        }

        // 모든 경로 소모 시 이동 종료
        if (pathQueue.Count == 0 && Vector3.Distance(transform.position, curTargetPos) < 0.1f)
        {
            isMoving = false;
        }
    }

    public void OnRun(InputAction.CallbackContext context)
    {
        if (context.started && IsMyTurn() && isMoveMode)
        {
            UIManager.GetInstance.ShowActionPanel(false);
            Debug.Log("달리기 모드 활성화");
            StartMode(ref isRunMode);
        }
    }

    public void OnThrow(InputAction.CallbackContext context)
    {


        if (context.canceled && IsMyTurn() && isMoveMode)
        {
            UIManager.GetInstance.ShowActionPanel(false);
            StartMode(ref isThrowMode);
            TurnOnHighlighter(6);
        }
    }

    private void CheckThrow(Vector3 mouseScreenPos)
    {
        Vector3Int targetNodeCenter = GetNodeVector3ByRay(mouseScreenPos);

        if (targetNodeCenter == new Vector3Int(-999, -999, -999))
        {
            Debug.Log("유효하지 않은 좌표입니다!");
            return;
        }

        if (!CheckRange(targetNodeCenter, 6)) //========================================================================임의로 6범위
        {
            Debug.Log("해당 위치에 적이 없거나 범위를 벗어났습니다!");
            return;
        }

        UIManager.GetInstance.ShowActionPanel(true);
        if (playerStats.ConsumeActionPoint(1))
        {
            
           ThrowSystem.GetInstance.ExecuteCoinThrow(this, targetNodeCenter);
            StartMode(ref isMoveMode);
            TurnOffHighlighter();
        }
        else
        {
            Debug.Log("행동력이 부족합니다!");
        }
    }


    public void OnHideAndSneakAttack(InputAction.CallbackContext context)
    {
        if (context.started && IsMyTurn() && !isHide && isMoveMode)
        {
            UIManager.GetInstance.ShowActionPanel(false);
            Debug.Log("숨기 모드 활성화");
            StartMode(ref isHideMode);
        }

        if (context.started && IsMyTurn() && isHide && isMoveMode)
        {
            UIManager.GetInstance.ShowActionPanel(false);
            Debug.Log("기습 공격 모드 활성화");
            StartMode(ref isSneakAttackMode);
        }
    }

    private void HideMode()
    {
        UIManager.GetInstance.pip.HideAndSneakText();
        isHide = true;
    }

    private void RemoveHideMode()
    {
        UIManager.GetInstance.pip.HideAndSneakText();
        isHide = false;
    }

    private void CheckSneakAttack(Vector3 mouseScreenPos)
    {
        Vector3Int targetNodeCenter = GetNodeVector3ByRay(mouseScreenPos);

        if (targetNodeCenter == new Vector3Int(-999, -999, -999))
        {
            Debug.Log("유효하지 않은 좌표입니다!");
            return;
        }

        if (!CheckRangeAndEntity(targetNodeCenter, 2))
        {
            Debug.Log("해당 위치에 적이 없거나 범위를 벗어났습니다!");
            return;
        }

        Vector3Int bestNode = FindClosestWalkableAdjacentNode(targetNodeCenter);

        if (bestNode == new Vector3Int(-999, -999, -999))
        {
            Debug.Log("이동할 수 있는 인접 노드를 찾지 못했습니다.");
            return;
        }

        int cost = CalculateMoveCost(bestNode);

        if (!playerStats.ConsumeMovement(cost))
        {
            Debug.Log("인접 노드로 이동할 수 있는 이동력 부족!");
            return;
        }
        UIManager.GetInstance.ShowActionPanel(true);
        if (playerStats.ConsumeActionPoint(1))
        {
            RemoveHideMode();

            DiceManager.GetInstance.DelayedRoll(0, RollDice);
            if (diceResult + hitBonus /*- 대상의 회피율*/ > 0)
                SneakAttack(bestNode);

            Debug.Log("기습 공격 성공!");
        }
        else
        {
            Debug.Log("행동력이 부족합니다!");
        }
    }

    private void SneakAttack(Vector3Int pos)
    {
        agent.SetDestination(pos);
        playerVec = pos;
        TurnOffHighlighter();


    }

    public void OnPickPocket(InputAction.CallbackContext context)
    {
        if (context.started && IsMyTurn() && isMoveMode && isHide)
        {
            UIManager.GetInstance.ShowActionPanel(false);
            Debug.Log("훔치기 모드 활성화");
            StartMode(ref isPickPocketMode);
        }
    }

    private void PickPocket(Vector3 mouseScreenPos)
    {
        Vector3Int targetNodeCenter = GetNodeVector3ByRay(mouseScreenPos);

        if (targetNodeCenter == new Vector3Int(-999, -999, -999))
        {
            Debug.Log("유효하지 않은 좌표입니다!");
            return;
        }

        if (!CheckRangeAndInteractable(targetNodeCenter, 1))
        {
            Debug.Log("해당 위치에 훔칠 대상이 없거나 범위를 벗어났습니다!");
            return;
        }

        if (!GameManager.GetInstance.IsNoneBattlePhase())
        {
            Debug.Log("배틀 페이즈에 행동할 수 없습니다!");
            return;
        }

        if (playerStats.ConsumeActionPoint(1))
        {
            UIManager.GetInstance.ShowActionPanel(true);
            Debug.Log("훔치기 성공!");
        }
        else
        {
            Debug.Log("행동력이 부족합니다!");
        }
    }

    public void OnAiming(InputAction.CallbackContext context)
    {
        if (context.started && IsMyTurn() && isMoveMode)
        {
            UIManager.GetInstance.ShowActionPanel(false);
            Debug.Log("조준 모드 활성화");
            StartMode(ref isAimingMode);
        }
    }

    private void Aiming()
    {
        isAiming = true;
        hitBonus += 3;
    }

    private void RemoveAiming()
    {
        isAiming = false;
        hitBonus -= 3;
    }

    public void OnRangeAttack(InputAction.CallbackContext context)
    {
        if (context.started && IsMyTurn() && isMoveMode)
        {
            UIManager.GetInstance.ShowActionPanel(false);
            Debug.Log("원거리 공격 모드 활성화");
            StartMode(ref isRangeAttackMode);
        }
    }

    private void RangeAttack(Vector3 mouseScreenPos)
    {
        if (CheckRangeAndEntity(GetNodeVector3ByRay(mouseScreenPos), (int)playerStats.attackRange))
        {
            if (playerStats.ConsumeActionPoint(1))
            {
                UIManager.GetInstance.ShowActionPanel(true);
                if (isAiming)
                {
                    RemoveAiming();
                }
            }
        }
    }

    public void OnPerkAction(InputAction.CallbackContext context)
    {
        if (context.started && IsMyTurn())
        {
            UIManager.GetInstance.ShowActionPanel(false);
            Debug.Log("특전 모드 활성화");
            // 특전 로직
        }
    }


    /// <summary>
    /// 플레이어의 턴, 해당 캐릭터의 턴인지를 판별하여 해당 캐릭터의 행동 조건을 판별
    /// </summary>
    /// <returns></returns>
    public bool IsMyTurn()
    {
        if(GameManager.GetInstance.CurrentPhase == GamePhase.NoneBattle)
        {
            return (NodePlayerManager.GetInstance.GetCurrentPlayer() == this) && (GameManager.GetInstance.NoneBattleTurn.GetCurrState() == TurnTypes.ally);
        }
        else if(GameManager.GetInstance.CurrentPhase == GamePhase.Battle)
        {
            return (NodePlayerManager.GetInstance.GetCurrentPlayer() == this); //캐릭터의 턴이 시작되었다는 것을 혹은 해당 기물의 턴이 시작되었다는 조건이 하나 더 붙어야함
        }
        else
        {
            Debug.Log("유효하지 않은 게임 페이즈입니다!");
            return false;
        }

    }

    public void ResetPlayer() 
    {
        List<NodePlayerController> temp = NodePlayerManager.GetInstance.GetAllPlayers();
        int i = 0;

        for (; i < temp.Count; i++)
        {
            if (temp[i] == this) break;
        }

        playerStats.ResetForNewTurn();
        NodePlayerManager.GetInstance.SwitchToPlayer(i);
    }

    private void TurnOnHighlighter(Vector3Int destination, int range)
    {
        if (destination == GameManager.GetInstance.GetNode(transform.position).GetCenter && !isHighlightOn)
        {
            isHighlightOn = true;
            highlighter.ShowMoveRange(GameManager.GetInstance.GetNode(transform.position).GetCenter, range);
        }
    }

    private void TurnOnHighlighter(int range)
    {
            isHighlightOn = true;
            highlighter.ShowMoveRange(GameManager.GetInstance.GetNode(transform.position).GetCenter, range);
        
    }

    private void TurnOffHighlighter()
    {
        isHighlightOn = false;
        highlighter.ClearHighlights();
    }

    public void StartMode(ref bool mode)
    {
        isMoveMode = false;
        isSneakAttackMode = false;
        isAimingMode = false;
        isRunMode = false;
        isPickPocketMode = false;
        isRangeAttackMode = false;
        isPerkActionMode = false;
        isThrowMode = false;

        mode = true;
    }

    public bool CheckRange(Vector3Int Pos, int range)
    {
        Vector3Int start = GameManager.GetInstance.GetNode(transform.position).GetCenter;
        for (int x = -range; x <= range; x++)
        {
            for (int z = -range; z <= range; z++)
            {
                Vector3Int current = start + new Vector3Int(x, 0, z); //y값이 안 맞을 수도 있으니까 나중에 버그나면 이놈 탓

                Node node = GameManager.GetInstance.GetNode(current);
                if (node == null || !node.isWalkable)
                    continue;

                if(current == Pos) return true;
            }
        }
        return false;
    }

    public bool CheckRangeAndEntity(Vector3 Pos, int range) //반환값을 bool로 하는게 맞나? 애매
    {
        Vector3Int start = GameManager.GetInstance.GetNode(transform.position).GetCenter;
        for (int x = -range; x <= range; x++)
        {
            for (int z = -range; z <= range; z++)
            {
                Vector3Int current = start + new Vector3Int(x, 0, z); //y값이 안 맞을 수도 있으니까 나중에 버그나면 이놈 탓

                Node node = GameManager.GetInstance.GetNode(current); //요쯤? 엔티티 검출되는지 확인하는 로직
                if (node == null || !node.isWalkable)
                    continue;

                if (current == Pos) return true;
            }
        }
        return false;
    }

    public bool CheckRangeAndInteractable(Vector3 Pos, int range) //반환값을 bool로 하는게 맞나? 애매
    {
        Vector3Int start = GameManager.GetInstance.GetNode(transform.position).GetCenter;
        for (int x = -range; x <= range; x++)
        {
            for (int z = -range; z <= range; z++)
            {
                Vector3Int current = start + new Vector3Int(x, 0, z); //y값이 안 맞을 수도 있으니까 나중에 버그나면 이놈 탓

                Node node = GameManager.GetInstance.GetNode(current); //요쯤? 인터랙터블 검출되는지 확인하는 로직
                if (node == null || !node.isWalkable)
                    continue;

                if (current == Pos) return true;
            }
        }
        return false;
    }

    public Vector3Int GetNodeVector3ByRay(Vector3 mouseScreenPos)
    {
        Ray ray = mainCamera.ScreenPointToRay(mouseScreenPos);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            return GameManager.GetInstance.GetNode(hit.point).GetCenter;
        }
        Debug.Log("유효하지 않은 좌표입니다!");
        return new Vector3Int(-999, -999, -999); //유효하지 않은 좌표 반환
    }

    /// <summary>
    /// targetNode 기준으로 상하좌우 대각선 8방향 중 가장 가까운 이동 가능한 노드 반환
    /// </summary>
    private Vector3Int FindClosestWalkableAdjacentNode(Vector3Int targetNode)
    {
        Vector3Int[] directions = new Vector3Int[]
        {
        new Vector3Int(1, -1, 0),
        new Vector3Int(-1, -1, 0),
        new Vector3Int(0, -1, 1),
        new Vector3Int(0, -1, -1),
        new Vector3Int(1, -1, 1),
        new Vector3Int(-1, -1, 1),
        new Vector3Int(1, -1, -1),
        new Vector3Int(-1, -1, -1)
        };

        Vector3Int bestNode = new Vector3Int(-999, -999, -999);
        float bestDist = float.MaxValue;

        foreach (var dir in directions)
        {
            Vector3Int checkNode = targetNode + dir;
            Node node = GameManager.GetInstance.GetNode(checkNode);
            if (node == null || !node.isWalkable)
                continue;

            float dist = Vector3.Distance(transform.position, checkNode);
            if (dist < bestDist)
            {
                bestDist = dist;
                bestNode = checkNode;
            }
        }

        return bestNode;
    }

    /// <summary>
    /// 현재 위치에서 targetNode까지 이동하는데 필요한 이동력 코스트를 계산한다.
    /// 팔방향 이동 기준으로, 1칸당 1코스트.
    /// </summary>
    public int CalculateMoveCost(Vector3Int targetNode)
    {
        Vector3Int current = GameManager.GetInstance.GetNode(transform.position).GetCenter;

        int dx = Mathf.Abs(targetNode.x - current.x);
        int dz = Mathf.Abs(targetNode.z - current.z);

        // 팔방향 이동이 가능하므로 체비쇼프 거리 사용
        int cost = Mathf.Max(dx, dz);

        return cost;
    }

    public bool RangeAttackActionCheck(Vector3Int targetPos /*, 타겟 엔티티*/)
    {
        int hitAdjustment;
        if (CheckRange(targetPos, 5))
        {
            hitAdjustment = 0;
        }
        else if (CheckRange(targetPos, 9))
        {
            hitAdjustment = -2;
        }
        else if (CheckRange(targetPos, 20))
        {
            hitAdjustment = -5;
        }
        else
        {
            hitAdjustment = -13;
        }

        hitAdjustment += hitBonus;

        return true;

        //return (/*3d6 다이스*/ (playerCondition.playerStats.attackRange + hitAdjustment - /*타겟 엔티티의 회피율*/)>)
    }

    public void EndAction()
    {
        NodePlayerManager.GetInstance.NotifyPlayerEndTurn(this);
    }

    private void RollDice(int result)
    {
        diceResult = result;
    }
}
