using UnityEngine;

public class ManagerNPC : MonoBehaviour
{
    public EntityData baseData;
    private PlayerStats stats;
    private NeutralStateMachine nfsm;

    private bool canSeeAlly;

    private void Awake()
    {
        // 직원마다 독립된 스탯 생성
        stats = new PlayerStats(baseData);

        // 상태머신 초기화 (기본 상태 ManagerIdleState)
        nfsm = new NeutralStateMachine(this, NeutralStates.ManagerIdleState);
    }

    private void Update()
    {
        //현재 상태 실행
        nfsm.Current?.Execute();
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
        if (canSeeAlly)
        {
            nfsm.ChangeState(nfsm.FindState(NeutralStates.ManagerIdleCowerState));
        }
    }
}
