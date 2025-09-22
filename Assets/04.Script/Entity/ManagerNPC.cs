using UnityEngine;

public class ManagerNPC : NeutralNPC
{
    private bool canSeeAlly;

    protected override void Awake()
    {
        base.Awake();
        // 상태머신 초기화 (기본 상태 ManagerIdleState)
        nfsm = new NeutralStateMachine(this, NeutralStates.ManagerIdleState);

        stats.OnDamaged += TakeDamage;
    }

    protected override void Update()
    {
        //현재 상태 실행
        nfsm.Current?.Execute();
    }

    public void OnTurnStart()
    {
        // 피격시 사망
        if (stats.CurHp != stats.maxHp)
        {
            Die();
        }

        // 플레이어 발견시 상태 변경
        else if (canSeeAlly)
        {
            nfsm.ChangeState(nfsm.FindState(NeutralStates.ManagerIdleCowerState));
        }
    }

    // 피격시 사망
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
