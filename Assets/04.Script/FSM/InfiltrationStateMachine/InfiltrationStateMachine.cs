using UnityEngine;
using IStateMachine;

public class InfiltrationStateMachine : IStateMachineBase<InfiltrationState>
{
    private InfiltrationState currentState;

    public void ChangeState(InfiltrationState next)
    {
        currentState?.Exit();  // 이전 상태 종료
        currentState = next;   // 상태 교체
        currentState.Enter();  // 새 상태 진입
    }

    public void ForceSet(InfiltrationState next)
    {
        currentState = next;   // 그냥 교체
        currentState.Enter();
    }
}

public class InfiltrationState : IStateBase
{
    public virtual void Enter()
    {

    }

    public virtual void Execute()
    {

    }

    public virtual void Exit()
    {

    }
}