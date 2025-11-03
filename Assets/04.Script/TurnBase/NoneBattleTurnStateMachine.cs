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
public class NoneBattleTurnStateMachine
{
    NoneBattleTurnStateBase currState;
    public TurnTypes GetCurrState() => currState.StateType();
    private NoneBattleTurnStateBase[] states;
    public Action BuffCount;
    private int round = 1;
    public int currRound { get { return round; } }
    public void ChangeState()
    {
        currState.Exit();

        int typeLen = Enum.GetValues(typeof(TurnTypes)).Length;
        
        if ((int)GetCurrState() + 1 >= typeLen)
        {
            round++;
            BuffCount?.Invoke();
        }

        currState = states[(((int)GetCurrState() + 1) % (typeLen))];
        Debug.Log(GetCurrState());
        
        if(GetCurrState() == TurnTypes.ally) GameManager.GetInstance.StartPlayerTurn();
        
        currState.Enter();
        
        if (currState.StartPointer == null || currState.StartPointer.Method == null|| currState.StartPointer.GetInvocationList().Length  <= 0)
        {
            Debug.Log($"논 배틀턴 등록된 이벤트 없음, 타입 {GetCurrState()}");
            ChangeState();
        }
    }

    public void ForceSet(int index)
    {
        round++;
        currState.Exit();
        currState = states[index];
        currState.Enter();
    }

    /// <summary>
    /// 스크립트를 Reset함는 함수
    /// </summary>
    public void OnSceneChange()
    {
        round = 1;
        BuffCount = null;
        ReleaseDelegateChain();
    }

    public NoneBattleTurnStateMachine(TurnTypes startType = TurnTypes.ally)
    {
        ReleaseDelegateChain();
        states = new NoneBattleTurnStateBase[Enum.GetValues(typeof(TurnTypes)).Length];
        for (int i = 0; i < states.Length; i++)
        {
            states[i] = (NoneBattleTurnStateBase.Factory((TurnTypes)i));
        }
        states[(int)TurnTypes.enemy].StartPointer += NPCDefaultEnterPoint;
        states[(int)TurnTypes.neutral].StartPointer += NPCDefaultEnterPoint;
        currState = states[(int)startType];
        OnSceneChange();
    }

    private void ReleaseDelegateChain()
    {
        if (states == null) return;

        for (int i = 0; i < states.Length; i++)
        {
            if (states[i] == null) continue;
            states[i].EndPointer = null;
            states[i].StartPointer = null;
        }
    }
    
    public void NPCDefaultEnterPoint()
    {
        TaskManager.GetInstance.RemoveTurnBehaviour(new TurnTask(GameManager.GetInstance.NoneBattleTurn.ChangeState, 1f));
        TaskManager.GetInstance.AddTurnBehaviour(new TurnTask(() => { }, 1f));
        TaskManager.GetInstance.AddTurnBehaviour(new TurnTask(GameManager.GetInstance.NoneBattleTurn.ChangeState, 0f));
    }

    /// <summary>
    /// 턴 시작 이벤트를 추가하는 함수
    /// </summary>
    /// <param name="targetType">대상 팀</param>
    /// <param name="pointer">대상 함수</param>
    public void AddStartPointer(TurnTypes targetType,TurnBehaviour pointer)
    {
        if (states[(int)targetType].StateType() == targetType)
        {
            if (GetCurrState() == targetType)
            {
                pointer?.Invoke();
            }
            states[(int)targetType].StartPointer += pointer;
        }
        else
        {
            Debug.LogError($"{targetType} 해당 타입과 인덱스가 일치하지 않습니다");
        }
    }

    /// <summary>
    /// 턴 종료 이벤트를 추가하는 함수
    /// </summary>
    /// <param name="targetType">대상 팀</param>
    /// <param name="pointer">대상 함수</param>
    public void AddEndPointer(TurnTypes targetType,TurnBehaviour pointer)
    {
        
        if (states[(int)targetType].StateType() == targetType)
        {
            states[(int)targetType].EndPointer += pointer;
        }
        else
        {
            Debug.LogError($"{targetType} 해당 타입과 인덱스가 일치하지 않습니다");
        }
    }

    /// <summary>
    /// 사망 시 삭제할 스타트 이벤트
    /// </summary>
    /// <param name="startType">대상 팀</param>
    /// <param name="pointer">대상 함수</param>
    public void RemoveStartPointer(TurnTypes targetType,TurnBehaviour pointer)
    {
        if (states[(int)targetType].StateType() == targetType)
        {
            states[(int)targetType].StartPointer -= pointer;
        }
        else
        {
            Debug.LogError($"{targetType} 해당 타입과 인덱스가 일치하지 않습니다");
        }
    }

    /// <summary>
    /// 사망 시 삭제할 엔드 이벤트
    /// </summary>
    /// <param name="startType">대상 팀</param>
    /// <param name="pointer">대상 함수</param>
    public void RemoveEndPointer(TurnTypes targetType, TurnBehaviour pointer)
    {
        if (states[(int)targetType].StateType() == targetType)
        {
            states[(int)targetType].EndPointer -= pointer;
        }
        else
        {
            Debug.LogError($"{targetType} 해당 타입과 인덱스가 일치하지 않습니다");
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