using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AI;

public class CitizenFleeState : NeutralState
{
    public CitizenFleeState(NeutralNPC citizen, Animator anim)
    {
        this.neutralNPC = citizen;
        this.anim = anim;
    }
    public override void Enter()
    {

    }

    public override void Exit() 
    {

    }

}
