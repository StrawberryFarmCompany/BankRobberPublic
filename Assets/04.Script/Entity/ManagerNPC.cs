using UnityEngine;
using System.Collections;
public class ManagerNPC : NeutralNPC
{
    public bool canSeeAlly;

    protected override IEnumerator Start()
    {
        StartCoroutine(base.Start());
        yield return new WaitUntil(() => ResourceManager.GetInstance.IsLoaded);
        // 상태머신 초기화 (기본 상태 ManagerIdleState)
        nfsm = new NeutralStateMachine(this, transform.GetComponentInChildren<Animator>(), NeutralStates.ManagerIdleState);

        stats.OnDamaged += TakeDamage;
    }

    protected override void CalculateBehaviour()
    {
        // 피격시 사망
        if (stats.CurHp != stats.maxHp)
        {
            Die();
        }

        // 플레이어 발각시
        else if (canSeeAlly == true)
        {
            // Status에서 3턴의 목격자 Status를 받아옴.
            // 턴 끝날 때 마다 목격자 status -= 1 해주기.

            // if (status가 다시 0이 된다면)
            // {
            //     securityLevel += 1;
            // }
        }

        // 경계레벨이 3일 때
        else if (stats.secData.GetSecLevel == 3)
        {
            OnPlayerDetected();
        }
        base.CalculateBehaviour();
    }

    // 피격시 사망 // 위에서 Action에 추가 되어 있는데 추후에 해결하기.
    public void TakeDamage()
    {
        // if (주사위에서 대미지 받는 매서드 받아오기)
        {
            Die();
        }
    }

    public void Die()
    {
        nfsm.ChangeState(nfsm.FindState(NeutralStates.ManagerDeadState));
    }

    public void OnPlayerDetected()
    {
        nfsm.ChangeState(nfsm.FindState(NeutralStates.ManagerIdleCowerState));
    }
}
