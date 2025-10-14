using UnityEngine;

public class HipRangedAttackState : AnimationState
{
    private static readonly int isHipAttack = Animator.StringToHash("isHipAttack");
    private static readonly int ar_Hip_Attack = Animator.StringToHash("AR_HipAim_Enter");
    private static readonly int hg_Hip_Attack = Animator.StringToHash("HG_HipAim_Enter");
    Animator animator;
    Gun gun;

    public HipRangedAttackState(Animator animator, Gun gun)
    {
        this.animator = animator;
        this.gun = gun;
    }

    public override void Enter()
    {
        //animator.SetBool(isHipAttack, true);
        if (gun.type != GunType.HandGun)
        {
            animator.Play(ar_Hip_Attack);
        }
        else
        {
            animator.Play(hg_Hip_Attack);
        }
    }

    public override void Execute()
    {
        // Walk 동안 처리할 로직 (예: 입력 체크)
    }

    public override void Exit()
    {
        //animator.SetBool(isHipAttack, false);
    }
}