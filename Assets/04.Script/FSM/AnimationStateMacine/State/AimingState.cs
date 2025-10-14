using UnityEngine;

public class AimingState : AnimationState
{
    private static readonly int isAiming = Animator.StringToHash("isAiming");
    private static readonly int isIdle = Animator.StringToHash("isIdle");
    Animator animator;

    public AimingState(Animator animator)
    {
        this.animator = animator;
    }

    public override void Enter()
    {
        animator.SetBool(isIdle, true);
        animator.SetBool(isAiming, true);
    }

    public override void Execute()
    {
        // Walk 동안 처리할 로직 (예: 입력 체크)
    }

    public override void Exit()
    {
        animator.SetBool(isIdle, false);
    }
}