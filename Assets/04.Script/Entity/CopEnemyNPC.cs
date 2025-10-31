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
        DetectVisibleTargets();
        if (nearPlayerLocation.currNode.GetCenter != null)
        {
            // LookAt 대신
            RotateToward(nearPlayerLocation.currNode.GetCenter, 0.3f);

            // 회전 끝난 후 공격
            DOVirtual.DelayedCall(0.3f, () => TryAttack());
        }
        else
        {
            TryAttack();
        }

        // 공격이 실패했거나 이동력이 남았으면 추적
        if (stats.curActionPoint > 0)
        {
            
            if (nearPlayerLocation != null)
            {
                TaskManager.GetInstance.AddTurnBehaviour(new TurnTask(() => { Move(nearPlayerLocation.GetPosition()); }, 0f));
                efsm.eta = 3;
                efsm.ChangeState(efsm.FindState(EnemyStates.CopEnemyChaseState));
            }

            else
            {
                Debug.LogError($"플레이어 로케이션이 지정되지 않았습니다 : {gameObject.name}");
            }
            
        }
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