using UnityEngine;

public class InteractionState : AnimationState
{
    private static readonly int Interact = Animator.StringToHash("Interact");
    Animator animator;

    public InteractionState(Animator animator)
    {
        this.animator = animator;
    }

    public override void Enter()
    {
        animator.Play(Interact);
    }

    public override void Execute()
    {
        // Interaction 동안 처리할 로직 (예: 입력 체크)
    }

    public override void Exit()
    {
    }
}