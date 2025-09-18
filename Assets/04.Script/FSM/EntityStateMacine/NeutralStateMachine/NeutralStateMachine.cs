using IStateMachine;

public class NeutralStateMachine : IStateMachineBase<NeutralState>
{
    private NeutralState currentState;

    public NeutralState Current => currentState;

    public void ChangeState(NeutralState next)
    {
        currentState?.Exit();
        currentState = next;
        currentState.Enter();
    }

    public void ForceSet(NeutralState next)
    {
        currentState = next;
        currentState.Enter();
    }
}


public class NeutralState : IStateBase 
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
