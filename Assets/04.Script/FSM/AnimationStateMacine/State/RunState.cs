using UnityEngine;

public class RunState : AnimationState
{
    private static readonly int Run = Animator.StringToHash("Run");
    //private static readonly int unEquip = Animator.StringToHash("UnEquip");
    Animator animator;

    public RunState(Animator animator)
    {
        this.animator = animator;
    }

    public override void Enter()
    {
        animator.SetBool(AnimationStateController.isIdle, true);
        animator.Play(Run);
        animator.Play(AnimationStateController.unEquip);
    }

    public override void Execute()
    {
        // Run 동안 처리할 로직 (예: 입력 체크)
    }

    public override void Exit()
    {
        animator.SetBool(AnimationStateController.isIdle, false);
    }
}