using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolEnemyCombatState : EnemyState
{
    public PatrolEnemyCombatState(EnemyNPC enemyNPC, Animator anim)
    {
        this.patrolEnemy = enemyNPC;
        this.anim = anim;
    }

    public override void Enter()
    {
        anim.Play("HG_HipAim_Enter");
    }

    public override void Execute()
    {

    }

    public override void Exit()
    {
        anim.Play("HG_Idle_Pose");
    }
}
