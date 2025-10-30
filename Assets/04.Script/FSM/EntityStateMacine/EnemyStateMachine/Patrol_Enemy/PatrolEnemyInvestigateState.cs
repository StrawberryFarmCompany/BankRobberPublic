using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PatrolEnemyInvestigateState : EnemyState
{

    public PatrolEnemyInvestigateState(EnemyNPC enemyNPC, Animator anim)
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
