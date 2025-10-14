using UnityEngine;

public class IdleState : AnimationState
{
    private static readonly int isIdle = Animator.StringToHash("isIdle");
    Animator animator;

    public IdleState(Animator animator)
    {
        this.animator = animator;
    }

    public override void Enter()
    {
        animator.SetBool(isIdle, true);
    }

    public override void Execute()
    {
        // Idle 동안 처리할 로직 (예: 입력 체크)
    }

    public override void Exit()
    {
        animator.SetBool(isIdle, false);
    }
}