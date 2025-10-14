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

    private Vector3Int playerVec;
    public bool isHighlightOn = false;

    // [변경됨] GameManager 대신 NodePlayerManager에서 턴 관리
    public PlayerInput playerInput;

    [SerializeField] NavMeshAgent agent;
    [SerializeField] Camera mainCamera;

    [SerializeField] private MoveRangeHighlighter highlighter;
    public Gun gun;

    [HideInInspector]
    public bool isHide;
    [HideInInspector]
    public bool isAiming;
    [HideInInspector]
    public bool isEndReady;

    //[Header("현재 플레이어의 액션 상태")]
    [HideInInspector]
    public bool isMoveMode;
    [HideInInspector]
    public bool isRunMode;
    [HideInInspector]
    public bool isHideMode;
    [HideInInspector]
    public bool isSneakAttackMode;
    [HideInInspector]
    public bool isThrowMode;
    [HideInInspector]
    public bool isPickPocketMode;
    [HideInInspector]
    public bool isAimingMode;
    [HideInInspector]
    public bool isRangeAttackMode;
    [HideInInspector]
    public bool isPerkActionMode;

    [Header("명중 보정치")]
    public int hitBonus = 0;

    private bool isEndTurn;
    public bool IsEndTurn { get { return isEndTurn; } }

    private Queue<Vector3Int> pathQueue = new Queue<Vector3Int>();
    Vector3Int curTargetPos;
    public bool isMoving;
    public bool canNextMove;

    [Header("백팩")]
    public GameObject fullBackPackPrefab;
    public GameObject backPackParent;
    public GameObject emptyBackPack;
    public GameObject fullBackPack;

    private void Awake()
    {
        playerStats = new EntityStats(playerData);
        if (agent == null) agent = GetComponent<NavMeshAgent>();
        if (gun == null) gun = GetComponent<Gun>();
        if (playerInput == null) playerInput = GetComponent<PlayerInput>();
        playerStats.ForceMove += WindowForcMove;
        isHide = true;
        isEndTurn = false;
        StartMode(ref isMoveMode);
        //playerStats.OnDamaged += 추후 애니메이션 구현시 데미지 스테이트로 변환되도록
    }

    void Start()
    {
        playerInput.DeactivateInput();
        playerVec = GameManager.GetInstance.GetNode(transform.position).GetCenter;

        // [변경됨] 매니저에 자기 자신 등록
        NodePlayerManager.GetInstance.RegisterPlayer(this);
        GameManager.GetInstance.BattleTurn.AddUnit(false, ResetPlayer, EndAction); //+++++++++++++++++==================================================================================================

        playerStats.SetCurrentNode(transform.position);
        playerStats.NodeUpdates(transform.position);
        GameManager.GetInstance.RegisterEntity(playerStats);
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

    public void OnCancel(InputAction.CallbackContext context)
    {
        if (context.canceled)
        {

            Vector3 mousePos = Mouse.current.position.ReadValue();
            
            if (!ViewBuffData(mousePos)) return;
        }
        if (context.started && IsMyTurn())
        {
            StartMode(ref isMoveMode);
            UIManager.GetInstance.ShowActionPanel(true);
            TurnOnHighlighter(playerStats.movement);
        }
    }
    public bool ViewBuffData(Vector3 mouseScreenPos)
    {
        Ray ray = mainCamera.ScreenPointToRay(mouseScreenPos);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Node node = GameManager.GetInstance.GetNode(hit.point);
            if (node != null && node.standing.Count > 0)
            {
                UIManager.GetInstance.BuffPannel.UpdateBuffList(node);
                UIManager.GetInstance.BuffPannel.Description.TurnOn(false);
                return true;
            }
        }
        if(!EventSystem.current.IsPointerOverGameObject()) UIManager.GetInstance.BuffPannel.TurnOn(false);
        return false;
    }
    public void OnClickNode(InputAction.CallbackContext context)
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            // UI 클릭 중이면 실행 안 함
            return;
        }

        if (context.started && IsMyTurn() && isMoveMode && !isMoving)
        {
            Vector3 mousePos = Mouse.current.position.ReadValue();
            Move(mousePos);
        }

        if (context.started && IsMyTurn() && isRunMode)
        {
            UIManager.GetInstance.ShowActionPanel(true);
            playerStats.ActiveRun();
            isHighlightOn = false;
        }

        if (context.started && IsMyTurn() && isHideMode)
        {
            HideMode();
            UIManager.GetInstance.ShowActionPanel(true);
        }

        if(context.started && IsMyTurn() && isThrowMode)
        {
            Vector3 mousePos = Mouse.current.position.ReadValue();
            CheckThrow(mousePos);
            UIManager.GetInstance.ShowActionPanel(true);
        }

        if (context.started && IsMyTurn() && isSneakAttackMode)
        {
            Vector3 mousePos = Mouse.current.position.ReadValue();
            CheckSneakAttack(mousePos);
        }

        if (context.started && IsMyTurn() && isPickPocketMode)
        {
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
            Vector3 mousePos = Mouse.current.position.ReadValue();
            CheckRangeAttack(mousePos);
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
            if (GameManager.GetInstance.GetNode(hit.point) == null)
            {
                Debug.Log("노드가 아니다.");
                return;
            }

            if (!GameManager.GetInstance.GetNode(hit.point).isWalkable || GameManager.GetInstance.GetEntityAt(GameManager.GetInstance.GetNode(hit.point).GetCenter) != null)
            {
                Debug.Log("갈 수 없는 곳이거나, 엔티티가 있다.");
                return;
            }


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
                if (GameManager.GetInstance.GetEntityAt(next) != null) continue;

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
            playerStats.NodeUpdates(curTargetPos);
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
            StartMode(ref isRunMode);
        }
    }

    public void OnThrow(InputAction.CallbackContext context)
    {
        if (context.started && IsMyTurn() && isMoveMode)
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
            return;
        }

        if (!CheckRange(targetNodeCenter, 6)) //========================================================================임의로 6범위
        {
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
            StartMode(ref isHideMode);
        }

        if (context.started && IsMyTurn() && isHide && isMoveMode)
        {
            UIManager.GetInstance.ShowActionPanel(false);
            StartMode(ref isSneakAttackMode);
        }
    }

    private void HideMode()
    {
        isHide = true;
        UIManager.GetInstance.pip.HideAndSneakText();
    }

    private void RemoveHideMode()
    {
        isHide = false;
        UIManager.GetInstance.pip.HideAndSneakText();
    }

    private void CheckSneakAttack(Vector3 mouseScreenPos)
    {
        Vector3Int targetNodeCenter = GetNodeVector3ByRay(mouseScreenPos);

        if (targetNodeCenter == new Vector3Int(-999, -999, -999))
        {
            return;
        }

        if (!CheckRangeAndEntity(targetNodeCenter, 2))
        {
            return;
        }

        Vector3Int bestNode = FindClosestWalkableAdjacentNode(targetNodeCenter);

        if (bestNode == new Vector3Int(-999, -999, -999))
        {
            return;
        }

        int cost = CalculateMoveCost(bestNode);

        if (!playerStats.ConsumeMovement(cost))
        {
            return;
        }
        UIManager.GetInstance.ShowActionPanel(true);
        if (playerStats.ConsumeActionPoint(1))
        {
            RemoveHideMode();
            int result = DiceManager.GetInstance.DirrectRoll(0, 6, 3);
            if (result + hitBonus - GameManager.GetInstance.GetEntityAt(targetNodeCenter).evasionRate > 0)
            SneakAttack(bestNode, targetNodeCenter);
            StartMode(ref isMoveMode);
        }
        else
        {
            Debug.Log("행동력이 부족합니다!");
        }
    }

    private void SneakAttack(Vector3Int movePos, Vector3Int targetPos)
    {
        agent.SetDestination(movePos);
        playerStats.NodeUpdates(movePos);
        playerVec = movePos;
        TurnOffHighlighter();
        int result = DiceManager.GetInstance.DirrectRoll(0, 6, 2);
        Debug.Log($"{result}의 데미지를 상대에게 줌");
        GameManager.GetInstance.GetEntityAt(targetPos).Damaged(result);


    }

    public void OnPickPocket(InputAction.CallbackContext context)
    {
        if (context.started && IsMyTurn() && isMoveMode && isHide)
        {
            UIManager.GetInstance.ShowActionPanel(false);
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
            StartMode(ref isRangeAttackMode);
            TurnOnHighlighter(0);
        }
    }

    private void CheckRangeAttack(Vector3 mouseScreenPos)
    {
        Vector3Int targetPos = GetNodeVector3ByRay(mouseScreenPos);

        Vector3 start = transform.position;
        Vector3 target = targetPos;

        if (NavMesh.Raycast(start, target, out NavMeshHit hit, NavMesh.AllAreas))
        {
            Debug.Log("무언가로 막혀있음");
            return;
        }

        if (!CheckRangeAndEntity(targetPos, (int)playerStats.attackRange))
        {
            Debug.Log("엔티티가 없엉");
            return;
            
        }

        if (!playerStats.ConsumeActionPoint(1))
        {
            Debug.Log("행동 포인트가 부족");
            return;
        }

        gun.Shoot(targetPos, hitBonus);
        UIManager.GetInstance.ShowActionPanel(true);
        if (isAiming)
        {
            RemoveAiming();
        }
        TurnOffHighlighter();
        StartMode(ref isMoveMode);
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

    public void TurnOnHighlighter(Vector3Int destination, int range)
    {
        if (destination == GameManager.GetInstance.GetNode(transform.position).GetCenter && !isHighlightOn)
        {
            isHighlightOn = true;
            highlighter.ShowMoveRange(GameManager.GetInstance.GetNode(transform.position).GetCenter, range);
        }
    }

    public void TurnOnHighlighter(int range)
    {
            isHighlightOn = true;
            highlighter.ShowMoveRange(GameManager.GetInstance.GetNode(transform.position).GetCenter, range);
        
    }

    public void TurnOffHighlighter()
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
                if (node == null || !node.isWalkable || GameManager.GetInstance.GetEntityAt(current) == null)
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
        if(CheckRangeAndEntity(targetNode, 1))
        {
            return GameManager.GetInstance.GetVecInt(transform.position);
        }

        Vector3Int[] directions = new Vector3Int[]
        {
        new Vector3Int(1, 0, -1),
        new Vector3Int(-1, 0, 0),
        new Vector3Int(0, 0, 1),
        new Vector3Int(0, 0, -1),
        new Vector3Int(1, 0, 1),
        new Vector3Int(-1, 0, 1),
        new Vector3Int(1, 0, 0),
        new Vector3Int(-1, 0,-1),
        new Vector3Int(0, 0,0)
        };

        Vector3Int bestNode = new Vector3Int(-999, -999, -999);
        float bestDist = float.MaxValue;

        foreach (var dir in directions)
        {
            Vector3Int checkNode = targetNode + dir;
            Node node = GameManager.GetInstance.GetNode(checkNode);
            if (node == null || !node.isWalkable)
                continue;
            if (GameManager.GetInstance.GetEntityAt(checkNode) != null) continue;

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

    public void EndAction()
    {
        NodePlayerManager.GetInstance.NotifyPlayerEndTurn(this);
    }

    public void GetGold()
    {
        Destroy(emptyBackPack);
        fullBackPack = Instantiate(fullBackPack, backPackParent.transform);
    }

    private void WindowForcMove(Vector3Int nextTile)
    {
        Debug.Log("강제 이동");
        Node targetNode = GameManager.GetInstance.GetNode(nextTile);

        if (targetNode == null) return;

        Debug.Log($"next {nextTile}, target {targetNode.GetCenter}");

        agent.Warp(targetNode.GetCenter);
        
        playerStats.SetCurrentNode(transform.position);
        playerStats.NodeUpdates(transform.position);
    }
}
