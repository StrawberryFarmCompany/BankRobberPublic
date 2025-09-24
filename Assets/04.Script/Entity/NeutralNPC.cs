using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeutralNPC : MonoBehaviour
{
    public EntityData entityData;
    protected PlayerStats stats;
    protected NeutralStateMachine nfsm;
    protected Action neutralEntityState;

    protected virtual void Awake()
    {
        stats = new PlayerStats(entityData);
        GameManager.GetInstance.NoneBattleTurn.AddStartPointer(TurnTypes.neutral, CalculateBehaviour);
    }

    protected virtual void CalculateBehaviour()
    {
        if(GameManager.GetInstance.CurrentPhase == GamePhase.NoneBattle)
        {
            TaskManager.GetInstance.AddTurnBehaviour(new TurnTask(GameManager.GetInstance.NoneBattleTurn.ChangeState,0.1f));
        }

        else 
        {
            TaskManager.GetInstance.AddTurnBehaviour(new TurnTask(GameManager.GetInstance.BattleTurn.ChangeState, 0.1f));
        }
    }

    protected virtual void Start()
    {
        
    }

    protected virtual void Update()
    {
        
    }
}
