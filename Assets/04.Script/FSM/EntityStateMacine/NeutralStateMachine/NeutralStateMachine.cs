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

    public NeutralStateMachine(ManagerNPC managerNPC, NeutralStates startType = NeutralStates.CitizenIdleState) 
    {
        neutralStates = new Dictionary<NeutralStates, NeutralState>();
        for (int i = 0; i < Enum.GetValues(typeof(NeutralStates)).Length; i++)
        {
            neutralStates.TryAdd((NeutralStates)i, NeutralState.Factory((NeutralStates)i, managerNPC));
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

    public static NeutralState Factory(NeutralStates neutralStatesType, ManagerNPC managerNPC)
    {
        switch (neutralStatesType)
        {
            case NeutralStates.CitizenIdleState:
                managerNPC = null;
                return new CitizenIdleState();
            case NeutralStates.CitizenCowerState:
                managerNPC = null;
                return new CitizenCowerState();
            case NeutralStates.CitizenFleeState:
                managerNPC = null;
                return new CitizenFleeState();
            case NeutralStates.CitizenDeadState:
                managerNPC = null;
                return new CitizenDeadState();
            case NeutralStates.ManagerIdleState:
                return new ManagerIdleState(managerNPC);
            case NeutralStates.ManagerIdleCowerState:
                return new ManagerIdleCowerState(managerNPC);
            case NeutralStates.ManagerDeadState:
                return new ManagerDeadState(managerNPC);
            default:
                return null;
        }
    }
}
