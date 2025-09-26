using IStateMachine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BattleTurnStateMachine
{
    public List<BattleTurnState> turnStates;
    public PictureUpdate UnitPicUpdate;
    //리스트로 담긴 했으나
    //Queue와 동일하게 구현
    public BattleTurnStateMachine()
    {
        turnStates = new List<BattleTurnState>();
    }
    public void ChangeState()
    {
        if (turnStates.Count > 1)
        {
            if (turnStates.All(x => x.isEnemy))
            {
                //TODO : 전투 패배 이벤트 실행 필요
            }
            BattleTurnState last = turnStates[0];
            last.Exit();
            RemoveUnit(last);
            turnStates.Add(last);
            turnStates[0].Enter();
            TaskManager.GetInstance.RemoveTurnBehaviour(new TurnTask(GameManager.GetInstance.BattleTurn.ChangeState, 1f));
            TaskManager.GetInstance.AddTurnBehaviour(new TurnTask(GameManager.GetInstance.BattleTurn.ChangeState, 1f));
        }
        else
        {
            //전투종료 함수를 넣거나 해야될듯
        }
    }
    /// <summary>
    /// 유닛 사망, 전장 이탈 시 유닛 객체에서 필드로 BattleTurnState를 통해 이벤트 해제 및 삭제에 필요, 이벤트 등록은 유닛 등록 이후 유닛 객체에서 해결
    /// </summary>
    /// <param name="isEnemy">적인지</param>
    /// <param name="OnStarts">해당 유닛의 턴이 되었을때 실행 될 함수 묶음</param>
    /// <param name="OnEnd">해당 유닛의 턴이 종료되었을때 실행 될 함수 묶음</param>
    /// <returns></returns>
    public BattleTurnState AddUnit(bool isEnemy, Action OnStarts, Action OnEnd)
    {
        BattleTurnState state = new BattleTurnState();
        turnStates.Add(state);
        MergePlayerTurn();
        return state;
    }
    /// <summary>
    /// 전투 턴 큐에서 유닛을 제거합니다
    /// </summary>
    /// <param name="state"></param>
    public void RemoveUnit(BattleTurnState state)
    {
        state.OnEnd = null;
        state.OnStart = null;
        turnStates.Remove(state);
        MergePlayerTurn();
    }
    public void MergePlayerTurn()
    {
        TurnBehaviour start;
        TurnBehaviour end;
        int lastAlly = -1;
        for (int i = 0; i < turnStates.Count; i++)
        {

            if (!turnStates[i].isEnemy)
            {
                if(lastAlly != -1 && i-1 == lastAlly)
                {
                    turnStates[lastAlly].OnStart += turnStates[i].OnStart;
                    turnStates[lastAlly].OnEnd += turnStates[i].OnEnd;
                    if (lastAlly == 0)
                    {
                        turnStates[i].OnStart?.Invoke();
                    }
                    turnStates.RemoveAt(i);
                    i--;
                }
                lastAlly = i;
            }
        }
    }
}
public delegate void PictureUpdate(Sprite sprite);
public class BattleTurnState : IStateBase
{
    public TurnBehaviour OnStart;
    public TurnBehaviour OnEnd;
    public bool isEnemy;

    public void Init(bool isEnemy)
    {
        this.isEnemy = isEnemy;
    }

    public void Enter() 
    {
        OnStart?.Invoke();
    }
    public void Execute() 
    { 
    
    }
    public void Exit()
    {
        OnEnd?.Invoke();
    }
}
