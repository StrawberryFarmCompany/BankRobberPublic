using UnityEngine;
using UnityEngine.AI;

public class HoldEnemyNPC : EnemyNPC
{
    bool isRangeDetection = false;
    bool isNoise = false;
    bool isNoisePlace = false;
    bool isHomePlace = true;
    bool allySpottedStatus = false;
    public int securityLevel = 1;
    int countTurn = 0;
    [SerializeField] private Vector3 homeLocation;
    [SerializeField] private Vector3 noiseLocation;
    [SerializeField] private Vector3 nearPlayerLocation;
    Gun gun;
    protected override void Awake()
    {
        base.Awake();
        efsm = new EnemyStateMachine(this, EnemyStates.HoldEnemyIdleState);
        gun = GetComponent<Gun>();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    protected void Update()
    {

    }

    protected override void CalculateBehaviour()
    {
        if (stats.CurHp <= 0)//체력이 0보다 낮거나 같으면
        {
            ChangeToDead();//사망
        }

        else if (securityLevel == 1)//경계수준 1레벨
        {
            if (isNoise == false && isHomePlace == true)//소음 감지가 false라면
            {
                ChangeToIdle();//대기 상태
            }

            else if (isNoise == false && isHomePlace == false)
            {
                ChangeToMoveReturn(homeLocation);
                if(this.gameObject.transform.position == homeLocation)
                {
                    isHomePlace = true;
                }
            }

            else if (isNoise == true && isNoisePlace == false)
            {
                ChangeToInvestigate(noiseLocation);//소음 재 감지시 외부에서 isNoise를 true로 만들어주기
                if (this.gameObject.transform.position == noiseLocation)
                {
                    isNoise = false;
                }
            }

            else if (isNoise == false && isNoisePlace == true)//소음감지가 true 소음 발생지 도착시 외부에서 isNoisePlace를 트루로 만들어 주기
            {
                ChangeToIdleRotation();
                isNoisePlace = false;//한 턴 끝나고 isNoisPlace false만들기
            }
        }

        else if (securityLevel >= 2)
        {
            TryAttack();
            Debug.Log("죽자 준게이야");

            // 공격이 실패했거나 행동력이 남았으면 추적
            if (stats.movement > 0)
            {
                ChangeToChase(nearPlayerLocation);
            }
        }
    }

    public void ChangeToIdle()
    {
        efsm.ChangeState(efsm.FindState(EnemyStates.HoldEnemyIdleState));
    }

    public void ChangeToInvestigate(Vector3 pos)
    {
        HoldEnemyInvestigateState investigateState = (HoldEnemyInvestigateState)efsm.FindState(EnemyStates.HoldEnemyInvestigateState);

        if(investigateState.agent == null)
        {
            investigateState.agent = gameObject.GetComponent<NavMeshAgent>();
        }

        investigateState.pos.Enqueue(pos);

        float eta = investigateState.agent.remainingDistance / investigateState.agent.speed;
        efsm.ChangeState(efsm.FindState(EnemyStates.HoldEnemyInvestigateState));
    }

    public void ChangeToIdleRotation()
    {
        float firstLookAngle = Random.Range(-180,180); // 첫 번째 각도 확인
        float secondLookAngle = Random.Range(-180,180); // 두 번째 각도 확인
        Quaternion originalRotation = transform.rotation;

        transform.rotation = Quaternion.Euler(0, firstLookAngle , 0);
        Debug.Log("첫 번째 랜덤 각도 두리번");

        transform.rotation = Quaternion.Euler(0, secondLookAngle , 0);
        Debug.Log("두 번째 랜덤 각도 두리번");

        // 정면 복귀
        transform.rotation = originalRotation;
        Debug.Log("정면 복귀");
        efsm.ChangeState(efsm.FindState(EnemyStates.HoldEnemyIdleRotationState));
    }

    public void ChangeToMoveReturn(Vector3 pos)
    {
        HoldEnemyMoveReturnState moveReturnState = (HoldEnemyMoveReturnState)efsm.FindState(EnemyStates.HoldEnemyMoveReturnState);

        if (moveReturnState.agent == null)
        {
            moveReturnState.agent = gameObject.GetComponent<NavMeshAgent>();
        }

        moveReturnState.pos.Enqueue(pos);

        float eta = moveReturnState.agent.remainingDistance / moveReturnState.agent.speed;
        efsm.ChangeState(efsm.FindState(EnemyStates.HoldEnemyMoveReturnState));
    }

    public void ChangeToChase(Vector3 pos)
    {
        HoldEnemyChaseState chaseState = (HoldEnemyChaseState)efsm.FindState(EnemyStates.HoldEnemyChaseState);

        if (chaseState.agent == null)
        {
            chaseState.agent = gameObject.GetComponent<NavMeshAgent>();
        }

        chaseState.pos.Enqueue(pos);

        float eta = chaseState.agent.remainingDistance / chaseState.agent.speed;
        efsm.ChangeState(efsm.FindState(EnemyStates.HoldEnemyChaseState));
    }

    public void ChangeToCombat()
    {
        //여기 시간 주기 1초?
        efsm.ChangeState(efsm.FindState(EnemyStates.HoldEnemyCombatState));
    }

    public void ChangeToDamaged()
    {
        efsm.ChangeState(efsm.FindState(EnemyStates.HoldEnemyDamagedState));
    }

    public void ChangeToDead()
    {
        efsm.ChangeState(efsm.FindState(EnemyStates.HoldEnemyDeadState));
    }
}
