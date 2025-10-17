using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class HoldEnemyMoveReturnState : EnemyState
{
    public NavMeshAgent agent;
    public Queue<Vector3> pos;
    
    public HoldEnemyMoveReturnState(EnemyNPC enemyNPC, Animator anim)
    {
        pos = new Queue<Vector3>();
        this.patrolEnemy = enemyNPC;
        this.anim = anim;
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
