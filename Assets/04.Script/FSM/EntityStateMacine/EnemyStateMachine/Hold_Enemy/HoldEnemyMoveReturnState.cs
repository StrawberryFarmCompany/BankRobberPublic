using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class HoldEnemyMoveReturnState : EnemyState
{
    public EnemyNPC holdEnemy;
    public HoldEnemyMoveReturnState(EnemyNPC holdEnemy)
    {
        this.holdEnemy = holdEnemy;
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
