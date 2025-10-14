using UnityEngine;

public class StrafeState : AnimationState
{
    private static readonly int isStrafe = Animator.StringToHash("isStrafe");
    Animator animator;

    public StrafeState(Animator animator)
    {
        this.animator = animator;
    }

    public override void Enter()
    {
        animator.SetBool(isStrafe, true);
    }

    public override void Execute()
    {
        // Walk 동안 처리할 로직 (예: 입력 체크)
    }

    public override void Exit()
    {
        animator.SetBool(isStrafe, false);
    }
}