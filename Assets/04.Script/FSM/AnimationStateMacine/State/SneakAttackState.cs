using UnityEngine;

public class SneakState : AnimationState
{
    private static readonly int isSneak = Animator.StringToHash("isSneakAttack");
    //private static readonly int unEquip = Animator.StringToHash("UnEquip");
    Animator animator;

    public SneakState(Animator animator)
    {
        this.animator = animator;
    }

    public override void Enter()
    {
        animator.SetBool(isSneak, true);
        animator.Play(AnimationStateController.unEquip);
    }

    public override void Execute()
    {
        // Melee 동안 처리할 로직 (예: 입력 체크)
    }

    public override void Exit()
    {
        animator.SetBool(isSneak, false);
    }
}