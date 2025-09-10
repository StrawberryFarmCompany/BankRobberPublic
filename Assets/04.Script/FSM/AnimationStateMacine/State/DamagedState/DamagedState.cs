using UnityEngine;

public class DamagedState : AnimationState
{
    Animator animator;

    public DamagedState(Animator animator)
    {
        this.animator = animator;
    }

    public override void Enter()
    {
        Debug.Log("Enter Damaged");
        animator.Play("Damaged");
    }

    public override void Execute()
    {
        // Damaged 동안 처리할 로직 (예: 입력 체크)
    }

    public override void Exit()
    {
        Debug.Log("Exit Damaged");
    }
}