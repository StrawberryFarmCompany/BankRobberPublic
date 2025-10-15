using UnityEngine;

public class DamagedState : AnimationState
{
    private static readonly int damaged = Animator.StringToHash("Damaged");
    Animator animator;

    public DamagedState(Animator animator)
    {
        this.animator = animator;
    }

    public override void Enter()
    {
        animator.SetBool(AnimationStateController.isIdle, true);
        animator.Play(damaged);
    }

    public override void Execute()
    {
        // Damaged 동안 처리할 로직 (예: 입력 체크)
    }

    public override void Exit()
    {
        animator.SetBool(AnimationStateController.isIdle, false);
    }
}