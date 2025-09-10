using UnityEngine;

public class InteractionState : AnimationState
{
    Animator animator;

    public InteractionState(Animator animator)
    {
        this.animator = animator;
    }

    public override void Enter()
    {
        Debug.Log("Enter Interaction");
        animator.Play("Interaction");
    }

    public override void Execute()
    {
        // Interaction 동안 처리할 로직 (예: 입력 체크)
    }

    public override void Exit()
    {
        Debug.Log("Exit Interaction");
    }
}