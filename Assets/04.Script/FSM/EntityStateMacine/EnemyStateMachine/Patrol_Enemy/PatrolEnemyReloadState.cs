using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PatrolEnemyReloadState : EnemyState
{

    public PatrolEnemyReloadState(EnemyNPC enemyNPC, Animator anim)
    {
        this.patrolEnemy = enemyNPC;
        this.anim = anim;
    }

public override void Enter()
    {
        anim.Play("HG_Reload");
    }

    public override void Execute()
    {

    }

    public override void Exit()
    {
        anim.Play("HG_Idle_Pose");
    }
}
