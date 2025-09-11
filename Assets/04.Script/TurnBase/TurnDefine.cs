using IStateMachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class TurnStateMachine : IStateMachineBase<TurnStateBase>
{
    TurnStateBase currState;
    public Dictionary<TurnTypes,TurnStateBase> states;
    public void ChangeState(TurnStateBase next)
    {
        if (currState == next) return;
        currState.Exit();
        currState = next;
        currState.Enter();
    }
    public void ForceSet(TurnStateBase next)
    {
        currState.Exit();
        currState = next;
        currState.Enter();
    }
    public TurnStateBase FindState(TurnTypes type)
    {
        return states[type];
    }
    public TurnStateMachine(TurnTypes startType = TurnTypes.allay)
    {
        for (int i = 0; i < Enum.GetValues(typeof(TurnTypes)).Length; i++)
        {
            states.TryAdd((TurnTypes)i, TurnStateBase.Factory((TurnTypes)i));
        }
        currState = states[TurnTypes.allay];
    }
}
public enum TurnTypes{allay,enemy,neutral}
public class TurnStateBase : IStateBase
{
    public virtual void Enter() { }
    public virtual void Execute() { }
    public virtual void Exit() { }
    public static TurnStateBase Factory(TurnTypes turnTypes)
    {
        switch (turnTypes)
        {
            case TurnTypes.allay:
                return new AllayTurnState();
            case TurnTypes.enemy:
                return new EnemyTurnState();
            case TurnTypes.neutral:
                return new NeutralTurnState();
            default:
                return null;
        }
    }
}
public class EnemyTurnState : TurnStateBase
{
    public override void Enter() 
    { 
    
    }
    public override void Exit() 
    { 
    
    }
}
public class NeutralTurnState : TurnStateBase
{
    public override void Enter() 
    { 
    
    }
    public override void Exit() 
    { 
    
    }
}
public class AllayTurnState : TurnStateBase
{
    public override void Enter() 
    { 
    
    }
    public override void Exit() 
    { 
    
    }
}