using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PatrolEnemyPatrolState : EnemyState
{
    public Queue<Vector3> pos;
    public NavMeshAgent agent;
    public EnemyNPC patrolEnemy;

    public PatrolEnemyPatrolState(EnemyNPC patrolEnemy)
    {
        this.patrolEnemy = patrolEnemy;
        pos = new Queue<Vector3>();
    }

    public override void Enter()
    {
        if(pos.TryDequeue(out Vector3 current))
        {
            agent.SetDestination(current);
            Debug.Log("이동함?");
        }

        else
        {
            Debug.LogError("이동경로 음슴");
        }
    }

    public override void Execute()
    {
        
    }

    public override void Exit()
    {
        
    }
}
