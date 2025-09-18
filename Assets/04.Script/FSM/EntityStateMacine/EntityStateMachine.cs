using IStateMachine;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EntityStateMachine : IStateMachineBase<EntityState>
{
    public EntityState currentState;
    public EntityTag tag;
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
