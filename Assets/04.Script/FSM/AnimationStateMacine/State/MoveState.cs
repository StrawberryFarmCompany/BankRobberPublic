using UnityEngine;

public class MoveState : AnimationState
{
    Animator animator;
    NodePlayerController controller;

    private readonly int isMove = Animator.StringToHash("isMove");
    private readonly int isStrafe = Animator.StringToHash("isStrafe");

    public MoveState(Animator animator, NodePlayerController controller = null)
    {
        this.animator = animator;
        this.controller = controller;
    }

    public override void Enter()
    {
        if(controller != null && controller.isHide)
        {
            animator.SetBool(isStrafe, true);
        }
        else
        {
            animator.SetBool(isMove, true);
        }
    }

    public override void Execute()
    {
        // Walk 동안 처리할 로직 (예: 입력 체크)
    }

    public override void Exit()
    {
        animator.SetBool(isMove, false);
        animator.SetBool(isStrafe, false);
    }
}