using UnityEngine;

public class PatrolEnemyLookAroundState : EnemyState
{
    public PatrolEnemyLookAroundState(EnemyNPC enemyNPC, Animator anim)
    {
        this.patrolEnemy = enemyNPC;
        this.anim = anim;
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
