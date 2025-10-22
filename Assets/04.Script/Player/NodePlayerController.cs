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
using DG.Tweening;
using System;

public class NodePlayerController : MonoBehaviour
{
    public EntityData playerData;
    public EntityStats playerStats;

    private Vector3Int playerVec;

    // [변경됨] GameManager 대신 NodePlayerManager에서 턴 관리
    public PlayerInput playerInput;

    [SerializeField] Camera mainCamera;

    [SerializeField] private MoveRangeHighlighter highlighter;
    public Gun gun;

    [HideInInspector]
    public bool isHide;
    [HideInInspector]
    public bool isAiming;

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
    [HideInInspector]
    public bool isReloadMode;

    [Header("명중 보정치")]
    public int hitBonus = 0;

    private Queue<Vector3Int> pathQueue = new Queue<Vector3Int>();
    public float eta = 0f;
    public bool isMoving;
    public bool canNextMove;

    [Header("백팩")]
    public GameObject fullBackPackPrefab;
    public GameObject backPackParent;
    public GameObject emptyBackPack;
    public GameObject fullBackPack;

    [Header("애니메이션")]
    public AnimationStateController animationController;

    [HideInInspector] public Vector3Int targetNodePos;
    [HideInInspector] public Vector3Int bestNearNodePos;


    private void Awake()
    {
        playerStats = new EntityStats(playerData, gameObject);
        if (gun == null) gun = GetComponent<Gun>();
        if (playerInput == null) playerInput = GetComponent<PlayerInput>();
        playerStats.ForceMove += WindowForcMove;
        isHide = true;
        isEndReady = false;
        StartMode(ref isMoveMode);
        playerStats.OnDead += UnsubscribePlayer;
    }

    void Start()
    {
        playerInput.DeactivateInput();
        playerVec = GameManager.GetInstance.GetNode(transform.position).GetCenter;

        // [변경됨] 매니저에 자기 자신 등록
        NodePlayerManager.GetInstance.RegisterPlayer(this);

        GameManager.GetInstance.NoneBattleTurn.AddStartPointer(TurnTypes.ally, () => { MoveRangeHighlighter.normalHighlighter.Enable(true); });
        GameManager.GetInstance.NoneBattleTurn.AddEndPointer(TurnTypes.ally, () => { MoveRangeHighlighter.normalHighlighter.Enable(false); });

        playerStats.SetCurrentNode(transform.position);
        playerStats.NodeUpdates(transform.position, true);
        transform.position = playerStats.currNode.GetCenter;
        GameManager.GetInstance.RegisterEntity(playerStats);
        NodePlayerManager.GetInstance.SwitchToPlayer(0); // 첫 번째 플레이어로 시작
    }

    void Update()
    {

        if (isMoving)
        {
            SequentialMove();
        }
    }

