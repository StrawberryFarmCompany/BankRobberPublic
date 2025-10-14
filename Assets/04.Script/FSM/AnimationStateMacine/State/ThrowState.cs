using UnityEngine;

public class ThrowState : AnimationState
{
    private static readonly int isThrow = Animator.StringToHash("isThrow");
    //public static readonly int unEquip = Animator.StringToHash("UnEquip");
    Animator animator;

    public ThrowState(Animator animator)
    {
        this.animator = animator;
    }

    public override void Enter()
    {
        animator.SetBool(isThrow, true);
        animator.Play(AnimationStateController.unEquip);
    }

    public override void Execute()
    {
        // Walk 동안 처리할 로직 (예: 입력 체크)
    }

    public override void Exit()
    {
        animator.SetBool(isThrow, false);
    }
}