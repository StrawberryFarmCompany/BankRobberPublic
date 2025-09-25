using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AI;

public class CitizenFleeState : NeutralState
{
    public NavMeshAgent agent;
    public Queue<Vector3> pos;

    public NeutralNPC citizen;
    public CitizenFleeState(NeutralNPC citizen)
    {
        this.citizen = citizen;
        pos = new Queue<Vector3>();
    }
    public override void Enter()
    {
        if (pos.TryDequeue(out Vector3 current))
        {
            agent.SetDestination(current);
        }
        else
        {
            Debug.LogError("이동 경로 없음");
        }
    }

    public override void Exit() 
    {

    }

}
