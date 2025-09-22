using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using NodeDefines;

public class NodePlayerController : MonoBehaviour
{
    public NodePlayerCondition playerCondition; // 플레이어 컨디션 (인스펙터에 할당)
    public PlayerStats playerStats { get { return playerCondition.playerStats; } } // 플레이어 스탯 (자동 생성)

    // [변경됨] 캐릭터 고유 번호 대신, 매니저가 관리하는 ID 사용
    public int playerID { get; private set; }

    private Vector3Int vec;
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
    public bool isPickPocketMode;
    public bool isAimingMode;
    public bool isRangeAttackMode;
    public bool isPerkActionMode;

    [Header("명중 보정치")]
    public int hitBonus = 0;

    private bool isEndTurn;
    public bool IsEndTurn { get { return isEndTurn; } }

    void Start()
    {
        if (agent == null) agent = GetComponent<NavMeshAgent>();
        isHide = true;
        isEndTurn = false;
        vec = GameManager.GetInstance.GetNode(transform.position).GetCenter;
        StartMode(ref isMoveMode);

        // [변경됨] 매니저에 자기 자신 등록
        NodePlayerManager.GetInstance.RegisterPlayer(this);
    }

    void Update()
    {
        if (IsMyTurn())
        {
        TurnOnHighlighter(vec, playerCondition.moveRange);
        }
        else
            {
            TurnOffHighlighter();
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
        }
    }

    public void OnClickNode(InputAction.CallbackContext context)
    {
        if (context.started && IsMyTurn() && isMoveMode)
        {
            Vector3 mousePos = Mouse.current.position.ReadValue();
            Move(mousePos);
        }

        if (context.started && IsMyTurn() && isRunMode)
        {
            Debug.Log("달리기");
            playerCondition.ActiveRun();
        }

        if (context.started && IsMyTurn() && isHideMode)
        {
            Debug.Log("숨기");
            HideMode();
        }

        if (context.started && IsMyTurn() && isSneakAttackMode)
        {
            Debug.Log("기습 공격");
            Vector3 mousePos = Mouse.current.position.ReadValue();
            SneakAttack(mousePos);
        }

        if (context.started && IsMyTurn() && isPickPocketMode)
        {
            Debug.Log("훔치기");
            Vector3 mousePos = Mouse.current.position.ReadValue();
            PickPocket(mousePos);
        }

        if (context.started && IsMyTurn() && isAimingMode)
        {
            Debug.Log("조준");
            if (isAiming) RemoveAiming();
            else Aiming();
        }

        if (context.started && IsMyTurn() && isRangeAttackMode)
        {
            Debug.Log("원거리 공격");
            Vector3 mousePos = Mouse.current.position.ReadValue();
            RangeAttack(mousePos);
        }

        if(context.canceled && IsMyTurn() && (isRunMode || isAimingMode || isHideMode))
        {
            StartMode(ref isMoveMode);
        }
    }

