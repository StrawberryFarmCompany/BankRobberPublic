using UnityEngine;

public class HideState : AnimationState
{
    private static readonly int hide = Animator.StringToHash("Hide");
    Animator animator;

    public HideState(Animator animator)
    {
        this.animator = animator;
    }

    public override void Enter()
    {
        animator.SetBool(AnimationStateController.isIdle, true);
        animator.Play(hide);
    }

    public override void Execute()
    {
        // Walk 동안 처리할 로직 (예: 입력 체크)
    }

    public override void Exit()
    {
        animator.SetBool(AnimationStateController.isIdle, false);
    }
}