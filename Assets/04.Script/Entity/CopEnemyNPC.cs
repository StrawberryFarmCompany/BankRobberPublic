using UnityEngine;

public class CopEnemyNPC : EnemyNPC
{
    bool isDetection = false;
    bool isHit = false;

    protected override void Awake()
    {
        base.Awake();
        efsm = new EnemyStateMachine(this, EnemyStates.CopEnemyChaseState);
    }

    protected override void CalculateBehaviour()
    {
        //if (사거리 내에 발각 스테이터스를 가진 얼라이 태그가 있다면)사거리내 발각 스테이터스 true를 가진 얼라이 태그가 있다면//발각시 스테이터스에 3을 초기화해줌 int값의 발각 스테이터스 321 이런식으로 턴마다 마이너스 해준다
        //{
        //    ChangeToCombat();//교전 총쏘기
        //}

        //else if (사거리 내에 발각 스테이터스를 가진 얼라이 태그가 없다면)사거리 7이라고 가정하고 사거리내 raycast에 발각 스테이터스를 가진 얼라이 태그가 닿았는지와 // 기획한테 물어봐 
        //{
        //    ChangeToChase(가까운 적 위치);
        //    
        //}
    }

    public void ChangeToChase()
    {
        efsm.ChangeState(efsm.FindState(EnemyStates.CopEnemyChaseState));
    }

    public void ChangeToCombat()
    {
        efsm.ChangeState(efsm.FindState(EnemyStates.CopEnemyCombatState));
    }

    public void ChangeToDamaged()
    {
        efsm.ChangeState(efsm.FindState(EnemyStates.CopEnemyDamagedState));
    }

    public void ChangeToDead()
    {
        efsm.ChangeState(efsm.FindState(EnemyStates.CopEnemyDeadState));
    }

}
