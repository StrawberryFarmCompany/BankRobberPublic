using UnityEngine;

public class ManagerNPC : NeutralNPC
{
    public bool canSeeAlly;
    public int securityLevel = 1; 

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

    // 턴 마다 실행될 매서드
    public void OnTurnStart()
    {
        // 피격시 사망
        if (stats.CurHp != stats.maxHp)
        {
            Die();
        }

        // 경계레벨이 2이하 또는 플레이어 발각시
        else if (canSeeAlly == true && securityLevel <= 2)
        {
            // Status에서 2턴의 목격자 Status를 받아옴. (경계수준 2미만일 경우 라고 되어 있는데 오타 인지 기획자 한테 물어보기)
            // 턴 끝날 때 마다 목격자 status -= 1 해주기.

            // if (status가 다시 0이 된다면)
            // {
            //     securityLevel += 1;
            // }
        }

        // 경계레벨이 3일 때
        else if (securityLevel == 3)
        {
            OnPlayerDetected();
        }
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
