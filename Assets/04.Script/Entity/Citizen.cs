using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Citizen : MonoBehaviour
{
    private EntityStateMachine entityStateMachine;
    private CitizenIdleState citizenIdleState;
    private CitizenCowerState citizenCowerState;
    private CitizenDeadState citizenDeadState;
    private CitizenFleeState citizenFleeState;
    public EntityData entityData;

    private void Awake()
    {
        entityStateMachine = new EntityStateMachine();
        citizenIdleState = new CitizenIdleState();
        citizenCowerState = new CitizenCowerState();
        citizenDeadState = new CitizenDeadState();
        citizenFleeState = new CitizenFleeState();
        entityData = new EntityData();

        entityStateMachine.ForceSet(citizenIdleState);
    }

    private void Update()
    {
        
    }
    public void ChangeToIdle()
    {
        entityStateMachine.ChangeState(citizenIdleState);
    }

    public void ChangeToCowerState()
    {
        entityStateMachine.ChangeState(citizenCowerState);
    }

    public void ChangeToDead()
    {
        entityStateMachine.ChangeState(citizenDeadState);
    }

    public void ChangeToFlee()
    {
        entityStateMachine.ChangeState(citizenFleeState);
    }

}
