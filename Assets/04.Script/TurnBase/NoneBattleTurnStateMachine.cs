using IStateMachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.VersionControl.Asset;

/// <summary>
/// 잠입턴 스테이트머신
/// </summary>
public class NoneBattleTurnStateMachine : IStateMachineBase<TurnStateBase>
{
    TurnStateBase currState;
    public TurnTypes GetCurrState() => currState.StateType();
    private Dictionary<TurnTypes,TurnStateBase> states;
    
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
    public NoneBattleTurnStateMachine(TurnTypes startType = TurnTypes.allay)
    {
        states = new Dictionary<TurnTypes, TurnStateBase>();
        for (int i = 0; i < Enum.GetValues(typeof(TurnTypes)).Length; i++)
        {
            states.TryAdd((TurnTypes)i, TurnStateBase.Factory((TurnTypes)i));
        }
        currState = states[startType];
    }
    public void AddStartPointer(TurnTypes startType,TurnStateBase.TurnBehaviour pointer)
    {
        if (states.ContainsKey(startType))
        {
            if (GetCurrState() == startType)
            {
                pointer?.Invoke();
            }
            states[startType].StartPointer += pointer;
        }
        else
        {
            Debug.LogError($"{startType} 해당 타입이 딕셔너리에 있지 않습니다");
        }
    }
    public void AddEndPointer(TurnTypes startType,TurnStateBase.TurnBehaviour pointer)
    {
        if (states.ContainsKey(startType))
        {
            states[startType].EndPointer += pointer;
        }
        else
        {
            Debug.LogError($"{startType} 해당 타입이 딕셔너리에 있지 않습니다");
        }
    }
    public void RemoveStartPointer(TurnTypes startType,TurnStateBase.TurnBehaviour pointer)
    {
        if (states.ContainsKey(startType))
        {
            states[startType].StartPointer -= pointer;
        }
        else
        {
            Debug.LogError($"{startType} 해당 타입이 딕셔너리에 있지 않습니다");
        }
    }
    public void RemoveEndPointer(TurnTypes startType,TurnStateBase.TurnBehaviour pointer)
    {
        if (states.ContainsKey(startType))
        {
            states[startType].EndPointer -= pointer;
        }
        else
        {
            Debug.LogError($"{startType} 해당 타입이 딕셔너리에 있지 않습니다");
        }
    }

}
public enum TurnTypes{allay,enemy,neutral}
public class TurnStateBase : IStateBase
{
    public virtual TurnTypes StateType() => TurnTypes.allay;
    public delegate void TurnBehaviour();
    public TurnBehaviour StartPointer;
    public TurnBehaviour EndPointer;
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
    public override TurnTypes StateType() => TurnTypes.enemy;

    public override void Enter() 
    {
        StartPointer?.Invoke();
    }
    public override void Exit() 
    { 
        EndPointer?.Invoke();
    }
}
public class NeutralTurnState : TurnStateBase
{
    public override TurnTypes StateType() => TurnTypes.neutral;

    public override void Enter() 
    { 
        StartPointer?.Invoke();
    }
    public override void Exit() 
    {
        EndPointer?.Invoke();
    }
}
public class AllayTurnState : TurnStateBase
{
    public override TurnTypes StateType() => TurnTypes.allay;

    public override void Enter() 
    { 
        StartPointer?.Invoke();
    }
    public override void Exit() 
    {
        EndPointer?.Invoke();
    }
}