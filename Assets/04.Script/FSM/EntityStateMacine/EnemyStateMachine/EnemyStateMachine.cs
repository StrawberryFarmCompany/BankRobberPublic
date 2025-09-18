using IStateMachine;
using System;

public class EnemyStateMachine : IStateMachineBase<EnemyState>
{
    private EnemyState currentState;

    public EnemyState Current => currentState;

    public void ChangeState(EnemyState next)
    {
        currentState?.Exit();
        currentState = next;
        currentState.Enter();
    }

    public void ForceSet(EnemyState next)
    {
        currentState = next;
        currentState.Enter();
    }
}

public class EnemyState : IStateBase
{
    public Action StartAction;
    public Action EndAction;

    public virtual void Enter()
    {
         StartAction?.Invoke();
    }

    public virtual void Execute()
    {
        
    }

    public virtual void Exit()
    {
        EndAction?.Invoke();
    }
}
