using IStateMachine;
using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateMachine : IStateMachineBase<EnemyState>
{
    private Dictionary<EnemyStates, EnemyState> enemyStates;

    private EnemyState currentState;

    public EnemyState Current => currentState;

    public void ChangeState(EnemyState next)
    {
        currentState = next;
        TaskManager.GetInstance.AddTurnBehaviour(new TurnTask(currentState.Enter, currentState.duration));
        TaskManager.GetInstance.AddTurnBehaviour(new TurnTask(currentState.Exit, 1f));
    }

    public void ForceSet(EnemyState next)
    {
        currentState = next;
        currentState.Enter();
    }
    public EnemyState FindState(EnemyStates statesType)
    {
        return enemyStates[statesType];
    }

    public EnemyStateMachine(EnemyNPC enemyNPC, Animator anim, EnemyStates startType = EnemyStates.PatrolEnemyIdleRotationState)
    {
        enemyStates = new Dictionary<EnemyStates, EnemyState>();
        for (int i = 0; i < Enum.GetValues(typeof(EnemyStates)).Length; i++)
        {
            enemyStates.TryAdd((EnemyStates)i, EnemyState.Factory((EnemyStates)i,enemyNPC,anim));
        }
        currentState = enemyStates[startType];
        currentState.Enter();
    }
}

public enum EnemyStates
{
    PatrolEnemyIdleRotationState,
    PatrolEnemyChaseState,
    PatrolEnemyCombatState,
    PatrolEnemyDamagedState,
    PatrolEnemyDeadState,    
    PatrolEnemyInvestigateState,
    PatrolEnemyLookAroundState,
    PatrolEnemyPatrolState,
    HoldEnemyChaseState,
    HoldEnemyCombatState,
    HoldEnemyDamagedState,
    HoldEnemyDeadState,
    HoldEnemyIdleRotationState,
    HoldEnemyIdleState,
    HoldEnemyInvestigateState,
    HoldEnemyMoveReturnState,
    CopEnemyChaseState,
    CopEnemyCombatState,
    CopEnemyDamagedState,
    CopEnemyDeadState
}

public class EnemyState : IStateBase
{
    public float duration;
    protected Animator anim;
    protected EnemyNPC patrolEnemy;
    public virtual void Enter()
    {

    }

    public virtual void Execute()
    {

    }

    public virtual void Exit()
    {

    }
    public static EnemyState Factory(EnemyStates turnTypes,EnemyNPC enemyNPC,Animator anim)
    {
        switch (turnTypes)
        {
            case EnemyStates.PatrolEnemyIdleRotationState:
                return new PatrolEnemyIdleRotationState(enemyNPC,anim);
            case EnemyStates.PatrolEnemyChaseState:
                return new PatrolEnemyChaseState(enemyNPC, anim);
            case EnemyStates.PatrolEnemyCombatState:
                return new PatrolEnemyCombatState(enemyNPC, anim);
            case EnemyStates.PatrolEnemyDamagedState:
                return new PatrolEnemyDamagedState(enemyNPC, anim);
            case EnemyStates.PatrolEnemyDeadState:
                return new PatrolEnemyDeadState(enemyNPC, anim);
            case EnemyStates.PatrolEnemyInvestigateState:
                return new PatrolEnemyInvestigateState(enemyNPC, anim);
            case EnemyStates.PatrolEnemyLookAroundState:
                return new PatrolEnemyLookAroundState(enemyNPC, anim);
            case EnemyStates.PatrolEnemyPatrolState:
                return new PatrolEnemyPatrolState(enemyNPC, anim);
            case EnemyStates.HoldEnemyChaseState:
                return new HoldEnemyChaseState(enemyNPC, anim);
            case EnemyStates.HoldEnemyCombatState:
                return new HoldEnemyCombatState(enemyNPC, anim);
            case EnemyStates.HoldEnemyDamagedState:
                return new HoldEnemyDamagedState(enemyNPC, anim);
            case EnemyStates.HoldEnemyDeadState:
                return new HoldEnemyDeadState(enemyNPC, anim);
            case EnemyStates.HoldEnemyIdleRotationState:
                return new HoldEnemyIdleRotationState(enemyNPC, anim);
            case EnemyStates.HoldEnemyIdleState:
                return new HoldEnemyIdleState(enemyNPC, anim);
            case EnemyStates.HoldEnemyInvestigateState:
                return new HoldEnemyInvestigateState(enemyNPC, anim);
            case EnemyStates.HoldEnemyMoveReturnState:
                return new HoldEnemyMoveReturnState(enemyNPC, anim);
            case EnemyStates.CopEnemyChaseState:
                return new CopEnemyChaseState(enemyNPC, anim);
            case EnemyStates.CopEnemyCombatState:
                return new CopEnemyCombatState(enemyNPC, anim);
            case EnemyStates.CopEnemyDamagedState:
                return new CopEnemyDamagedState(enemyNPC, anim);
            case EnemyStates.CopEnemyDeadState:
                return new CopEnemyDeadState(enemyNPC, anim);
            default:
                return null;
        }
    }
}
