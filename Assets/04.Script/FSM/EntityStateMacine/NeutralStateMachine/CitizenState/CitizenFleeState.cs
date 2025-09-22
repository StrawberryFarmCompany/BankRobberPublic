using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AI;

public class CitizenFleeState : NeutralState
{
    public NavMeshAgent agent;
    public Vector3 pos;

    public NeutralNPC citizen;
    public CitizenFleeState(NeutralNPC citizen)
    {
        this.citizen = citizen;
    }
    public override void Enter()
    {
        agent.SetDestination(pos);
    }

    public override void Exit() 
    {

    }

}
