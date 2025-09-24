using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static UnityEditor.PlayerSettings;

public class PatrolEnemyChaseState : EnemyState
{
    public EnemyNPC patrolEnemy;
    public NavMeshAgent agent;
    public Queue<Vector3> pos;

    public PatrolEnemyChaseState(EnemyNPC patrolEnemy)
    {
        this.patrolEnemy = patrolEnemy;
        pos = new Queue<Vector3>();
    }

    public override void Enter()
    {
        if (pos.TryDequeue(out Vector3 current))
        {
            agent.SetDestination(current);
        }

        else
        {
            Debug.LogError("이동 경로 없음");
        }
    }

    public override void Execute()
    {

    }

    public override void Exit()
    {

    }
}
