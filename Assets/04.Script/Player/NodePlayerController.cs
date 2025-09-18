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

    public bool isEndTurn;

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

    public void OnMove(InputAction.CallbackContext context)
    {
        //무브에 관한 로직, 현재는 마우스 클릭으로 이동
        if (context.started && IsMyTurn())
        {
            Vector3 mousePos = Mouse.current.position.ReadValue();

            Move(mousePos);
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

    public void OnThrow(InputAction.CallbackContext context) 
    {
       if(context.started && IsMyTurn())
        {
            //던지는 로직 1인칭 변환과 함께 투척가능 모션 생성
        }
    }

    public void OnHideAndSneakAttack(InputAction.CallbackContext context) 
    {
        if (context.started && IsMyTurn() && !isHide)
        {
            //숨기 로직
        }
        
        if(context.started && IsMyTurn() && isHide)
        {
            //기습 공격 로직
        }
    }
    public void OnPickPocket(InputAction.CallbackContext context)
    {
        if (context.started && IsMyTurn())
        {
            //소매치기 로직
        }
    }


    public void OnPerkAction(InputAction.CallbackContext context)
    {
        if (context.started && IsMyTurn())
        {
            //특전 로직
        }
    }

    public void OnMelee(InputAction.CallbackContext context)
    {
        if( context.started && IsMyTurn())
        {
            //근접 공격 로직
        }
    }

    public void OnAiming(InputAction.CallbackContext context)
    {
        if (context.started && IsMyTurn())
        {
            //조준 로직
        }

    }
    public void OnRangeAttack(InputAction.CallbackContext context)
    {
        if (context.started && IsMyTurn())
        {
            //원거리 공격 로직
        }

    }

    public void OnEndTurn(InputAction.CallbackContext context)
    {
        if (context.started && IsMyTurn())
        {
            characterTurn = false; // 턴이 끝났으므로 행동 불가
            //GameManager.GetInstance.EndCharacterTurn(characterNumber);
            //EndCharacterTurn() 안에는 앤드갯수를 더해주고 다 끝났는지 체크하는 함수 호출 기능 탑재
            
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

    /// <summary>
    /// 이거 게임매니저로 옮겨버렷
    /// </summary>
    public void CheckAllCharacterEndTurn()
    {
        if(true/*character의 엔드 갯수가 특정 값 이상일 때*/)
        {
            //플레이어 턴 종료
        }
    }

}
