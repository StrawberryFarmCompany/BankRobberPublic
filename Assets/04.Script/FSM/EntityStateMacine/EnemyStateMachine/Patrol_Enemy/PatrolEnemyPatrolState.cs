using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PatrolEnemyPatrolState : EnemyState
{
    public NavMeshAgent agent;
    public Vector3 pos;

    public EnemyNPC patrolEnemy;

    public PatrolEnemyPatrolState(EnemyNPC patrolEnemy)
    {
        this.patrolEnemy = patrolEnemy;
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
