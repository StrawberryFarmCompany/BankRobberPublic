using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CitizenCowerState : NeutralState
{
    public NeutralNPC citizen;

    public CitizenCowerState(NeutralNPC citizen)
    {
        this.citizen = citizen;
    }
    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit() 
    {
        base.Exit();
    }
}
