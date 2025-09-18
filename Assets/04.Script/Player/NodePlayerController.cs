using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public enum CharacterNumber
{
    Character_1,
    Character_2,
    Character_3
}

public class NodePlayerController : MonoBehaviour
{
    public EntityData playerData; // 플레이어 데이터 (인스펙터에 할당)

    public BattleTurnStateMachine turnMachine; // 인스펙터에 할당
    public BattleTurnState myTurnState;        // 플레이어 자신의 턴 상태 (AddUnit 반환값 저장)
    public NoneBattleTurnStateBase noneBattleTurnState; // 잠입 턴 상태 머신
    public AllayTurnState allayTurnState; // 아군 턴 상태 (AddUnit 반환값 저장)

    public CharacterNumber characterNumber; // 캐릭터 번호

    private Vector3Int vec;
    private bool isHighlightOn = false;

    private bool playerTurn = false;
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


    void Start()
    {
        if (agent == null) agent = GetComponent<NavMeshAgent>();
        isHide = true;
        isEndTurn = false;
        vec = GameManager.GetInstance.GetNode(transform.position).GetCenter;
        TurnOnHighlighter(vec);
    }


    void Update()
    {
        // 현재 턴이 플레이어인지 확인
        if (turnMachine != null && turnMachine.turnStates.Count > 0)
        {
            BattleTurnState current = turnMachine.turnStates[0];
            playerTurn = (current == myTurnState && myTurnState.isEnemy == false);
        }
        TurnOnHighlighter(vec);
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
            //행동력 소모 후 이동력 회복
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


    }

    private void Move(Vector3 mouseScreenPos)
    {
        //마우스 클릭을 통한 이동 로직
        Ray ray = mainCamera.ScreenPointToRay(mouseScreenPos);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            vec = GameManager.GetInstance.GetNode(hit.point).GetCenter;
            if (GameManager.GetInstance.IsExistNode(vec))
            {
                TurnOffHighlighter();
                agent.SetDestination(vec);
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
            StartHideMode();
        }
        
        if(context.started && IsMyTurn() && isHide && isMoveMode)
        {
            StartMode(ref isSneakAttackMode);
        }
    }
    private void StartHideMode()
    {
        isHide = true;
        //하이드 모드에 진입하면 얻게 되는 이득에 관한 로직
    }

    private void SneakAttack(Vector3 mouseScreenPos)
    {
        // 클릭한 노드에 위치한 적과 캐릭터와 최단거리에 있는 노드로 이동 후 노드에 있는 적 공격하는 로직
    }

    public void OnPickPocket(InputAction.CallbackContext context)
    {
        if (context.started && IsMyTurn() && isMoveMode)
        {
            //소매치기 로직
            StartMode(ref isPickPocketMode);
        }
    }

    private void PickPocket(Vector3 mouseScreenPos)
    {
        // 클릭한 노드에 위치한 적과 캐릭터와 최단거리에 있는 노드로 이동 후 노드에 있는 적 소매치기 로직
    }


    public void OnPerkAction(InputAction.CallbackContext context)
    {
        if (context.started && IsMyTurn())
        {
            //특전 로직 아직 모르니까 보류
        }
    }

    public void OnMelee(InputAction.CallbackContext context)
    {
        if( context.started && IsMyTurn() && isMoveMode)
        {
            //근접 공격 로직
            StartMode(ref isMeleeMode);
        }
    }

    public void MeleeAttack(Vector3 mouseScreenPos)
    {
        // 클릭한 노드에 위치한 적과 캐릭터와 최단거리에 있는 노드로 이동 후 노드에 있는 적 공격하는 로직
    }

    public void OnAiming(InputAction.CallbackContext context)
    {
        if (context.started && IsMyTurn() && isMoveMode && !isAiming)
        {
            //조준 로직
            StartMode(ref isAimingMode);
        }

    }

    private void Aiming()
    {
        isAiming = true;
        //조준 상태로 있을 때 얻는 이득 로직
    }

    private void RemoveAiming()
    {
        isAiming = false;
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
    }

    
    public void OnEndTurn(InputAction.CallbackContext context)
    {
        if (context.started && IsMyTurn())
        {
            characterTurn = false; // 턴이 끝났으므로 행동 불가
            GameManager.GetInstance.EndCharacterTurn(characterNumber);
            
        }
    }

    /// <summary>
    /// 해당 캐릭터가 활동할 수 있는 조건인가를 반환
    /// </summary>
    /// <returns></returns>
    public bool IsMyTurn()
    {
        return (GameManager.GetInstance.CurrCharacter == characterNumber) && playerTurn && characterTurn;
    }

    private void TurnOnHighlighter(Vector3Int destination)
    {
        if(destination == GameManager.GetInstance.GetNode(transform.position).GetCenter && !isHighlightOn)
        {
            isHighlightOn = true;
            highlighter.ShowMoveRange(GameManager.GetInstance.GetNode(transform.position).GetCenter);
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

}
