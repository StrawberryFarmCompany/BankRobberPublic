using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class HoldEnemyNPC : EnemyNPC
{
    bool isNoise = false;
    bool isNoisePlace = false;
    bool isHomePlace = true;

    [SerializeField] private Vector3 homeLocation;
    [SerializeField] private Vector3 noiseLocation;
    
    Animator animator;

    protected override IEnumerator Start()
    {
        animator = GetComponent<Animator>();
        StartCoroutine(base.Start());
        yield return new WaitUntil(() => ResourceManager.GetInstance.IsLoaded);
        efsm = new EnemyStateMachine(this,transform.GetComponentInChildren<Animator>(), EnemyStates.HoldEnemyIdleState);
        stats.OnDead += DeadAnimator;
    }

    protected override void Update()
    {
        base.Update();
    }

    protected override void CalculateBehaviour()
    {
        DetectNoise();
        DetectVisibleTargets();

        if (stats.secData.GetSecLevel == 0)
        {

            if (isNoise == false && isHomePlace == true) //소음 감지가 false라면
            {
                efsm.ChangeState(efsm.FindState(EnemyStates.HoldEnemyIdleState)); //대기 상태
            }

            else if (isNoise == false && isHomePlace == false)
            {
                TaskManager.GetInstance.AddTurnBehaviour(new TurnTask(() => { Move(homeLocation); }, 0f));
                efsm.eta = 3;
                efsm.ChangeState(efsm.FindState(EnemyStates.HoldEnemyChaseState));

                if (this.gameObject.transform.position == homeLocation)
                {
                    isHomePlace = true;
                }
            }

            else if (isNoise == true && isNoisePlace == false)
            {
                TaskManager.GetInstance.AddTurnBehaviour(new TurnTask(() => { Move(noiseLocation); }, 0f));
                efsm.eta = 3;
                efsm.ChangeState(efsm.FindState(EnemyStates.HoldEnemyInvestigateState));

                if (this.gameObject.transform.position == noiseLocation)
                {
                    isNoise = false;
                    isNoisePlace = true;
                }
            }

            else if (isNoise == false && isNoisePlace == true)//소음감지가 true 소음 발생지 도착시 외부에서 isNoisePlace를 트루로 만들어 주기
            {
                ChangeToIdleRotation();
                isNoisePlace = false;//한 턴 끝나고 isNoisPlace false만들기
            }
        }

        else if (stats.secData.GetSecLevel >= 1)
        {
            DetectVisibleTargets();
            if (nearPlayerLocation.currNode.GetCenter != null)
            {
                // LookAt 대신
                RotateToward(nearPlayerLocation.currNode.GetCenter, 0.3f);

                // 회전 끝난 후 공격
                DOVirtual.DelayedCall(0.3f, () => TryAttack());
            }
            else
            {
                TryAttack();
            }

            //공격이 실패했거나 행동력이 남았으면 추적 후 공격
            if (stats.curActionPoint > 0)
            {
                if (nearPlayerLocation != null)
                {
                    TaskManager.GetInstance.AddTurnBehaviour(new TurnTask(() => { Move(nearPlayerLocation.GetPosition()); }, 0f));
                    efsm.eta = 3;
                    efsm.ChangeState(efsm.FindState(EnemyStates.HoldEnemyChaseState));
                }

                else
                {
                    Debug.LogError($"플레이어 로케이션이 지정되지 않았습니다 : {gameObject.name}");
                }

            }
        }

        NoiseManager.ClearNoises(); // 게임메니저든 어디든 턴 종료시 한 번만 호출하게 해줘야함. (이동 필요!)

        base.CalculateBehaviour();
    }

    public void ChangeToIdleRotation()
    {
        float firstLookAngle = Random.Range(-180,180); // 첫 번째 각도 확인
        float secondLookAngle = Random.Range(-180,180); // 두 번째 각도 확인
        Quaternion originalRotation = transform.rotation;

        transform.rotation = Quaternion.Euler(0, firstLookAngle , 0);
        transform.rotation = Quaternion.Euler(0, secondLookAngle , 0);

        // 정면 복귀
        transform.rotation = originalRotation;
        efsm.ChangeState(efsm.FindState(EnemyStates.HoldEnemyIdleRotationState));
    }

    public void DeadAnimator()
    {
        animator.Play("Dead_Fwd");
    }

    public void DestroyObject()
    {
        GameManager.GetInstance.NoneBattleTurn.RemoveStartPointer(TurnTypes.enemy, CalculateBehaviour);
        Destroy(gameObject);
    }

    protected override void OnNoiseDetected(Vector3 noisePos)
    {
        isNoise = true;
        isHomePlace = false;
        noiseLocation = noisePos;
    }
}