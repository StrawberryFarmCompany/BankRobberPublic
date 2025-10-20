using UnityEngine;

public class ReadyState : AnimationState
{
    private static readonly int ready = Animator.StringToHash("Ready");
    Animator animator;

    public ReadyState(Animator animator)
    {
        this.animator = animator;
    }

    public override void Enter()
    {
        animator.Play(ready);
    }

    public override void Execute()
    {
        // Walk 동안 처리할 로직 (예: 입력 체크)
    }

    public override void Exit()
    {
    }
}