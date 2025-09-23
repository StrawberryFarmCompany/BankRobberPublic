using UnityEngine;
using UnityEngine.AI;

public class CitizenNPC : NeutralNPC
{
    bool isDetection = false;
    bool isHit = false;
    int securityLevel = 1;

    protected override void Awake()
    {
        base.Awake();
        nfsm = new NeutralStateMachine(this, NeutralStates.CitizenIdleState);
    }

    protected override void Update()
    {

    }

    protected override void CalculateBehaviour()
    {
        if (stats.CurHp != stats.maxHp)//맞으면 바로 죽음
        {
            ChangeToDead();
        }
        else if (securityLevel >= 3)//경계수준이 3레벨 이상이면
        {
            ChangeToCowerState();
        }
        else if (isDetection == true)//플레이어 발각시
        {
            //ChangeToFlee(//도망갈 위치?);
        }
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

}
