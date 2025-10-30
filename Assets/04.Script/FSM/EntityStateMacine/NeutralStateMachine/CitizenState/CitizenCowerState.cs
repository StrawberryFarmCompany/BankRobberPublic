using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CitizenCowerState : NeutralState
{

    public CitizenCowerState(NeutralNPC citizen, Animator anim)
    {
        this.neutralNPC = citizen;
        this.anim = anim;
    }
    public override void Enter()
    {
        anim.Play("Ducking_Idle");
    }

    public override void Exit() 
    {

    }
}
