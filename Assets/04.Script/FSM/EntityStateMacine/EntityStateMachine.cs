using IStateMachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityStateMachine : IStateMachineBase<EntityState>
{
    private EntityState currentState;
    public void ChangeState(EntityState next)
    {
        currentState.Exit();
        currentState = next;
        currentState.Enter();
    }

    public void ForceSet(EntityState next)
    {
        currentState = next;
        currentState.Enter();
    }
}

public class EntityState : IStateBase
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
