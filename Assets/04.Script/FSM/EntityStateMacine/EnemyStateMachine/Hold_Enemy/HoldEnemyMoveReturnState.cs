using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class HoldEnemyMoveReturnState : EnemyState
{
    public EnemyNPC holdEnemy;
    public NavMeshAgent agent;
    public Vector3 pos;
    
    public HoldEnemyMoveReturnState(EnemyNPC holdEnemy)
    {
        this.holdEnemy = holdEnemy;
    }

    public override void Enter()
    {
        agent.SetDestination(pos);
    }

    public override void Execute()
    {

    }

    public override void Exit()
    {

    }
}
