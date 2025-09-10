using UnityEngine;
using IStateMachine;

public class AnimationStateMachine : IStateMachineBase<AnimationState>
{
    private AnimationState currentState;

    public void ChangeState(AnimationState next)
    {
        if (currentState != null)
            currentState.Exit();

        currentState = next;

        if (currentState != null)
            currentState.Enter();
    }

    public void ForceSet(AnimationState next)
    {
        currentState = next;
        if (currentState != null)
            currentState.Enter();
    }

    public void Update()
    {
        if (currentState != null)
            currentState.Execute();
    }
}


public class AnimationState : IStateBase
{
    public virtual void Enter()
    {

    }

    public virtual void Execute()
    {

    }

    public virtual void Exit()
    {

    }
}