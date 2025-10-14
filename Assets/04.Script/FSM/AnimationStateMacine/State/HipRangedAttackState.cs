using UnityEngine;

public class HipRangedAttackState : AnimationState
{
    private static readonly int isHipAttack = Animator.StringToHash("isHipAttack");
    Animator animator;

    public HipRangedAttackState(Animator animator)
    {
        this.animator = animator;
    }

    public override void Enter()
    {
        animator.SetBool(isHipAttack, true);
    }

    public override void Execute()
    {
        // Walk 동안 처리할 로직 (예: 입력 체크)
    }

    public override void Exit()
    {
        animator.SetBool(isHipAttack, false);
    }
}