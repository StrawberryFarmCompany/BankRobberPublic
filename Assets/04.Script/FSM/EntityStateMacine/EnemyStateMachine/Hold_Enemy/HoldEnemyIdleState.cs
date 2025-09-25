using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class HoldEnemyIdleState : EnemyState
{
    public EnemyNPC holdEnemy;

    public HoldEnemyIdleState(EnemyNPC holdEnemy)
    {
        this.holdEnemy = holdEnemy;
    }

    public override void Enter()
    {
        Debug.Log("대기중");
    }

    public override void Execute()
    {

    }

    public override void Exit()
    {

    }
}
