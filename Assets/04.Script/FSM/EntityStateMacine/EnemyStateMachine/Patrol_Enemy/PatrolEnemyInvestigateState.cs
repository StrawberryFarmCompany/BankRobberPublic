using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PatrolEnemyInvestigateState : EnemyState
{
    public EnemyNPC patrolEnemy;
    public Vector3 pos;
    public NavMeshAgent agent;

    public PatrolEnemyInvestigateState(EnemyNPC patrolEnemy)
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
