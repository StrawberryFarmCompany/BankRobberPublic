using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopEnemy : MonoBehaviour
{
    public EntityData entityData;
    PlayerStats playerStats;
    EnemyStateMachine efsm;
    bool isDetection = false;
    bool isHit = false;

    private void Awake()
    {
        playerStats = new PlayerStats(entityData);
        efsm = new EnemyStateMachine(EnemyStates.CopEnemyChaseState);
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
