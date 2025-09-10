using UnityEngine;

public class IdleState : AnimationState
{
    Animator animator;

    public IdleState(Animator animator)
    {
        this.animator = animator;
    }

    public override void Enter()
    {
        Debug.Log("Enter Idle");
        animator.Play("Idle");
    }

    public override void Execute()
    {
        // Idle 동안 처리할 로직 (예: 입력 체크)
    }

    public override void Exit()
    {
        Debug.Log("Exit Idle");
    }
}