using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopEnemyDeadState : EnemyState
{
    public CopEnemyDeadState(EnemyNPC enemyNPC, Animator anim)
    {
        this.patrolEnemy = enemyNPC;
        this.anim = anim;
    }

    public override void Enter()
    {
        anim.Play("E_Dead_Fwd");
    }

}
