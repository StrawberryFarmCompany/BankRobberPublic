using IStateMachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCStateMachine : IStateMachineBase<NPCState>
{
    private NPCState currentState;
    public void ChangeState(NPCState next)
    {
        
    }

    public void ForceSet(NPCState next)
    {
        
    }
}

public class NPCState : IStateBase
{
    public void Enter()
    {
        
    }

    public void Execute()
    {
        
    }

    public void Exit()
    {
        
    }
}
