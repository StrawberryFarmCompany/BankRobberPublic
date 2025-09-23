using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopEnemyDeadState : EnemyState
{
    public EnemyNPC copEnemy;
    public CopEnemyDeadState(EnemyNPC copEnemy)
    {
        this.copEnemy = copEnemy;
    }

    public override void Enter()
    {

    }

}
