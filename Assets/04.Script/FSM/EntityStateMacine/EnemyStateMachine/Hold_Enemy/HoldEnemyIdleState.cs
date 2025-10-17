using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class HoldEnemyIdleState : EnemyState
{
    public HoldEnemyIdleState(EnemyNPC enemyNPC, Animator anim)
    {
        this.patrolEnemy = enemyNPC;
        this.anim = anim;
    }

    public override void Enter()
    {
        Debug.Log("대기중");
    }

    public override void Execute()
    {

    }

    public override void Exit()
    {

    }
}