    private void Move(Vector3 mouseScreenPos)
    {
        Ray ray = mainCamera.ScreenPointToRay(mouseScreenPos);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Vector3Int targetNodeCenter = GameManager.GetInstance.GetNode(hit.point).GetCenter;
            if (!CheckRange(targetNodeCenter, playerCondition.moveRange))
            {
                Debug.Log("이동 범위를 벗어났습니다!");
                return;
            }

            int cost = CalculateMoveCost(targetNodeCenter);

            if (playerCondition.ConsumeMovement(cost))
            {
                if (GameManager.GetInstance.IsExistNode(targetNodeCenter))
                {
                    TurnOffHighlighter();
                    agent.SetDestination(targetNodeCenter);
                    vec = targetNodeCenter;
                }
            }
            else
            {
                Debug.Log("이동력이 부족합니다!");
            }
        }
    }

    public void OnRun(InputAction.CallbackContext context)
    {
        if (context.started && IsMyTurn() && isMoveMode)
        {
            Debug.Log("달리기 모드 활성화");
            StartMode(ref isRunMode);
        }
    }

    public void OnThrow(InputAction.CallbackContext context)
    {
        if (context.started && IsMyTurn() && isMoveMode)
        {
            Debug.Log("투척 모드 활성화");
            // 던지는 로직
        }
    }

    public void OnHideAndSneakAttack(InputAction.CallbackContext context)
    {
        if (context.started && IsMyTurn() && !isHide && isMoveMode)
        {
            Debug.Log("숨기 모드 활성화");
            StartMode(ref isHideMode);
        }

        if (context.started && IsMyTurn() && isHide && isMoveMode)
        {
            Debug.Log("기습 공격 모드 활성화");
            StartMode(ref isSneakAttackMode);
        }
    }

    private void HideMode()
    {
        isHide = true;
    }

    private void RemoveHideMode()
    {
        isHide = false;
    }

    private void SneakAttack(Vector3 mouseScreenPos)
    {
        Vector3Int targetNodeCenter = GetNodeVector3ByRay(mouseScreenPos);

        if (targetNodeCenter == new Vector3Int(-1, -1, -1))
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

        if (bestNode == new Vector3Int(-1, -1, -1))
        {
            Debug.Log("이동할 수 있는 인접 노드를 찾지 못했습니다.");
            return;
        }

        int cost = CalculateMoveCost(bestNode);

        if (!playerCondition.ConsumeMovement(cost))
        {
            Debug.Log("인접 노드로 이동할 수 있는 이동력 부족!");
            return;
        }

        if (playerCondition.ConsumeActionPoint(1))
        {
            RemoveHideMode();

            agent.SetDestination(bestNode);
            vec = bestNode;
            TurnOffHighlighter();

            Debug.Log("기습 공격 성공!");
        }
        else
        {
            Debug.Log("행동력이 부족합니다!");
        }
    }

    public void OnPickPocket(InputAction.CallbackContext context)
    {
        if (context.started && IsMyTurn() && isMoveMode && isHide)
        {
            Debug.Log("훔치기 모드 활성화");
            StartMode(ref isPickPocketMode);
        }
    }

    private void PickPocket(Vector3 mouseScreenPos)
    {
        Vector3Int targetNodeCenter = GetNodeVector3ByRay(mouseScreenPos);

        if (targetNodeCenter == new Vector3Int(-1, -1, -1))
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

        if (playerCondition.ConsumeActionPoint(1))
        {
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
            Debug.Log("원거리 공격 모드 활성화");
            StartMode(ref isRangeAttackMode);
        }
    }

    private void RangeAttack(Vector3 mouseScreenPos)
    {
        if (CheckRangeAndEntity(GetNodeVector3ByRay(mouseScreenPos), (int)playerStats.attackRange))
        {
            if (playerCondition.ConsumeActionPoint(1))
            {
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
        return (NodePlayerManager.GetInstance.GetCurrentPlayer() == this) && (GameManager.GetInstance.NoneBattleTurn.GetCurrState() == TurnTypes.ally || GameManager.GetInstance.BattleTurn.GetCurrState() == TurnTypes.ally);
    }

    private void TurnOnHighlighter(Vector3Int destination, int range)
    {
        if (destination == GameManager.GetInstance.GetNode(transform.position).GetCenter && !isHighlightOn)
        {
            isHighlightOn = true;
            highlighter.ShowMoveRange(GameManager.GetInstance.GetNode(transform.position).GetCenter, range);
        }
    }

    private void TurnOffHighlighter()
    {
        isHighlightOn = false;
        highlighter.ClearHighlights();
    }

    private void StartMode(ref bool mode)
    {
        isMoveMode = false;
        isSneakAttackMode = false;
        isAimingMode = false;
        isRunMode = false;
        isPickPocketMode = false;
        isRangeAttackMode = false;
        isPerkActionMode = false;

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
        return new Vector3Int(-1, -1, -1); //유효하지 않은 좌표 반환
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

        Vector3Int bestNode = new Vector3Int(-1, -1, -1);
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

}