    void RefreshPipAllSafe()
    {
        var ui = UIManager.GetInstance;
        if (ui?.pip == null) return;
        ui.pip.RefreshAll();
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
            if (node != null && node.Standing.Count > 0)
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
            animationController.RunState();
            playerStats.ActiveRun();
            highlighter.ShowMoveRange(playerStats.currNode.GetCenter, playerStats.movement);
            RefreshPipAllSafe();
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

        if (context.started && IsMyTurn() && isPerkActionMode)
        {
            Vector3 mousePos = Mouse.current.position.ReadValue();
            if (playerStats.playerSkill == PlayerSkill.SneakAttack)
            {
                CheckSneakAttack(mousePos);
            }
            else if (playerStats.playerSkill == PlayerSkill.Heal)
            {
                if(!playerStats.ConsumeActionPoint(1)) return;
                UIManager.GetInstance.ShowActionPanel(true);
                Heal();
                RefreshPipAllSafe();
            }
            else if (playerStats.playerSkill == PlayerSkill.Ready)
            {
                if (!playerStats.ConsumeActionPoint(1)) return;
                UIManager.GetInstance.ShowActionPanel(true);
                Ready();
                RefreshPipAllSafe();
            }
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
            RefreshPipAllSafe();
        }

        if (context.started && IsMyTurn() && isReloadMode)
        {
            if (!playerStats.ConsumeActionPoint(1))
            {
                Debug.Log("행동력이 부족함");
                return;
            }
            gun.Reload();
            animationController.ReloadState();
            UIManager.GetInstance.ShowActionPanel(true);

            RefreshPipAllSafe();
        }

        if (context.started && IsMyTurn() && isRangeAttackMode)
        {
            Vector3 mousePos = Mouse.current.position.ReadValue();
            CheckRangeAttack(mousePos);
        }

        if(context.canceled && IsMyTurn() && (isRunMode || isAimingMode || isHideMode || isReloadMode || isPerkActionMode))
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
        LayerMask layerMask = ~(1 << 8);

        if (Physics.Raycast(ray, out RaycastHit hit,float.PositiveInfinity,layerMask))
        {
            if (GameManager.GetInstance.GetNode(hit.point) == null)
            {
                Debug.Log("노드가 아니다.");
                return;
            }

            if (!GameManager.GetInstance.GetNode(hit.point).isWalkable || (GameManager.GetInstance.GetEntityAt(GameManager.GetInstance.GetNode(hit.point).GetCenter) != null ))
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
                animationController.MoveState();
                playerVec = pathQueue.Last();
                playerStats.NodeUpdates(playerVec);
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
        eta -= Time.deltaTime;
        if (eta <= 0f) canNextMove = true;

        if (canNextMove && pathQueue.Count > 0)
        {
            canNextMove = false;
            eta = DoMoveAndRotate(Ease.Unset ,pathQueue.Dequeue(), 0.2f, 0.3f,()=> {
                playerStats.SetCurrentNode(transform.position);
                playerStats.NodeUpdates(transform.position);
            });

            /*transform.DOComplete(true);
            transform.DOMove(pathQueue.Dequeue(), 0.3f).OnComplete(()=> { playerStats.NodeUpdates(transform.position); });*/
            
        }

        // 모든 경로 소모 시 이동 종료
        if (pathQueue.Count == 0 && eta <= 0f)
        {
            isMoving = false;
            eta = 0f;

            if (NodePlayerManager.GetInstance.GetCurrentPlayer() == this)
            {
                playerStats.SetCurrentNode(transform.position);
                playerStats.NodeUpdates(transform.position);
                TurnOnHighlighter();
                RefreshPipAllSafe();
            }
            else
            {
                animationController.IdleState();
            }
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
        Vector3Int targetNodeCenter = GetNodeVector3ByRay(mouseScreenPos, ~(1 << 8));

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
            targetNodePos = targetNodeCenter;
            animationController.ThrowState();
            StartMode(ref isMoveMode);
            RefreshPipAllSafe();
        }
        else
        {
            Debug.Log("행동력이 부족합니다!");
        }
    }


    public void OnHide(InputAction.CallbackContext context)
    {
        if (context.started && IsMyTurn() && !isHide && isMoveMode)
        {
            UIManager.GetInstance.ShowActionPanel(false);
            StartMode(ref isHideMode);
        }
    }

    private void HideMode()
    {
        animationController.HideState();
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
        Vector3Int targetNodeCenter = GetNodeVector3ByRay(mouseScreenPos, (1 << 8),true);

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
            targetNodePos = targetNodeCenter;
            bestNearNodePos = bestNode;
            RemoveHideMode();

            animationController.OnUnEquipForSneak();
            
            StartMode(ref isMoveMode);

            RefreshPipAllSafe();
        }
        else
        {
            Debug.Log("행동력이 부족합니다!");
        }
    }

    public void MoveBestNode()
    {
        transform.DOMove(bestNearNodePos, 0.3f).OnComplete(()=>
        {
            playerStats.NodeUpdates(bestNearNodePos);
            playerVec = bestNearNodePos;
            TurnOffHighlighter();
            RefreshPipAllSafe();
        });
    }

