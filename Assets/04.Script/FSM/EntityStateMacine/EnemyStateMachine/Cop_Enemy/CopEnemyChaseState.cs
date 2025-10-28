using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopEnemyChaseState : EnemyState
{
    public CopEnemyChaseState(EnemyNPC enemyNPC, Animator anim)
    {
        this.patrolEnemy = enemyNPC;
        this.anim = anim;
    }

    public override void Enter()
    {
        anim.Play("HG_Move");
    }

    public override void Execute()
    {

    }

    public override void Exit()
    {
        anim.Play("HG_Idle_Pose");
    }
}
