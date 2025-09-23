using UnityEngine;
using UnityEngine.AI;

public class PatrolEnemy : EnemyNPC
{
    public int securityLevel = 1;
    public bool departurePoint = true;       // 출발 지점
    public bool destinationPoint = false;    // 도착 지점
    public bool isNoise = false;             // 소음 감지
    public bool isArrivedNoisePlace = false; // 소음 발생 지역 도착

    protected override void Awake()
    {
        base.Awake();
        // 상태머신 초기화 (기본 상태)
        efsm = new EnemyStateMachine(this, EnemyStates.PatrolEnemyPatrolState);
    }

    private void Update()
    {
        efsm.Current?.Execute(); // 현재 상태 실행
    }

    // 턴마다 실행될 매서드
    public void OnTurnStart()
    {
        if(stats.CurHp <= 0)
        {
            Die();
        }

        else if (securityLevel == 1)
        {
            if (isNoise == true && isArrivedNoisePlace == false)
            {
                //Investigate(소리 난 지역 pos넣기);
                //if (현재 위치가 pos와 같으면)
                {
                    isArrivedNoisePlace = true;
                }
            }

            else if (isNoise == true && isArrivedNoisePlace == true)
            {
                LookAround();
                isNoise = false;
                isArrivedNoisePlace = false;
            }

            else if (departurePoint == true && destinationPoint == false)
            {
                //Patrol(도착 지점으로 가는 pos넣기);
                //if (현재 위치가 pos와 같으면 )
                {
                    LookAround();
                    departurePoint = false;     // 출발 지점 true, false로 계속 바꿔주기
                    destinationPoint = true;    // 도착 지점 true, false로 계속 바꿔주기
                }
            }

            else if (departurePoint == false && destinationPoint == true)
            {
                //Patrol(시작 지점으로 가는 pos넣기);
                //if (현재 위치가 pos와 같으면)
                {
                    LookAround();
                    departurePoint = true;      // 출발 지점 true, false로 계속 바꿔주기
                    destinationPoint = false;   // 도착 지점 true, false로 계속 바꿔주기
                }
            }
        }

        else if (securityLevel >= 2)
        {
            if (securityLevel >= 2)//사거리내 발각 스테이터스 true를 가진 얼라이 태그가 있다면//발각시 스테이터스에 3을 초기화해줌 int값의 발각 스테이터스 321 이런식으로 턴마다 마이너스 해준다
            {
                Combat();//교전 총쏘기
            }

            else if (securityLevel >= 2)
            {
                //ChangeToChase(가까운 적 위치);
                //사거리 7이라고 가정하고 사거리내 raycast에 발각 스테이터스를 가진 얼라이 태그가 닿았는지와 // 기획한테 물어봐 
            }
        }
    }

    // 순찰
    public void Patrol(Vector3 pos)
    {
        PatrolEnemyPatrolState patrolState = (PatrolEnemyPatrolState)efsm.FindState(EnemyStates.PatrolEnemyPatrolState);
        NavMeshPath path = new NavMeshPath();
        if (agent.CalculatePath(pos,path))
        {
            patrolState.pos = pos;

            float eta = agent.remainingDistance / agent.speed;
            efsm.ChangeState(efsm.FindState(EnemyStates.PatrolEnemyPatrolState));
        }
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
    }

    public void Investigate(Vector3 pos)
    {
        PatrolEnemyInvestigateState investigateState = (PatrolEnemyInvestigateState)efsm.FindState(EnemyStates.PatrolEnemyInvestigateState);
        investigateState.agent = gameObject.GetComponent<NavMeshAgent>();
        investigateState.pos = pos;
        float eta = investigateState.agent.remainingDistance / investigateState.agent.speed;
        efsm.ChangeState(efsm.FindState(EnemyStates.PatrolEnemyInvestigateState));
    }

    public void Combat()
    {
        efsm.ChangeState(efsm.FindState(EnemyStates.PatrolEnemyCombatState));
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