    public void SneakAttack(Vector3Int targetPos)
    {
        int result = DiceManager.GetInstance.DirrectRoll(0, 6, 3);
        if (result + hitBonus - GameManager.GetInstance.GetEntityAt(targetPos).evasionRate > 0)
        {
            int resultDamage = DiceManager.GetInstance.DirrectRoll(0, 6, 2);
            Debug.Log($"{result}의 데미지를 상대에게 줌");
            GameManager.GetInstance.GetEntityAt(targetPos).Damaged(resultDamage);
            animationController.SneakAttackState();
        }
        else
        {
            Debug.Log("스니크 어택 빗나감!");
            return;
        }
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
        Vector3Int targetNodeCenter = GetNodeVector3ByRay(mouseScreenPos, ~(1 << 8));

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
            RefreshPipAllSafe();
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

    public void OnNodeSelection(InputAction.CallbackContext ctx)
    {
        if (IsMyTurn() && isMoveMode && !isMoving && this == NodePlayerManager.GetInstance.GetCurrentPlayer())
        {
            Vector2 pos = ctx.ReadValue<Vector2>();
            Vector3Int selectedNode = GetNodeVector3ByRay(pos, ~(1 << 8));
            if (MoveRangeHighlighter.normalHighlighter.IsPosCludeInBound(selectedNode))
            {
                MoveRangeHighlighter.normalHighlighter.SetGoalPos(selectedNode);
                List<Vector3Int> list = new List<Vector3Int>();
                list.Add(playerStats.currNode.GetCenter);
                list.AddRange(GenerateChebyshevPath(playerStats.currNode.GetCenter, selectedNode));
                MoveRangeHighlighter.normalHighlighter.SetPathLine(list.ToArray());
            }
            else
            {
                MoveRangeHighlighter.normalHighlighter.GoalPreviewOnOff(false);
            }
        }
        else
        {
            MoveRangeHighlighter.normalHighlighter.GoalPreviewOnOff(false);
        }
    }

    private void Aiming()
    {
        animationController.AimingState();
        isAiming = true;
        hitBonus += 3;
    }

    private void RemoveAiming()
    {
        animationController.UnAiming();
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
        Vector3Int targetPos = GetNodeVector3ByRay(mouseScreenPos, (1 << 8),true);

        Vector3 start = transform.position;
        Vector3 target = targetPos;
        LayerMask layerMask = ~(1 << 8);
        if (Physics.Raycast(new Ray(start+Vector3.up, (target - start).normalized),out RaycastHit hit, Vector3.Distance(start, target), layerMask))
        {
            Debug.Log($"방향성 : {(target - start).normalized} ");
            Debug.Log($"무언가로 막혀있음 : {hit.collider.name} ");
            return;
        }

        if (!CheckRangeAndEntity(targetPos, (int)playerStats.attackRange))
        {
            Debug.Log("엔티티가 없엉");
            return;

        }
        //행동력 소모를 막기 위한 조건문,  shoot에서도 처리는 하나 return을 위한 파라메터
        if (!gun.CheckAmmo())
        {
            Debug.Log("잔탄수 부족, 불발");
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
            animationController.AimRangedAttackState(targetPos);
            RemoveAiming();
        }
        else
        {
            animationController.HipRangedAttackState(targetPos);
        }
        RefreshPipAllSafe();
        TurnOffHighlighter();
        StartMode(ref isMoveMode);
    }


    public void OnPerkAction(InputAction.CallbackContext context)
    {
        if (context.started && IsMyTurn() && isMoveMode)
        {
            UIManager.GetInstance.ShowActionPanel(false);
            StartMode(ref isPerkActionMode);
        }
    }

    public void Heal()
    {
        animationController.HealState();
    }

    public void Ready()
    {
        animationController.ReadyState();
    }
    

    public void OnReload(InputAction.CallbackContext context)
    {
        if (context.started && IsMyTurn() && isMoveMode)
        {
            UIManager.GetInstance.ShowActionPanel(false);
            StartMode(ref isReloadMode);
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
    public void TurnOnHighlighter()
    {
        animationController.IdleState();
        highlighter.ShowMoveRange(playerStats.currNode.GetCenter, playerStats.movement);

    }
    public void TurnOnHighlighter(Vector3Int destination, int range)
    {
        if (destination == GameManager.GetInstance.GetNode(transform.position).GetCenter && !MoveRangeHighlighter.normalHighlighter.isGoalActivated)
        {
            animationController.IdleState();
            highlighter.ShowMoveRange(GameManager.GetInstance.GetNode(transform.position).GetCenter, range);
        }
    }

    public void TurnOnHighlighter(int range)
    {
            highlighter.ShowMoveRange(GameManager.GetInstance.GetNode(transform.position).GetCenter, range);
        
    }

    public void TurnOffHighlighter()
    {
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
        isReloadMode = false;

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
    //TODO : 의도 파악하여 추후 리펙토링 필요
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

    public Vector3Int GetNodeVector3ByRay(Vector3 mouseScreenPos,LayerMask layer,bool isEntityTarget = false)
    {
        Ray ray = mainCamera.ScreenPointToRay(mouseScreenPos);

        if (Physics.Raycast(ray, out RaycastHit hit, float.PositiveInfinity, layer))
        {
            Node node = GameManager.GetInstance.GetNode(isEntityTarget ? hit.transform.position : hit.point);
            if(node != null)return node.GetCenter;
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
        fullBackPack = Instantiate(fullBackPackPrefab, backPackParent.transform);
    }

    private void WindowForcMove(Vector3Int nextTile)
    {
        Debug.Log("강제 이동");
        Node targetNode = GameManager.GetInstance.GetNode(nextTile);

        if (targetNode == null) return;

        Debug.Log($"next {nextTile}, target {targetNode.GetCenter}");

        transform.DOComplete(true);


        DoMoveAndRotate(Ease.InCirc, nextTile, 0.2f, 0.1f,()=> 
        {
            playerStats.SetCurrentNode(transform.position);
            playerStats.NodeUpdates(transform.position);
            highlighter.ShowMoveRange(playerStats.currNode.GetCenter, playerStats.movement);
            RefreshPipAllSafe();
        });
    }

    private float DoMoveAndRotate(Ease ease, Vector3Int pos, float moveDuration, float rotationDuration, Action action = null)
    {
        transform.DOComplete(true);

        Vector2 relPos = new Vector2(pos.x, pos.z) - new Vector2(transform.position.x, transform.position.z);
        float radian = Mathf.Atan2(relPos.x, relPos.y);
        float angle = (Mathf.Rad2Deg * radian);


        float minAngle = (Mathf.Min(angle, transform.eulerAngles.y) + 180) % 360f;
        float maxAngle = (Mathf.Max(angle, transform.eulerAngles.y) + 180) % 360f;

        float rotAngle = (maxAngle - minAngle) / 360f;
        if (rotAngle == 0)
        {
            rotationDuration = 0;
        }
        else
        {
            float originRotDur = rotationDuration;
            rotationDuration = originRotDur*rotAngle;
            rotationDuration = MathF.Abs(rotationDuration);
        }
        var rotationSeq = transform.DORotate(Vector3.up* angle, rotationDuration).OnComplete(()=> 
        {
            var moveSeq = transform.DOMove(pos, moveDuration);

            moveSeq.SetEase(ease);
            moveSeq.OnComplete(() =>
            {
                if (playerStats == null) return;
                action?.Invoke();
                Debug.Log(playerStats.currNode.GetCenter);
            });
        });
        return moveDuration + rotationDuration;
    }
    /// <summary>
    /// EntityStats에 있는 OnDead 이벤트에 연결된 함수
    /// 그 이외에 탈출 루트 이벤트에도 연결 가능
    /// </summary>
    public void UnsubscribePlayer()
    {
        NodePlayerManager.GetInstance.UnregisterPlayer(this);
    }
    private void OnDestroy()
    {
        transform.DOKill(false);
    }
}
