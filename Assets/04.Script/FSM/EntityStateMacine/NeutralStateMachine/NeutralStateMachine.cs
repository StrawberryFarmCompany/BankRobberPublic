using IStateMachine;
using System;
using System.Collections.Generic;
using UnityEngine;

public class NeutralStateMachine : IStateMachineBase<NeutralState>
{
    private Dictionary<NeutralStates, NeutralState> neutralStates;
    public float eta;
    public NeutralState currentState;

    public NeutralState Current => currentState;

    public void ChangeState(NeutralState next)
    {
        currentState = next;
        TaskManager.GetInstance.AddTurnBehaviour(new TurnTask(currentState.Enter, currentState.duration = 1 * eta));
        TaskManager.GetInstance.AddTurnBehaviour(new TurnTask(currentState.Exit, 0f));
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

    public NeutralStateMachine(NeutralNPC neutralNPC,Animator anim, NeutralStates startType = NeutralStates.CitizenIdleState) 
    {
        neutralStates = new Dictionary<NeutralStates, NeutralState>();
        for (int i = 0; i < Enum.GetValues(typeof(NeutralStates)).Length; i++)
        {
            neutralStates.TryAdd((NeutralStates)i, NeutralState.Factory((NeutralStates)i,neutralNPC,anim));
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
    public float duration;
    protected Animator anim;
    protected NeutralNPC neutralNPC;

    public virtual void Enter()
    {

    }

    public virtual void Execute()
    {

    }

    public virtual void Exit()
    {

    }

    public static NeutralState Factory(NeutralStates neutralStatesType,NeutralNPC neutralNPC, Animator anim)
    {
        switch (neutralStatesType)
        {
            case NeutralStates.CitizenIdleState:
                return new CitizenIdleState(neutralNPC,anim);
            case NeutralStates.CitizenCowerState:
                return new CitizenCowerState(neutralNPC, anim);
            case NeutralStates.CitizenFleeState:
                return new CitizenFleeState(neutralNPC, anim);
            case NeutralStates.CitizenDeadState:
                return new CitizenDeadState(neutralNPC, anim);
            case NeutralStates.ManagerIdleState:
                return new ManagerIdleState(neutralNPC, anim);
            case NeutralStates.ManagerIdleCowerState:
                return new ManagerIdleCowerState(neutralNPC, anim);
            case NeutralStates.ManagerDeadState:
                return new ManagerDeadState(neutralNPC, anim);
            default:
                return null;
        }
    }
}
