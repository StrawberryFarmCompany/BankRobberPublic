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
        Debug.Log("Enter Melee");
        animator.Play("Melee");
    }

    public override void Execute()
    {

    }

    public override void Exit()
    {
        Debug.Log("Exit Melee");
    }
}