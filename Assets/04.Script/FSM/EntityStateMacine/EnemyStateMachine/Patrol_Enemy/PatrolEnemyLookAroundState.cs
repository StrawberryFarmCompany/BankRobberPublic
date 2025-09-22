using UnityEngine;

public class PatrolEnemyLookAroundState : EnemyState
{
    public PatrolEnemy patrolEnemy;

    public override void Enter()
    {
        patrolEnemy.LookAround();
    }

    public override void Execute()
    {

    }

    public override void Exit()
    {
        Debug.Log("두리번 종료");
    }
}
