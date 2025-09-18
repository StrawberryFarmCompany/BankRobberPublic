using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Manager : MonoBehaviour
{
    private NeutralStateMachine neutralStateMachine;
    private ManagerIdleState managerIdleState;
    private ManagerDeadState managerDeadState;
    private ManagerIdleCowerState managerIdleCowerState;

    public EntityData baseData;

    public int curHp;
    public int curActionPoint;
    public int curRerollCount;

    public bool CanSeeAlly;

    private void Awake()
    {
        neutralStateMachine = new NeutralStateMachine();
        managerIdleState = new ManagerIdleState();
        managerDeadState = new ManagerDeadState();
        managerIdleCowerState = new ManagerIdleCowerState();

        neutralStateMachine.ForceSet(managerIdleState);

        curHp = baseData.maxHp;
        curActionPoint = baseData.maxActionPoint;
        curRerollCount = baseData.maxRerollCount;
    }

    private void Start()
    {
        
    }

    private void Update()
    {
        neutralStateMachine.Current?.Execute();
    }

    public void ChangeToIdle()
    {
        neutralStateMachine.ChangeState(managerIdleState);
    }

    public void ChangeToIdleCower()
    {
        neutralStateMachine.ChangeState(managerIdleCowerState);
    }

    public void ChangeToDead()
    {
        neutralStateMachine.ChangeState(managerDeadState);
    }
}
