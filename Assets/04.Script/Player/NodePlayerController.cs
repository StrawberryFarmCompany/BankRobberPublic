using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using NodeDefines;

public enum CharacterNumber
{
    Character_1,
    Character_2,
    Character_3
}

public class NodePlayerController : MonoBehaviour
{
    public NodePlayerCondition playerCondition; // 플레이어 컨디션 (인스펙터에 할당)
    public PlayerStats playerStats { get{ return playerCondition.playerStats; } } // 플레이어 스탯 (자동 생성)

    public CharacterNumber characterNumber; // 캐릭터 번호

    private Vector3Int vec;
    private bool isHighlightOn = false;

    private bool characterTurn = false;

    [SerializeField] NavMeshAgent agent;
    [SerializeField] Camera mainCamera;

    [SerializeField] private MoveRangeHighlighter highlighter;

    public bool isHide;
    public bool isAiming;

    public bool isEndTurn;

    [Header("현재 플레이어의 액션 상태")]
    public bool isMoveMode;
    public bool isRunMode;
    public bool isSneakAttackMode;
    public bool isPickPocketMode;
    public bool isAimingMode;
    public bool isMeleeMode;
    public bool isRangeAttackMode;
    public bool isPerkActionMode;

    [Header("명중 보정치")]
    public int hitBonus;



    void Start()
    {
        if (agent == null) agent = GetComponent<NavMeshAgent>();
        isHide = true;
        isEndTurn = false;
        vec = GameManager.GetInstance.GetNode(transform.position).GetCenter;
        TurnOnHighlighter(vec, playerCondition.moveRange); 
    }


    void Update()
    {
        TurnOnHighlighter(vec, playerCondition.moveRange);
    }

    public void OnCancel(InputAction.CallbackContext context)
    {
        if (context.started && IsMyTurn())
        {
            StartMode(ref isMoveMode);
        }
    }

    public void OnClickNode(InputAction.CallbackContext context)
    {
        //무브에 관한 로직, 현재는 마우스 클릭으로 이동
        if (context.started && IsMyTurn() && isMoveMode)
        {
            Vector3 mousePos = Mouse.current.position.ReadValue();

            Move(mousePos);
        }

        if(context.started && IsMyTurn() && isRunMode)
        {
            playerCondition.ActiveRun();
        }

        if(context.started && IsMyTurn() && isSneakAttackMode)
        {
            Vector3 mousePos = Mouse.current.position.ReadValue();  //여기에 적 노드 좌표
            SneakAttack(mousePos);
        }

        if(context.started && IsMyTurn() && isPickPocketMode)
        {
            Vector3 mousePos = Mouse.current.position.ReadValue();  //여기에 적 노드 좌표
            PickPocket(mousePos);
        }

        if(context.started && IsMyTurn() && isAimingMode)
        {
            if(isAiming) RemoveAiming();
            else
                Aiming();
        }

        if(context.started && IsMyTurn() && isRangeAttackMode)
        {
            Vector3 mousePos = Mouse.current.position.ReadValue();  //여기에 적 노드 좌표
            RangeAttack(mousePos);
        }

    }

    private void Move(Vector3 mouseScreenPos)
    {
        // 마우스 클릭 위치로 레이 발사
        Ray ray = mainCamera.ScreenPointToRay(mouseScreenPos);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            // 이동하려는 목표 노드의 중심 좌표
            Vector3Int targetNodeCenter = GameManager.GetInstance.GetNode(hit.point).GetCenter;
            if(!CheckRange(targetNodeCenter, playerCondition.moveRange))
            {
                Debug.Log("이동 범위를 벗어났습니다!");
                return;
            }

           int cost = CalculateMoveCost(targetNodeCenter);

            // 현재 이동력이 충분한지 확인
            if (playerCondition.ConsumeMovement(cost))
            {
                // 이동력이 충분할 경우만 이동
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
            StartMode(ref isRunMode);
        }
    }

    public void OnThrow(InputAction.CallbackContext context) 
    {
       if(context.started && IsMyTurn() && isMoveMode)
        {
            //던지는 로직 1인칭 변환과 함께 투척가능 모션 생성
        }
    }

    public void OnHideAndSneakAttack(InputAction.CallbackContext context) 
    {
        if (context.started && IsMyTurn() && !isHide && isMoveMode)
        {
            HideMode();
        }
        
        if(context.started && IsMyTurn() && isHide && isMoveMode)
        {
            StartMode(ref isSneakAttackMode);
        }
    }
    private void HideMode()
    {
        isHide = true;
        //하이드 모드에 진입하면 얻게 되는 이득에 관한 로직
    }

    private void RemoveHideMode()
    {
        isHide = false;
        //하이드 모드에서 벗어나면 얻게 되는 패널티에 관한 로직
    }

