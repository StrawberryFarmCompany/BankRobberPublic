using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CitizenDeadState : NeutralState
{

    public NeutralNPC citizen;
    public CitizenDeadState(NeutralNPC citizen)
    {
        this.citizen = citizen;
    }
    public override void Enter()
    {
        base.Enter();
    }

}
