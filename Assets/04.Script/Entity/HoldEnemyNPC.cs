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

        List<EntityStats> visibleTargets = DetectVisibleTargets();

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

                if (Vector3.Distance(transform.position, homeLocation) < 0.1f)
                {
                    isHomePlace = true;
                }
            }

            else if (isNoise == true && isNoisePlace == false)
            {
                TaskManager.GetInstance.AddTurnBehaviour(new TurnTask(() => { Move(noiseLocation); }, 0f));
                efsm.eta = 3;
                efsm.ChangeState(efsm.FindState(EnemyStates.HoldEnemyInvestigateState));

                if (Vector3.Distance(transform.position, noiseLocation) < 0.1f)
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
            // 시야 갱신
            visibleTargets = DetectVisibleTargets();

            // 현재 타겟이 없거나 죽었으면 새로 찾기
            if (nearPlayerLocation == null || nearPlayerLocation.currNode == null || nearPlayerLocation.CurHp <= 0)
            {
                if (visibleTargets.Count > 0)
                {
                    nearPlayerLocation = visibleTargets[0];
                    Debug.Log($"{name} : 새 타깃 지정 -> {nearPlayerLocation.characterName}");
                }
                else
                {
                    //시야 안에 아무도 없으면, 전체 플레이어 중 가장 가까운 대상 탐색
                    var allPlayers = NodePlayerManager.GetInstance.GetAllPlayers();
                    if (allPlayers != null && allPlayers.Count > 0)
                    {
                        var closest = GetClosestAlivePlayer(allPlayers);
                        if (closest != null)
                        {
                            nearPlayerLocation = closest.playerStats;
                            Debug.Log($"{name}: 시야 밖 플레이어 추적 시작 -> {nearPlayerLocation.characterName}");
                        }
                        else
                        {
                            // 나중에 여기 아래에다가 게임 오버 넣으면 됨 
                            Debug.Log($"{name}: 추적할 플레이어 없음 -> 대기 상태 전환");
                            efsm.ChangeState(efsm.FindState(EnemyStates.HoldEnemyIdleState));
                            base.CalculateBehaviour();
                            return;
                        }
                    }
                }
            }

            // 이전 턴과 같은 타깃이면 이동 없이 공격만
            if (lastTarget == nearPlayerLocation && nearPlayerLocation != null)
            {
                Debug.Log($"{name}: 동일 타겟 {nearPlayerLocation.characterName} 재공격");
                RotateToward(nearPlayerLocation.currNode.GetCenter, 0.3f);

                DOVirtual.DelayedCall(0.3f, () =>
                {
                    TryAttack();

                    // 공격 후 죽었으면 타깃 초기화
                    if (nearPlayerLocation == null || nearPlayerLocation.CurHp <= 0)
                    {
                        Debug.Log($"{name}: 타깃 사망 -> 다음 타겟 탐색 준비");
                        nearPlayerLocation = null;
                    }
                });

                // 턴 종료 (이동 X)
                base.CalculateBehaviour();
                return;
            }

            // 새 타깃이면 이동 + 공격
            if (stats.movement > 0 && nearPlayerLocation != null)
            {
                TaskManager.GetInstance.AddTurnBehaviour(new TurnTask(() => { Move(nearPlayerLocation.GetPosition()); }, 0f));
                efsm.eta = 3;
                efsm.ChangeState(efsm.FindState(EnemyStates.HoldEnemyChaseState));

                RotateToward(nearPlayerLocation.currNode.GetCenter, 0.3f);

                DOVirtual.DelayedCall(0.3f, () =>
                {
                    TryAttack();

                    // 죽었으면 다음 턴에 새 타겟 찾게
                    if (nearPlayerLocation == null || nearPlayerLocation.CurHp <= 0)
                    {
                        Debug.Log($"{name}: 공격 후 타겟 사망 -> 타겟 초기화");
                        nearPlayerLocation = null;
                    }

                    // 마지막 타겟 저장
                    lastTarget = nearPlayerLocation;
                });
            }
            else
            {
                Debug.LogError($"플레이어 로케이션이 지정되지 않았습니다 : {gameObject.name}");
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