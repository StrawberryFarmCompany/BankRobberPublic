using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CitizenIdleState : EntityState
{
    IdleState idleState;
    public override void Enter()
    {
        idleState.Enter();
    }

    public override void Execute()
    {
        
    }

    public override void Exit() 
    {
        idleState.Exit();
    }

}
