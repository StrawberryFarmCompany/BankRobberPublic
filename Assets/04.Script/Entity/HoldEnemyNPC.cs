using UnityEngine;
using UnityEngine.AI;
using static UnityEditor.PlayerSettings;

public class HoldEnemyNPC : EnemyNPC
{
    bool isRangeDetection = false;
    bool isNoise = false;
    bool isNoisePlace = false;
    bool isHomePlace = true;
    bool allySpottedStatus = false;
    int securityLevel = 1;
    int countTurn = 0;
    [SerializeField] Vector3 homeLocation;
    [SerializeField] Vector3 noiseLocation;
    [SerializeField] Vector3 nearPlayerLocation;

    protected override void Awake()
    {
        base.Awake();
        efsm = new EnemyStateMachine(this, EnemyStates.HoldEnemyIdleState);
    }
    protected void Update()
    {
        CalculateBehaviour();
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
            if (securityLevel >= 2 && allySpottedStatus == true && isRangeDetection == true)//사거리내 발각 스테이터스 true를 가진 얼라이 태그가 있다면//발각시 스테이터스에 3을 초기화해줌 int값의 발각 스테이터스 321 이런식으로 턴마다 마이너스 해준다
            {
                ChangeToCombat();//교전 총쏘기
                Debug.Log("총쏜다");
            }

            else if (securityLevel >= 2 && allySpottedStatus == true)
            {
                ChangeToChase(nearPlayerLocation);
                Debug.Log("적 찾으러 간다");
                //사거리 7이라고 가정하고 사거리내 raycast에 발각 스테이터스를 가진 얼라이 태그가 닿았는지와 // 기획한테 물어봐 
            }
        }

        TaskManager.GetInstance.AddTurnBehaviour(new TurnTask(base.CalculateBehaviour, 0.1f));
    }

    public void ChangeToIdle()
    {
        efsm.ChangeState(efsm.FindState(EnemyStates.HoldEnemyIdleState));
    }

    public void ChangeToInvestigate(Vector3 pos)
    {
        HoldEnemyInvestigateState investigateState = (HoldEnemyInvestigateState)efsm.FindState(EnemyStates.HoldEnemyInvestigateState);
        investigateState.agent = gameObject.GetComponent<NavMeshAgent>();
        investigateState.pos = pos;
        float eta = investigateState.agent.remainingDistance / investigateState.agent.speed;
        efsm.ChangeState(efsm.FindState(EnemyStates.HoldEnemyInvestigateState));
    }

    public void ChangeToIdleRotation()
    {
        efsm.ChangeState(efsm.FindState(EnemyStates.HoldEnemyIdleRotationState));
    }

    public void ChangeToMoveReturn(Vector3 pos)
    {
        HoldEnemyMoveReturnState moveReturnState = (HoldEnemyMoveReturnState)efsm.FindState(EnemyStates.HoldEnemyMoveReturnState);
        moveReturnState.agent = gameObject.GetComponent<NavMeshAgent>();
        moveReturnState.pos = pos;
        float eta = moveReturnState.agent.remainingDistance / moveReturnState.agent.speed;
        efsm.ChangeState(efsm.FindState(EnemyStates.HoldEnemyMoveReturnState));
    }

    public void ChangeToChase(Vector3 pos)
    {
        HoldEnemyChaseState chaseState = (HoldEnemyChaseState)efsm.FindState(EnemyStates.HoldEnemyChaseState);
        chaseState.agent = gameObject.GetComponent<NavMeshAgent>();
        chaseState.pos = pos;
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
