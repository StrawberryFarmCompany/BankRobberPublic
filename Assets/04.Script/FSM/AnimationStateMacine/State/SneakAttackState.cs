using UnityEngine;

public class SneakState : AnimationState
{
    private static readonly int SneakAttack = Animator.StringToHash("SneakAttack");
    Animator animator;

    public SneakState(Animator animator)
    {
        this.animator = animator;
    }

    public override void Enter()
    {
        animator.Play(SneakAttack);
    }

    public override void Execute()
    {
        // Melee 동안 처리할 로직 (예: 입력 체크)
    }

    public override void Exit()
    {

    }
}