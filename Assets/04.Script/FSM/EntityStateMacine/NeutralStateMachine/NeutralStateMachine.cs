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
        currentState = next;
        TaskManager.GetInstance.AddTurnBehaviour(new TurnTask(currentState.Enter, currentState.duration));
        TaskManager.GetInstance.AddTurnBehaviour(new TurnTask(currentState.Exit, 0.1f));
        TaskManager.GetInstance.StartTask();
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

    public NeutralStateMachine(NeutralNPC neutralNPC,NeutralStates startType = NeutralStates.CitizenIdleState) 
    {
        neutralStates = new Dictionary<NeutralStates, NeutralState>();
        for (int i = 0; i < Enum.GetValues(typeof(NeutralStates)).Length; i++)
        {
            neutralStates.TryAdd((NeutralStates)i, NeutralState.Factory((NeutralStates)i,neutralNPC));
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
    Action StartAction;
    Action EndAction;
    public float duration;

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

    public static NeutralState Factory(NeutralStates neutralStatesType,NeutralNPC neutralNPC)
    {
        switch (neutralStatesType)
        {
            case NeutralStates.CitizenIdleState:
                return new CitizenIdleState(neutralNPC);
            case NeutralStates.CitizenCowerState:
                return new CitizenCowerState(neutralNPC);
            case NeutralStates.CitizenFleeState:
                return new CitizenFleeState(neutralNPC);
            case NeutralStates.CitizenDeadState:
                return new CitizenDeadState(neutralNPC);
            case NeutralStates.ManagerIdleState:
                return new ManagerIdleState(neutralNPC);
            case NeutralStates.ManagerIdleCowerState:
                return new ManagerIdleCowerState(neutralNPC);
            case NeutralStates.ManagerDeadState:
                return new ManagerDeadState(neutralNPC);
            default:
                return null;
        }
    }
}
