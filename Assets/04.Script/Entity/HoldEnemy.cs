using UnityEngine;
using UnityEngine.AI;

public class HoldEnemy : EnemyNPC
{
    bool isDetection = false;
    bool isHit = false;
    bool isNoise = false;
    bool isNoisePlace = false;
    int alertLevel = 1;
    int countTurn = 0;

    protected override void Awake()
    {
        base.Awake();
        efsm = new EnemyStateMachine(this, EnemyStates.HoldEnemyIdleState);
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
        HoldEnemyIdleRotationState idleState = (HoldEnemyIdleRotationState)efsm.FindState(EnemyStates.HoldEnemyIdleRotationState);
        idleState.agent = gameObject.GetComponent<NavMeshAgent>();
        idleState.pos = pos;
        efsm.ChangeState(efsm.FindState(EnemyStates.HoldEnemyIdleRotationState));
    }

    public void ChangeToChase()
    {
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

    public void HoldEnemyBehaviour()
    {
        if(stats.CurHp<=0)//체력이 0보다 낮거나 같으면
        {
            ChangeToDead();//사망
        }
        else if(alertLevel==1 && isNoise==false)//경계수준이 1이고 소음 감지가 false라면
        {
            ChangeToIdle();//대기상태로 돌아감
        }
        else if (alertLevel == 1 && isNoise==true && isNoisePlace==true)//경계수준이 1이고 소음감지가 true 
        {
            ChangeToIdleRotation();
            isNoise = false;
            isNoisePlace = false;
        }
        else if (alertLevel==1 && isNoise==true)
        {
            //ChangeToInvestigate(//소음 발생지로 이동);
            //소음 발생지 도착시 isNoise = false;
        }
        else if (alertLevel >= 2)//사거리내 발각 스테이터스를 가진 얼라이 태그가 없다면
        {
            ChangeToChase();
        }
        else if (alertLevel >= 2)//사거리내 발각 스테이터스를 가진 얼라이 태그가 있다면
        {
            ChangeToCombat();
        }
    }
}
