using UnityEngine;

public class AimRangedAttackState : AnimationState
{
    private static readonly int ar_Aim_Shooting = Animator.StringToHash("AR_Aim_Shooting");
    private static readonly int hg_Aim_Shooting = Animator.StringToHash("HG_Aim_Shooting");
    Animator animator;
    Gun gun;


    public AimRangedAttackState(Animator animator, Gun gun)
    {
        this.animator = animator;
        this.gun = gun;
    }

    public override void Enter()
    {
        if(gun.type != GunType.HandGun)
        {
            animator.Play(ar_Aim_Shooting);
        }
        else
        {
            animator.Play(hg_Aim_Shooting);
        }
    }

    public override void Execute()
    {
        // RangeAttack 동안 처리할 로직 (예: 입력 체크)
    }

    public override void Exit()
    {

    }
}