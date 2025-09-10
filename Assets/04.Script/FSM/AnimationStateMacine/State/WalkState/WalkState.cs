using UnityEngine;

public class WalkState : AnimationState
{
    Animator animator;

    public WalkState(Animator animator)
    {
        this.animator = animator;
    }

    public override void Enter()
    {
        Debug.Log("Enter Walk");
        animator.Play("Walk");
    }

    public override void Execute()
    {
        // Walk 동안 처리할 로직 (예: 입력 체크)
    }

    public override void Exit()
    {
        Debug.Log("Exit Walk");
    }
}