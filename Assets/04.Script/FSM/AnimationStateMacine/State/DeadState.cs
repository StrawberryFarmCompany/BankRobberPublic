using UnityEngine;

public class DeadState : AnimationState
{
    private static readonly int dead_Bwd = Animator.StringToHash("Dead_Bwd");
    private static readonly int dead_Fwd = Animator.StringToHash("Dead_Fwd");
    Animator animator;

    public DeadState(Animator animator)
    {
        this.animator = animator;
    }

    public override void Enter()
    {
        // 랜덤
        if (Random.value > 0.5f)
        {
            animator.Play(dead_Fwd);
        }
        else
        {
            animator.Play(dead_Bwd);
        }
    }

    public override void Execute()
    {
        // Dead 동안 처리할 로직 (예: 입력 체크)
    }

    public override void Exit()
    {

    }
}