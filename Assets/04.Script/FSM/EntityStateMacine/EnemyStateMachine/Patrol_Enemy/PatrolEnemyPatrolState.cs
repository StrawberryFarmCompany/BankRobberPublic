using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PatrolEnemyPatrolState : EnemyState
{
    public NavMeshAgent agent;
    public Vector3 pos;
    public PatrolEnemy patrolEnemy;

    public override void Enter()
    {
        patrolEnemy.Patrol(pos);
    }

    public override void Execute()
    {
        
    }

    public override void Exit()
    {
        
    }
}
