using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static UnityEditor.PlayerSettings;

public class HoldEnemyChaseState : EnemyState
{
    public EnemyNPC holdEnemy;
    public NavMeshAgent agent;
    public Vector3 pos;
    public HoldEnemyChaseState(EnemyNPC holdEnemy)
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
