using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor.Build;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.AI;

public class CitizenNPC : NeutralNPC
{
    bool isDetection = false;
    bool isHit = false;
    int alertLevel = 1;

    protected override void Awake()
    {
        base.Awake();
        nfsm = new NeutralStateMachine(this, NeutralStates.CitizenIdleState);
    }

    protected override void Update()
    {

    }
    public void ChangeToIdle()
    {

    }

    public void ChangeToCowerState()
    {
        nfsm.ChangeState(nfsm.FindState(NeutralStates.CitizenCowerState));
    }

    public void ChangeToDead()
    {
        nfsm.ChangeState(nfsm.FindState(NeutralStates.CitizenDeadState));
    }

    public void ChangeToFlee(Vector3 pos)
    {
        CitizenFleeState fleeState = (CitizenFleeState)nfsm.FindState(NeutralStates.CitizenFleeState);
        fleeState.agent = gameObject.GetComponent<NavMeshAgent>();
        fleeState.pos = pos;
        nfsm.ChangeState(nfsm.FindState(NeutralStates.CitizenFleeState));
    }

    public void CitizenBehaviour()
    {
        if (isHit == true)
        {
            ChangeToDead();
        }
        else if (alertLevel >= 3)//경계수준이 3레벨 이상이면
        {
            ChangeToCowerState();
        }
        else if (isDetection == true)
        {
            //ChangeToFlee(//도망갈 위치?);
        }
    }
}
