using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CitizenNPC : NeutralNPC
{
    public bool isDetection = false;
    public bool isCowered = false;
    [SerializeField] private Vector3 exitArea;

    Animator animator;

    protected override IEnumerator Start()
    {
        animator = GetComponent<Animator>();
        StartCoroutine(base.Start());
        yield return new WaitUntil(() => ResourceManager.GetInstance.IsLoaded);
        nfsm = new NeutralStateMachine(this, transform.GetComponentInChildren<Animator>(), NeutralStates.CitizenIdleState);
        stats.OnDead += DeadAnimator;
    }

    protected override void CalculateBehaviour()
    {
        List<EntityStats> visibleTargets = DetectVisibleTargets();

        if (visibleTargets.Count > 0 && isDetection == false)
        {
            isDetection = true;
            CitizenWitness();
        }

        if (stats.secData.GetSecLevel >= 3 && isCowered == false)
        {
            Debug.Log("겁먹은 상태");
            ChangeToCowerState();
            isCowered = true;
        }

        else if (isDetection == true && isCowered == false) //플레이어 발각시
        {
            Debug.Log("도망가는 상태");
            TaskManager.GetInstance.AddTurnBehaviour(new TurnTask(() => { Move(exitArea); }, 0f));
        }

        EndMyTurn();
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

    public void DeadAnimator()
    {
        animator.Play("Dead_Fwd");
    }

    public void DestroyObject()
    {
        GameManager.GetInstance.NoneBattleTurn.RemoveStartPointer(TurnTypes.neutral, CalculateBehaviour);
        Destroy(gameObject);
    }
}
