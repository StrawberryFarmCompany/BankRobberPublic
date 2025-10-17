using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolEnemyDeadState : EnemyState
{
    public PatrolEnemyDeadState(EnemyNPC enemyNPC, Animator anim)
    {
        this.patrolEnemy = enemyNPC;
        this.anim = anim;
    }

    public override void Enter()
    {

    }

    public override void Execute()
    {

    }

    public override void Exit()
    {

    }
}
