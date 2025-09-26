using IStateMachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 잠입턴 스테이트머신
/// </summary>
public class NoneBattleTurnStateMachine : IStateMachineBase<NoneBattleTurnStateBase>
{
    NoneBattleTurnStateBase currState;
    public TurnTypes GetCurrState() => currState.StateType();
    private Dictionary<TurnTypes,NoneBattleTurnStateBase> states;
    
    public void ChangeState()
    {
        currState.Exit();
        currState = states[(TurnTypes)(((int)GetCurrState()+1) % (Enum.GetValues(typeof(TurnTypes)).Length))];
        Debug.Log(GetCurrState());
        currState.Enter();
    }
    public void ChangeState(NoneBattleTurnStateBase next)
    {
        if (currState == next) return;
        currState.Exit();
        currState = next;
        currState.Enter();
    }
    public void ForceSet(NoneBattleTurnStateBase next)
    {
        currState.Exit();
        currState = next;
        currState.Enter();
    }
    public NoneBattleTurnStateBase FindState(TurnTypes type)
    {
        return states[type];
    }
    public NoneBattleTurnStateMachine(TurnTypes startType = TurnTypes.ally)
    {
        states = new Dictionary<TurnTypes, NoneBattleTurnStateBase>();
        for (int i = 0; i < Enum.GetValues(typeof(TurnTypes)).Length; i++)
        {
            states.TryAdd((TurnTypes)i, NoneBattleTurnStateBase.Factory((TurnTypes)i));
        }
        states[TurnTypes.enemy].StartPointer += NPCDefaultEnterPoint;
        states[TurnTypes.neutral].StartPointer += NPCDefaultEnterPoint;
        currState = states[startType];
    }
    public void NPCDefaultEnterPoint()
    {
        TaskManager.GetInstance.RemoveTurnBehaviour(new TurnTask(GameManager.GetInstance.NoneBattleTurn.ChangeState, 1f));
        TaskManager.GetInstance.AddTurnBehaviour(new TurnTask(GameManager.GetInstance.NoneBattleTurn.ChangeState, 1f));
        TaskManager.GetInstance.StartTask();
    }
    /// <summary>
    /// 턴 시작 이벤트를 추가하는 함수
    /// </summary>
    /// <param name="startType">대상 팀</param>
    /// <param name="pointer">대상 함수</param>
    public void AddStartPointer(TurnTypes startType,TurnBehaviour pointer)
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

    /// <summary>
    /// 턴 종료 이벤트를 추가하는 함수
    /// </summary>
    /// <param name="startType">대상 팀</param>
    /// <param name="pointer">대상 함수</param>
    public void AddEndPointer(TurnTypes startType,TurnBehaviour pointer)
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

    /// <summary>
    /// 사망 시 삭제할 스타트 이벤트
    /// </summary>
    /// <param name="startType">대상 팀</param>
    /// <param name="pointer">대상 함수</param>
    public void RemoveStartPointer(TurnTypes startType,TurnBehaviour pointer)
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


    /// <summary>
    /// 사망 시 삭제할 엔드 이벤트
    /// </summary>
    /// <param name="startType">대상 팀</param>
    /// <param name="pointer">대상 함수</param>
    public void RemoveEndPointer(TurnTypes startType,TurnBehaviour pointer)
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

    public TurnTypes GetNextTurn()
    {
        TurnTypes type = GetCurrState();
        int num = (int)type;
        num = (num + 1) % Enum.GetValues(typeof(TurnTypes)).Length;
        return (TurnTypes) num;
    }

}
public enum TurnTypes{ally,enemy,neutral}
public delegate void TurnBehaviour();
public class NoneBattleTurnStateBase : IStateBase
{
    public virtual TurnTypes StateType() => TurnTypes.ally;

    public TurnBehaviour StartPointer;
    public TurnBehaviour EndPointer;
    public virtual void Enter() { }
    public virtual void Execute() { }
    public virtual void Exit() { }
    public static NoneBattleTurnStateBase Factory(TurnTypes turnTypes)
    {
        switch (turnTypes)
        {
            case TurnTypes.ally:
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
public class EnemyTurnState : NoneBattleTurnStateBase
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
public class NeutralTurnState : NoneBattleTurnStateBase
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
public class AllayTurnState : NoneBattleTurnStateBase
{
    public override TurnTypes StateType() => TurnTypes.ally;

    public override void Enter() 
    { 
        StartPointer?.Invoke();
    }
    public override void Exit() 
    {
        EndPointer?.Invoke();
    }
}