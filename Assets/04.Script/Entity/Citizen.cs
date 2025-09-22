using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor.Build;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.AI;

public class Citizen : MonoBehaviour
{
    public EntityData entityData;
    PlayerStats playerStats;
    NeutralStateMachine nfsm;
    bool isDetection = false;
    bool isHit = false;
    int alertLevel = 1;

    private void Awake()
    {
        playerStats = new PlayerStats(entityData);
        nfsm = new NeutralStateMachine(NeutralStates.CitizenIdleState);
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
        if(isHit == true)
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
