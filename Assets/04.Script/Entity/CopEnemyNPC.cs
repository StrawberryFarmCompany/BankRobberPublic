using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CopEnemyNPC : EnemyNPC
{
    Animator animator;

    protected override IEnumerator Start()
    {
        animator = GetComponent<Animator>();
        StartCoroutine(base.Start());
        yield return new WaitUntil(() => ResourceManager.GetInstance.IsLoaded);
        efsm = new EnemyStateMachine(this, transform.GetComponentInChildren<Animator>(), EnemyStates.PatrolEnemyIdleRotationState);
        stats.OnDead += DeadAnimator;
    }

    protected override void Update()
    {
        base.Update();
    }

    protected override void CalculateBehaviour()
    {
        CombatBehaviour();

        base.CalculateBehaviour();
    }

    //public void ChangeToChase()
    //{
    //    efsm.ChangeState(efsm.FindState(EnemyStates.CopEnemyChaseState));
    //}

    //public void ChangeToCombat()
    //{
    //    efsm.ChangeState(efsm.FindState(EnemyStates.CopEnemyCombatState));
    //}

    //public void ChangeToDamaged()
    //{
    //    efsm.ChangeState(efsm.FindState(EnemyStates.CopEnemyDamagedState));
    //}

    //public void ChangeToDead()
    //{
    //    efsm.ChangeState(efsm.FindState(EnemyStates.CopEnemyDeadState));
    //}
    public void DeadAnimator()
    {
        animator.Play("Dead_Fwd");
    }

    public void DestroyObject()
    {
        GameManager.GetInstance.NoneBattleTurn.RemoveStartPointer(TurnTypes.enemy, CalculateBehaviour);
        Destroy(gameObject);
    }
}