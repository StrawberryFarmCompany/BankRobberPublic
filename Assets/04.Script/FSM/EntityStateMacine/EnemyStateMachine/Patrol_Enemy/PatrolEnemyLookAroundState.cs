using UnityEngine;

public class PatrolEnemyLookAroundState : EnemyState
{
    public EnemyNPC patrolEnemy;

    public PatrolEnemyLookAroundState(EnemyNPC patrolEnemy)
    {
        this.patrolEnemy = patrolEnemy;
    }

    public override void Enter()
    {

    }

    public override void Execute()
    {

    }

    public override void Exit()
    {
        Debug.Log("두리번 종료");
    }
}
