using IStateMachine;
using System;
using System.Collections.Generic;

public class EnemyStateMachine : IStateMachineBase<EnemyState>
{
    private Dictionary<EnemyStates, EnemyState> enemyStates;

    private EnemyState currentState;

    public EnemyState Current => currentState;

    public void ChangeState(EnemyState next)
    {
        currentState = next;
        TaskManager.GetInstance.AddTurnBehaviour(new TurnTask(currentState.Enter, currentState.duration));
        TaskManager.GetInstance.AddTurnBehaviour(new TurnTask(currentState.Exit, 0.1f));
        TaskManager.GetInstance.StartTask();
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

    public EnemyStateMachine(EnemyNPC enemyNPC, EnemyStates startType = EnemyStates.PatrolEnemyIdleRotationState)
    {
        enemyStates = new Dictionary<EnemyStates, EnemyState>();
        for (int i = 0; i < Enum.GetValues(typeof(EnemyStates)).Length; i++)
        {
            enemyStates.TryAdd((EnemyStates)i, EnemyState.Factory((EnemyStates)i,enemyNPC));
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

    public virtual void Enter()
    {

    }

    public virtual void Execute()
    {

    }

    public virtual void Exit()
    {

    }
    public static EnemyState Factory(EnemyStates turnTypes,EnemyNPC enemyNPC)
    {
        switch (turnTypes)
        {
            case EnemyStates.PatrolEnemyIdleRotationState:
                return new PatrolEnemyIdleRotationState(enemyNPC);
            case EnemyStates.PatrolEnemyChaseState:
                return new PatrolEnemyChaseState(enemyNPC);
            case EnemyStates.PatrolEnemyCombatState:
                return new PatrolEnemyCombatState(enemyNPC);
            case EnemyStates.PatrolEnemyDamagedState:
                return new PatrolEnemyDamagedState(enemyNPC);
            case EnemyStates.PatrolEnemyDeadState:
                return new PatrolEnemyDeadState(enemyNPC);
            case EnemyStates.PatrolEnemyInvestigateState:
                return new PatrolEnemyInvestigateState(enemyNPC);
            case EnemyStates.PatrolEnemyLookAroundState:
                return new PatrolEnemyLookAroundState(enemyNPC);
            case EnemyStates.PatrolEnemyPatrolState:
                return new PatrolEnemyPatrolState(enemyNPC);
            case EnemyStates.HoldEnemyChaseState:
                return new HoldEnemyChaseState(enemyNPC);
            case EnemyStates.HoldEnemyCombatState:
                return new HoldEnemyCombatState(enemyNPC);
            case EnemyStates.HoldEnemyDamagedState:
                return new HoldEnemyDamagedState(enemyNPC);
            case EnemyStates.HoldEnemyDeadState:
                return new HoldEnemyDeadState(enemyNPC);
            case EnemyStates.HoldEnemyIdleRotationState:
                return new HoldEnemyIdleRotationState(enemyNPC);
            case EnemyStates.HoldEnemyIdleState:
                return new HoldEnemyIdleState(enemyNPC);
            case EnemyStates.HoldEnemyInvestigateState:
                return new HoldEnemyInvestigateState(enemyNPC);
            case EnemyStates.HoldEnemyMoveReturnState:
                return new HoldEnemyMoveReturnState(enemyNPC);
            case EnemyStates.CopEnemyChaseState:
                return new CopEnemyChaseState(enemyNPC);
            case EnemyStates.CopEnemyCombatState:
                return new CopEnemyCombatState(enemyNPC);
            case EnemyStates.CopEnemyDamagedState:
                return new CopEnemyDamagedState(enemyNPC);
            case EnemyStates.CopEnemyDeadState:
                return new CopEnemyDeadState(enemyNPC);
            default:
                return null;
        }
    }
}
