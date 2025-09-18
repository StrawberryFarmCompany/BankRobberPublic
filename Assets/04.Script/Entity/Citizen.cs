using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Citizen : MonoBehaviour
{
    private NeutralStateMachine neutralStateMachine;
    private CitizenIdleState citizenIdleState;
    private CitizenCowerState citizenCowerState;
    private CitizenDeadState citizenDeadState;
    private CitizenFleeState citizenFleeState;
    public EntityData entityData;
    private void Awake()
    {
        neutralStateMachine = new NeutralStateMachine();
        citizenIdleState = new CitizenIdleState();
        citizenCowerState = new CitizenCowerState();
        citizenDeadState = new CitizenDeadState();
        citizenFleeState = new CitizenFleeState();
        entityData = new EntityData();

        neutralStateMachine.ForceSet(citizenIdleState);
    }

    private void Update()
    {
        if(entityData.curHp <= 0)
        {
            ChangeToDead();
        }
    }
    public void ChangeToIdle()
    {
        neutralStateMachine.ChangeState(citizenIdleState);
    }

    public void ChangeToCowerState()
    {
        neutralStateMachine.ChangeState(citizenCowerState);
    }

    public void ChangeToDead()
    {
        neutralStateMachine.ChangeState(citizenDeadState);
    }

    public void ChangeToFlee()
    {
        neutralStateMachine.ChangeState(citizenFleeState);
    }


}
