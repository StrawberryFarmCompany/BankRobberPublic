using UnityEngine;

public class SquatState : AnimationState
{
    Animator animator;

    public SquatState(Animator animator)
    {
        this.animator = animator;
    }

    public override void Enter()
    {
        Debug.Log("Enter Squat");
        animator.Play("Squat");
    }

    public override void Execute()
    {
        // Squat 동안 처리할 로직 (예: 입력 체크)
    }

    public override void Exit()
    {
        Debug.Log("Exit Squat");
    }
}