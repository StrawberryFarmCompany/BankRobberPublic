using UnityEngine;
using UnityEngine.AI;
using System.Collections;
public class CitizenNPC : NeutralNPC
{
    public bool isDetection = false;
    [SerializeField] private Vector3 exitArea;

    protected override IEnumerator Start()
    {
        StartCoroutine(base.Start());
        yield return new WaitUntil(() => ResourceManager.GetInstance.IsLoaded);
        nfsm = new NeutralStateMachine(this, NeutralStates.CitizenIdleState);
        yield return null;
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    protected override void Update()
    {

    }

    protected override void CalculateBehaviour()
    {
        if (stats.CurHp != stats.maxHp)//맞으면 바로 죽음
        {
            Debug.Log("죽은상태");
            ChangeToDead();
        }

        else if(stats.secData.GetSecLevel >= 3)
        {
            Debug.Log("개쫄은상태");
            ChangeToCowerState();
        }

        else if (isDetection == true)//플레이어 발각시
        {
            Debug.Log("존나 튀는 상태");
            ChangeToFlee(exitArea);
        }

        else 
        {
            Debug.Log("대기상태");
            ChangeToIdle();
        }
        base.CalculateBehaviour();
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
        if (fleeState.agent == null)
        {
            fleeState.agent = gameObject.GetComponent<NavMeshAgent>();
        }
        fleeState.pos.Enqueue(pos);
        nfsm.ChangeState(nfsm.FindState (NeutralStates.CitizenFleeState));
    }
}
