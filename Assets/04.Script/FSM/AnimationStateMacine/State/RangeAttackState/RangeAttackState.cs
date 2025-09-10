using UnityEngine;

public class RangeAttackState : AnimationState
{
    Animator animator;

    public RangeAttackState(Animator animator)
    {
        this.animator = animator;
    }

    public override void Enter()
    {
        Debug.Log("Enter RangeAttack");
        animator.Play("RangeAttack");
    }

    public override void Execute()
    {
        // RangeAttack 동안 처리할 로직 (예: 입력 체크)
    }

    public override void Exit()
    {
        Debug.Log("Exit RangeAttack");
    }
}