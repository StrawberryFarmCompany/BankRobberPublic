using UnityEngine;

public class RunState : AnimationState
{
    Animator animator;

    public RunState(Animator animator)
    {
        this.animator = animator;
    }

    public override void Enter()
    {
        Debug.Log("Enter Run");
        animator.Play("Run");
    }

    public override void Execute()
    {
        // Run 동안 처리할 로직 (예: 입력 체크)
    }

    public override void Exit()
    {
        Debug.Log("Exit Run");
    }
}