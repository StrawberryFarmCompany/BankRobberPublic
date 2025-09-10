using UnityEngine;

public class JumpState : AnimationState
{
    Animator animator;

    public JumpState(Animator animator)
    {
        this.animator = animator;
    }

    public override void Enter()
    {
        Debug.Log("Enter Jump");
        animator.Play("Jump");
    }

    public override void Execute()
    {
        // Jump 동안 처리할 로직 (예: 입력 체크)
    }

    public override void Exit()
    {
        Debug.Log("Exit Jump");
    }
}