using UnityEngine;

public class InteractionState : AnimationState
{
    //private static readonly int unEquip = Animator.StringToHash("UnEquip");
    private static readonly int isInteraction = Animator.StringToHash("isInteraction");
    Animator animator;

    public InteractionState(Animator animator)
    {
        this.animator = animator;
    }

    public override void Enter()
    {
        animator.SetBool(isInteraction, true);
        animator.Play(AnimationStateController.unEquip);
    }

    public override void Execute()
    {
        // Interaction 동안 처리할 로직 (예: 입력 체크)
    }

    public override void Exit()
    {
        animator.SetBool(isInteraction, false);
    }
}