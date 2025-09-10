using UnityEngine;

public class DeadState : AnimationState
{
    Animator animator;

    public DeadState(Animator animator)
    {
        this.animator = animator;
    }

    public override void Enter()
    {
        Debug.Log("Enter Dead");
        animator.Play("Dead");
    }

    public override void Execute()
    {
        // Dead 동안 처리할 로직 (예: 입력 체크)
    }

    public override void Exit()
    {
        Debug.Log("Exit Dead");
    }
}