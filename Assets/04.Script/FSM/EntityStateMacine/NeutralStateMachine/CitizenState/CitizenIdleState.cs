using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CitizenIdleState : NeutralState
{
    public CitizenIdleState(NeutralNPC citizen,Animator anim)
    {
        this.neutralNPC = citizen;
        this.anim = anim;
    }

    public override void Enter()
    {
        //idleState.Enter();
    }

    public override void Execute()
    {
        
    }

    public override void Exit() 
    {
        //idleState.Exit();
    }

    public void Idle()
    {

    }
}