    private void SneakAttack(Vector3 mouseScreenPos)
    {
        // 클릭한 위치의 노드 좌표 가져오기
        Vector3Int targetNodeCenter = GetNodeVector3ByRay(mouseScreenPos);

        // 클릭한 위치가 유효하지 않을 경우 탈출
        if (targetNodeCenter == new Vector3Int(-1, -1, -1))
        {
            Debug.Log("유효하지 않은 좌표입니다!");
            return;
        }

        // 지정한 범위(2칸) 내에 있고 적이 존재하는지 체크
        if (!CheckRangeAndEntity(targetNodeCenter, 2))
        {
            Debug.Log("해당 위치에 적이 없거나 범위를 벗어났습니다!");
            return;
        }

        // 주변 인접 노드 중 가장 가까운 이동 가능한 노드 찾기
        Vector3Int bestNode = FindClosestWalkableAdjacentNode(targetNodeCenter);

        if (bestNode == new Vector3Int(-1, -1, -1))
        {
            Debug.Log("이동할 수 있는 인접 노드를 찾지 못했습니다.");
            return;
        }

        int cost = CalculateMoveCost(bestNode);

        if(!playerCondition.ConsumeMovement(cost))
        {
            Debug.Log("인접 노드로 이동할 수 있는 이동력 부족!");
            return;
        }

        // 행동력 소모 및 이동 실행
        if (playerCondition.ConsumeActionPoint(1))
        {
            // 은신 해제
            RemoveHideMode();

            agent.SetDestination(bestNode);
            vec = bestNode;
            TurnOffHighlighter();

            // TODO: 여기서 실제 적 공격 로직 실행
            Debug.Log("기습 공격 성공! 인접 노드로 이동 후 공격 실행");
        }
        else
        {
            Debug.Log("행동력이 부족합니다!");
        }
    }


    //============================================================================================================아직 전투 페이즈의 시체 파밍은 구현 안 됨
    public void OnPickPocket(InputAction.CallbackContext context)
    {
        if (context.started && IsMyTurn() && isMoveMode && isHide)
        {
            //소매치기 로직
            StartMode(ref isPickPocketMode);
        }
    }

    private void PickPocket(Vector3 mouseScreenPos)
    {
        // 클릭한 노드에 위치한 적과 캐릭터와 최단거리에 있는 노드로 이동 후 노드에 있는 적 소매치기 로직
        // 클릭한 위치의 노드 좌표 가져오기
        Vector3Int targetNodeCenter = GetNodeVector3ByRay(mouseScreenPos);

        // 클릭한 위치가 유효하지 않을 경우 탈출
        if (targetNodeCenter == new Vector3Int(-1, -1, -1))
        {
            Debug.Log("유효하지 않은 좌표입니다!");
            return;
        }

        // 지정한 범위(1칸) 내에 있고 훔칠 대상이 존재하는지 체크
        if (!CheckRangeAndInteractable(targetNodeCenter, 1))
        {
            Debug.Log("해당 위치에 훔칠 대상이 없거나 범위를 벗어났습니다!");
            return;
        }
        // 잠입 페이즈 확인
        if (!GameManager.GetInstance.IsNoneBattlePhase())
        {
            Debug.Log("배틀 페이즈에 행동할 수 없습니다!");
            return;
        }

        // 행동력 소모 및 이동 실행
        if (playerCondition.ConsumeActionPoint(1))
        {
            // TODO: 여기서 실제 훔치기 로직 실행
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
            //조준 로직
            StartMode(ref isAimingMode);
        }
    }

    private void Aiming()                                                                                   //나중에 턴 시작 시 에이밍 초기화 필요
    {
        isAiming = true;
        hitBonus += 3;
        //조준 상태로 있을 때 얻는 이득 로직
    }

    private void RemoveAiming()
    {
        isAiming = false;
        hitBonus -= 3;
        //조준 상태에서 벗어날 때 패널티 로직
    }

    public void OnRangeAttack(InputAction.CallbackContext context)
    {
        if (context.started && IsMyTurn() && isMoveMode)
        {
            //원거리 공격 로직
            StartMode(ref isRangeAttackMode);
        }
    }

    private void RangeAttack(Vector3 mouseScreenPos)
    {
        // 클릭한 노드에 위치한 적 노드위치 계산 후 노드에 있는 적 공격하는 로직
        if (CheckRangeAndEntity(GetNodeVector3ByRay(mouseScreenPos), (int)playerStats.attackRange))
        {
            if (playerCondition.ConsumeActionPoint(1))
            {
                //실제 격발하는 로직

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
            //특전 로직 아직 모르니까 보류
        }
    }
    
    
    public void OnEndTurn(InputAction.CallbackContext context)
    {
        if (context.started && IsMyTurn())
        {
            GameManager.GetInstance.EndCharacterTurn(characterNumber);
        }
    }

    /// <summary>
    /// 해당 캐릭터가 활동할 수 있는 조건인가를 반환
    /// </summary>
    /// <returns></returns>
    public bool IsMyTurn()
    {
        return (GameManager.GetInstance.CurrCharacter == characterNumber) && GameManager.GetInstance.PlayerTurn && GameManager.GetInstance.IsCharacterTurn(characterNumber);
    }

    private void TurnOnHighlighter(Vector3Int destination, int range)
    {
        if(destination == GameManager.GetInstance.GetNode(transform.position).GetCenter && !isHighlightOn)
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
        isSneakAttackMode = false;
        isAimingMode = false;
        isRunMode= false;
        isPickPocketMode = false;
        isMeleeMode = false;
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
                Vector3Int current = start + new Vector3Int(x, -1, z); //y값이 안 맞을 수도 있으니까 나중에 버그나면 이놈 탓

                Node node = GameManager.GetInstance.GetNode(current);
                if (node == null || !node.IsWalkable)
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
                Vector3Int current = start + new Vector3Int(x, -1, z); //y값이 안 맞을 수도 있으니까 나중에 버그나면 이놈 탓

                Node node = GameManager.GetInstance.GetNode(current); //요쯤? 엔티티 검출되는지 확인하는 로직
                if (node == null || !node.IsWalkable)
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
                Vector3Int current = start + new Vector3Int(x, -1, z); //y값이 안 맞을 수도 있으니까 나중에 버그나면 이놈 탓

                Node node = GameManager.GetInstance.GetNode(current); //요쯤? 인터랙터블 검출되는지 확인하는 로직
                if (node == null || !node.IsWalkable)
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
            if (node == null || !node.IsWalkable)
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

}
