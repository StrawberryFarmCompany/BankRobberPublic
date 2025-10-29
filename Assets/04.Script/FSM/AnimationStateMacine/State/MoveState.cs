using UnityEngine;

public class MoveState : AnimationState
{
    Animator animator;
    NodePlayerController controller;
    Gun gun;

    private readonly int hg_Move = Animator.StringToHash("HG_Move");
    private readonly int hg_Strafe = Animator.StringToHash("HG_Strafe");
    private readonly int ar_Move = Animator.StringToHash("AR_Move");
    private readonly int ar_Strafe = Animator.StringToHash("AR_Strafe");

    public MoveState(Animator animator, NodePlayerController controller = null)
    {
        this.animator = animator;
        this.controller = controller;
        this.gun = controller.gun;
    }

    public override void Enter()
    {
        animator.SetBool(AnimationStateController.isIdle, false);
        if(controller != null && controller.isHide)
        {
            if (gun.type != GunType.HandGun) 
            {
                animator.Play(ar_Strafe);
            }
            else
            {
                animator.Play(hg_Strafe);
            }
        }
        else
        {
            if (gun.type != GunType.HandGun)
            {
                animator.Play(ar_Move);
            }
            else
            {
                animator.Play(hg_Move);
            }
        }
    }

    public override void Execute()
    {
        // Walk 동안 처리할 로직 (예: 입력 체크)
    }

    public override void Exit()
    {
    }
}