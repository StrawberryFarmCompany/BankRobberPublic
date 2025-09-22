using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AI;

public class Citizen : MonoBehaviour
{
    private NeutralStateMachine neutralStateMachine;
    PlayerStats playerStats;

    private void Awake()
    {
        //neutralStateMachine = new NeutralStateMachine();
    }

    private void Update()
    {

    }
    public void ChangeToIdle()
    {

    }

    public void ChangeToCowerState()
    {

    }

    public void ChangeToDead()
    {
        if(playerStats.curHp <= 0)
        {

        }
    }

    public void ChangeToFlee()
    {

    }

    public void MoveOrder(Vector3 pos)
    {
        CitizenFleeState fleeState = (CitizenFleeState)neutralStateMachine.FindState(NeutralStates.CitizenFleeState);
        fleeState.agent = gameObject.GetComponent<NavMeshAgent>();
        fleeState.pos = pos;
        neutralStateMachine.ChangeState(neutralStateMachine.FindState(NeutralStates.CitizenFleeState));
    }
}
