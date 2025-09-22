using UnityEngine;

public class CopEnemy : EnemyNPC
{
    bool isDetection = false;
    bool isHit = false;

    protected override void Awake()
    {
        base.Awake();
        efsm = new EnemyStateMachine(this, EnemyStates.CopEnemyChaseState);
    }

    public void ChangeToChase()
    {
        efsm.ChangeState(efsm.FindState(EnemyStates.CopEnemyChaseState));
    }

    public void ChangeToCombat()
    {
        efsm.ChangeState(efsm.FindState(EnemyStates.CopEnemyCombatState));
    }

    public void ChangeToDamaged()
    {
        efsm.ChangeState(efsm.FindState(EnemyStates.CopEnemyDamagedState));
    }

    public void ChangeToDead()
    {
        efsm.ChangeState(efsm.FindState(EnemyStates.CopEnemyDeadState));
    }
}
