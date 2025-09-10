using UnityEngine;

public class FallState : AnimationState
{
    Animator animator;

    public FallState(Animator animator)
    {
        this.animator = animator;
    }

    public override void Enter()
    {
        Debug.Log("Enter Fall");
        animator.Play("Fall");
    }

    public override void Execute()
    {
        // Fall 동안 처리할 로직 (예: 입력 체크)
    }

    public override void Exit()
    {
        Debug.Log("Exit Fall");
    }
}