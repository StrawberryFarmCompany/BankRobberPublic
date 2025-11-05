using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
public class ManagerNPC : NeutralNPC
{
    public bool isDetection = false;
    Animator animator;
    protected override IEnumerator Start()
    {
        animator = GetComponent<Animator>();
        StartCoroutine(base.Start());
        yield return new WaitUntil(() => ResourceManager.GetInstance.IsLoaded);
        // 상태머신 초기화 (기본 상태 ManagerIdleState)
        nfsm = new NeutralStateMachine(this, transform.GetComponentInChildren<Animator>(), NeutralStates.ManagerIdleState);
        stats.OnDead += DeadAnimator;
        stats.OnDamaged += TakeDamage;
    }

    protected override void CalculateBehaviour()
    {
        List<EntityStats> visibleTargets = DetectVisibleTargets();

        if (visibleTargets.Count > 0 && isDetection == false)
        {
            isDetection = true;
            BankManagerWitness();
        }

        if (isDetection == true || stats.secData.GetSecLevel == 3)
        {
            nfsm.ChangeState(nfsm.FindState(NeutralStates.ManagerIdleCowerState));
        }

        base.CalculateBehaviour();
    }

    // 피격시 사망 // 위에서 Action에 추가 되어 있는데 추후에 해결하기.
    public void TakeDamage()
    {
        
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
