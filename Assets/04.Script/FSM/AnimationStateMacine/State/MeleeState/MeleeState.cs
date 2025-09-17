using UnityEngine;

public class MeleeState : AnimationState
{
    Animator animator;

    public MeleeState(Animator animator)
    {
        this.animator = animator;
    }

    public override void Enter()
    {
        base.Enter();
        Debug.Log("Enter Melee");
        animator.Play("Melee");
    }

    public override void Execute()
    {
        // Melee 동안 처리할 로직 (예: 입력 체크)
        if (IsDone) return;

        var info = animator.GetCurrentAnimatorStateInfo(0);

        if (info.normalizedTime >= 1f)
        {
            IsDone = true;
        }
    }

    public override void Exit()
    {
        Debug.Log("Exit Melee");
    }
}