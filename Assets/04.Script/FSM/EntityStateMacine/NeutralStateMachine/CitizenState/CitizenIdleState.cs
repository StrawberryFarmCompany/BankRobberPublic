using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CitizenIdleState : NeutralState
{
    IdleState idleState;

    public NeutralNPC citizen;
    public CitizenIdleState(NeutralNPC citizen)
    {
        this.citizen = citizen;
    }

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

    public void Idle()
    {

    }
}
