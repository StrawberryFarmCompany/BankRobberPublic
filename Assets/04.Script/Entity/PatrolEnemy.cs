using UnityEngine;
using UnityEngine.AI;

public class PatrolEnemy : EnemyNPC
{
    protected override void Awake()
    {
        base.Awake();
        // 상태머신 초기화 (기본 상태)
        efsm = new EnemyStateMachine(this, EnemyStates.PatrolEnemyPatrolState);
    }

    private void Update()
    {
        efsm.Current.Execute(); // 현재 상태 실행
    }

    // 순찰
    public void Patrol(Vector3 pos)
    {
        PatrolEnemyPatrolState patrolState = (PatrolEnemyPatrolState)efsm.FindState(EnemyStates.PatrolEnemyPatrolState);
        patrolState.agent = gameObject.GetComponent<NavMeshAgent>();
        patrolState.pos = pos;
        efsm.ChangeState(efsm.FindState(EnemyStates.PatrolEnemyPatrolState));
    }

    // 두리번
    public void LookAround()
    {
        float lookAngle = 60f; // 좌우 확인 각도
        Quaternion originalRotation = transform.rotation;

        // 왼쪽 보기
        transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y - lookAngle, 0);
        Debug.Log("왼쪽 확인");

        // 오른쪽 보기
        transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y + (lookAngle * 2), 0);
        Debug.Log("오른쪽 확인");

        // 정면 복귀
        transform.rotation = originalRotation;
        Debug.Log("정면 복귀");
        
        // 소음 감지시 조사
        //if ()                // if문에 소음 났을때 추가
        //{
        //    efsm.ChangeState(efsm.FindState(EnemyStates.PatrolEnemyInvestigateState));
        //}

        //// 아니면 다시 순찰
        //else
        //{
        //    efsm.ChangeState(efsm.FindState(EnemyStates.PatrolEnemyPatrolState));
        //}
    }

    // 대미지 입었을 때
    public void TakeDamage()
    {
        // if (주사위에서 대미지 받는 매서드 받아오기)
        {
            //stats.curHp -= dice.dmg;   // 주사위에서 받은 대미지 값 필요
            
            efsm.ChangeState(efsm.FindState(EnemyStates.PatrolEnemyDamagedState));

            if(stats.CurHp <= 0)
            {
                Die();
            }
        }
    }

    // 사망
    public void Die()
    {
        efsm.ChangeState(efsm.FindState(EnemyStates.PatrolEnemyDeadState));
    }
}
