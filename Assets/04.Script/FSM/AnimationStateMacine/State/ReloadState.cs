using UnityEngine;

public class ReloadState : AnimationState
{
    private static readonly int isReload = Animator.StringToHash("isReload");
    Animator animator;

    public ReloadState(Animator animator)
    {
        this.animator = animator;
    }

    public override void Enter()
    {
        animator.SetBool(isReload, true);
    }

    public override void Execute()
    {
        // Walk 동안 처리할 로직 (예: 입력 체크)
    }

    public override void Exit()
    {
        animator.SetBool(isReload, false);
    }
}