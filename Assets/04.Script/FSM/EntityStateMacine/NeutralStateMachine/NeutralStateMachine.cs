using IStateMachine;
using System;
using System.Collections.Generic;

public class NeutralStateMachine : IStateMachineBase<NeutralState>
{
    private Dictionary<NeutralStates, NeutralState> neutralStates;

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

    public NeutralState FindState(NeutralStates statesType)
    {
        return neutralStates[statesType];
    }

    public NeutralStateMachine(NeutralStates startType = NeutralStates.CitizenIdleState) 
    {
        neutralStates = new Dictionary<NeutralStates, NeutralState>();
        for (int i = 0; i < Enum.GetValues(typeof(NeutralStates)).Length; i++)
        {
            neutralStates.TryAdd((NeutralStates)i, NeutralState.Factory((NeutralStates)i));
        }
        currentState = neutralStates[startType];
        currentState.Enter();
    }

}

public enum NeutralStates
{
    CitizenIdleState,
    CitizenCowerState,
    CitizenFleeState,
    CitizenDeadState,
    ManagerIdleState,
    ManagerIdleCowerState,
    ManagerDeadState
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

    public static NeutralState Factory(NeutralStates neutralStatesType)
    {
        switch (neutralStatesType)
        {
            case NeutralStates.CitizenIdleState:
                return new CitizenIdleState();
            case NeutralStates.CitizenCowerState:
                return new CitizenCowerState();
            case NeutralStates.CitizenFleeState:
                return new CitizenFleeState();
            case NeutralStates.CitizenDeadState:
                return new CitizenDeadState();
            case NeutralStates.ManagerIdleState:
                return new ManagerIdleState();
            case NeutralStates.ManagerIdleCowerState:
                return new ManagerIdleCowerState();
            case NeutralStates.ManagerDeadState:
                return new ManagerDeadState();
            default:
                return null;
        }
    }
}
