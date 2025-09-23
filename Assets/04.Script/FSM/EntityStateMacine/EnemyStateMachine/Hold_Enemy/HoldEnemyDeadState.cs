using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoldEnemyDeadState : EnemyState
{
    public EnemyNPC holdEnemy;

    public HoldEnemyDeadState(EnemyNPC holdEnemy)
    {
        this.holdEnemy = holdEnemy;
    }

    public override void Enter()
    {

    }

}
