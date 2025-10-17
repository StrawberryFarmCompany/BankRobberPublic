using UnityEngine;

public class HealState : AnimationState
{
    private static readonly int heal = Animator.StringToHash("Heal");
    Animator animator;

    public HealState(Animator animator)
    {
        this.animator = animator;
    }

    public override void Enter()
    {
        animator.Play(heal);
    }

    public override void Execute()
    {
        // Walk 동안 처리할 로직 (예: 입력 체크)
    }

    public override void Exit()
    {
    }
}