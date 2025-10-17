using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoldEnemyDeadState : EnemyState
{
    public HoldEnemyDeadState(EnemyNPC enemyNPC, Animator anim)
    {
        this.patrolEnemy = enemyNPC;
        this.anim = anim;
    }

    public override void Enter()
    {

    }

}
