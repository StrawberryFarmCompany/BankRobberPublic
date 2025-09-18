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
        currentState?.Exit();
        currentState = next;
        currentState.Enter();
    }

    public void ForceSet(EnemyState next)
    {
        currentState = next;
        currentState.Enter();
    }
    public EnemyState FindState(EnemyStates statesType)
    {
        new EnemyStateMachine(EnemyStates.PatrolEnemyIdleRotationState);
        return enemyStates[statesType];
    }

    public EnemyStateMachine(EnemyStates startType = EnemyStates.PatrolEnemyIdleRotationState)
    {
        enemyStates = new Dictionary<EnemyStates, EnemyState>();
        for (int i = 0; i < Enum.GetValues(typeof(EnemyStates)).Length; i++)
        {
            enemyStates.TryAdd((EnemyStates)i, EnemyState.Factory((EnemyStates)i));
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
    public static EnemyState Factory(EnemyStates turnTypes)
    {
        switch (turnTypes)
        {
            case EnemyStates.PatrolEnemyIdleRotationState:
                return new PatrolEnemyIdleRotationState();
            case EnemyStates.PatrolEnemyChaseState:
                return new PatrolEnemyChaseState();
            case EnemyStates.PatrolEnemyCombatState:
                return new PatrolEnemyCombatState();
            case EnemyStates.PatrolEnemyDamagedState:
                return new PatrolEnemyDamagedState();
            case EnemyStates.PatrolEnemyDeadState:
                return new PatrolEnemyDeadState();
            case EnemyStates.PatrolEnemyInvestigateState:
                return new PatrolEnemyInvestigateState();
            case EnemyStates.PatrolEnemyLookAroundState:
                return new PatrolEnemyLookAroundState();
            case EnemyStates.PatrolEnemyPatrolState:
                return new PatrolEnemyPatrolState();
            case EnemyStates.HoldEnemyChaseState:
                return new HoldEnemyChaseState();
            case EnemyStates.HoldEnemyCombatState:
                return new HoldEnemyCombatState();
            case EnemyStates.HoldEnemyDamagedState:
                return new HoldEnemyDamagedState();
            case EnemyStates.HoldEnemyDeadState:
                return new HoldEnemyDeadState();
            case EnemyStates.HoldEnemyIdleRotationState:
                return new HoldEnemyIdleRotationState();
            case EnemyStates.HoldEnemyIdleState:
                return new HoldEnemyIdleState();
            case EnemyStates.HoldEnemyInvestigateState:
                return new HoldEnemyInvestigateState();
            case EnemyStates.HoldEnemyMoveReturnState:
                return new HoldEnemyMoveReturnState();
            case EnemyStates.CopEnemyChaseState:
                return new CopEnemyChaseState();
            case EnemyStates.CopEnemyCombatState:
                return new CopEnemyCombatState();
            case EnemyStates.CopEnemyDamagedState:
                return new CopEnemyDamagedState();
            case EnemyStates.CopEnemyDeadState:
                return new CopEnemyDeadState();
            default:
                return null;
        }
    }
}
