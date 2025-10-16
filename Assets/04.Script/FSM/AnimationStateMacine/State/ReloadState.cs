using UnityEngine;

public class ReloadState : AnimationState
{
    private static readonly int ar_Reload = Animator.StringToHash("AR_Reload");
    private static readonly int hg_Reload = Animator.StringToHash("HG_Reload");
    Animator animator;
    Gun gun;

    public ReloadState(Animator animator, Gun gun)
    {
        this.animator = animator;
        this.gun = gun;
    }

    public override void Enter()
    {
        animator.SetBool(AnimationStateController.isIdle, true);
        if (gun != null)
        {
            if (gun.type != GunType.HandGun)
            {
                animator.Play(ar_Reload);
            }
            else
            {
                animator.Play(hg_Reload);
            }
        }
    }
    public override void Execute()
    {
        // Walk 동안 처리할 로직 (예: 입력 체크)
    }

    public override void Exit()
    {
        animator.SetBool(AnimationStateController.isIdle, false);

    }
}