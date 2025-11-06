using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class PatrolEnemyNPC : EnemyNPC
{
    public bool departurePoint = true;       // 출발 지점
    public bool destinationPoint = false;    // 도착 지점
    public bool isNoise = false;             // 소음 감지
    public bool isArrivedNoisePlace = false; // 소음 발생 지역 도착

    [SerializeField] private List<Vector3> locationList = new List<Vector3>();
    [SerializeField] private int curLocation;

    [SerializeField] private Vector3 noiseLocation;

    Animator animator;

    protected override IEnumerator Start()
    {
        if (locationList != null) curLocation = 1;
        animator = GetComponent<Animator>();
        StartCoroutine(base.Start());
        yield return new WaitUntil(() => ResourceManager.GetInstance.IsLoaded);
        // 상태머신 초기화 (기본 상태)
        efsm = new EnemyStateMachine(this, transform.GetComponentInChildren<Animator>() ,EnemyStates.PatrolEnemyIdleRotationState);
        stats.OnDead += DeadAnimator;
    }
    
    protected override void Update()
    {
        base.Update();
    }

    // 턴마다 실행될 매서드
    protected override void CalculateBehaviour()
    {
        DetectNoise();

        // 항상 시야 갱신 — secLevel이 0이어도 한 번은 감지해야 전투 전환 가능(없으면 턴 그냥 넘어감)
        List<EntityStats> visibleTargets = DetectVisibleTargets();

        if (visibleTargets.Count >= 1 && stats.secData.GetSecLevel == 1)
        {
            SecurityLevel(1);
            SecurityCall();
            Witness();
        }

        if (stats.secData.GetSecLevel == 1)
        {
            if (isNoise == true && isArrivedNoisePlace == false)
            {
                TaskManager.GetInstance.AddTurnBehaviour(new TurnTask(() => { Move(noiseLocation); }, 0f));
                efsm.eta = 3;
                efsm.ChangeState(efsm.FindState(EnemyStates.PatrolEnemyInvestigateState));

                if (Vector3.Distance(transform.position, noiseLocation) < 0.1f)
                {
                    isArrivedNoisePlace = true;
                }
            }
            
            else if (isNoise == true && isArrivedNoisePlace == true)
            {
                IdleRotation();
                isNoise = false;
                isArrivedNoisePlace = false;
            }

            else if (GameManager.GetInstance.GetVecInt(locationList[curLocation]) != GameManager.GetInstance.GetVecInt(gameObject.transform.position))
            {
                TaskManager.GetInstance.AddTurnBehaviour(new TurnTask(() => { Move(locationList[curLocation]); }, 0f));
                efsm.eta = 3;
                efsm.ChangeState(efsm.FindState(EnemyStates.PatrolEnemyPatrolState));
            }

            else if (Vector3.Distance(transform.position, locationList[curLocation]) < 0.1f)
            {
                LookAround();
                curLocation++;
                curLocation = curLocation % locationList.Count;
            }
        }

        else if (stats.secData.GetSecLevel == 2)
        {
            DetectVisibleTargets();
            if (nearPlayerLocation  != null && nearPlayerLocation.currNode.GetCenter != null)
            {
                RotateToward(nearPlayerLocation.currNode.GetCenter, 0.3f);
            }
            TryAttack();

            // 공격이 실패했거나 행동력이 남았으면 추적 후 공격
            if (stats.curActionPoint > 0)
            {
                if (nearPlayerLocation != null)
                {
                    TaskManager.GetInstance.AddTurnBehaviour(new TurnTask(() => { Move(nearPlayerLocation.GetPosition()); }, 0f));
                    efsm.eta = 3;
                    efsm.ChangeState(efsm.FindState(EnemyStates.PatrolEnemyPatrolState));
                }

                else
                {
                    Debug.LogError($"플레이어 로케이션이 지정되지 않았습니다 : {gameObject.name}");
                }

            }
        }

        else if (stats.secData.GetSecLevel == 3)
        {
            CombatBehaviour();
        }

        NoiseManager.ClearNoises();

        base.CalculateBehaviour();
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

#if UNITY_EDITOR
    public void OnDrawGizmos()
    {
        if (locationList == null)
            return;

        Color[] colors = { Color.yellow, Color.red, Color.black, Color.blue, Color.green, Color.grey };

        for (int i = 0; i < locationList.Count && i < colors.Length; i++)
        {
            Gizmos.color = colors[i];
            Gizmos.DrawCube(locationList[i], Vector3.one);
        }
    }
#endif

    //순찰
    //public void Patrol(Vector3 pos)  //나중에 리펙토링 해보기
    //{
    //    PatrolEnemyPatrolState patrolState = (PatrolEnemyPatrolState)efsm.FindState(EnemyStates.PatrolEnemyPatrolState);
    //    if (patrolState.agent == null)
    //    {
    //        patrolState.agent = gameObject.GetComponent<NavMeshAgent>();
    //    }
    //    patrolState.pos.Enqueue(pos);

    //    float eta = patrolState.agent.remainingDistance / patrolState.agent.speed;
    //    efsm.ChangeState(efsm.FindState(EnemyStates.PatrolEnemyPatrolState));
    //}

    // 두리번
    public void LookAround()
    {
        float lookAngle = Random.Range(-180, 180); // 좌우 확인 각도
        Quaternion originalRotation = transform.rotation;

        // 왼쪽 보기
        transform.rotation = Quaternion.Euler(0, lookAngle, 0);

        // 정면 복귀
        //transform.rotation = originalRotation;
        //Debug.Log("정면 복귀");
    }

    public void StartIdleRotation()
    {
        StartCoroutine(IdleRotation());
    }

    private IEnumerator IdleRotation()
    {
        float firstLookAngle = Random.Range(-180, 180); // 첫 번째 각도 확인
        float secondLookAngle = Random.Range(-180, 180); // 두 번째 각도 확인
        Quaternion originalRotation = transform.rotation;
        
        this.gameObject.transform.rotation = Quaternion.Euler(0, firstLookAngle, 0);
        Debug.Log(firstLookAngle);
        yield return new WaitForSeconds(1.2f);
        
        this.gameObject.transform.rotation = Quaternion.Euler(0, secondLookAngle, 0);
        Debug.Log(secondLookAngle);
        yield return new WaitForSeconds(1.2f);

        // 정면 복귀
        transform.rotation = originalRotation;
        efsm.ChangeState(efsm.FindState(EnemyStates.PatrolEnemyIdleRotationState));
    }

    //public void Investigate(Vector3 pos)//나중에 리팩터링 해보기
    //{
    //    PatrolEnemyInvestigateState investigateState = (PatrolEnemyInvestigateState)efsm.FindState(EnemyStates.PatrolEnemyInvestigateState);
    //    investigateState.agent = gameObject.GetComponent<NavMeshAgent>();
    //    investigateState.pos = pos;
    //    float eta = investigateState.agent.remainingDistance / investigateState.agent.speed;
    //    efsm.ChangeState(efsm.FindState(EnemyStates.PatrolEnemyInvestigateState));
    //}

    // 대미지 입었을 때
    public void TakeDamage()
    {
        // if (주사위에서 대미지 받는 매서드 받아오기)
        {
            //stats.curHp -= dice.dmg;   // 주사위에서 받은 대미지 값 필요

            efsm.ChangeState(efsm.FindState(EnemyStates.PatrolEnemyDamagedState));

            if (stats.CurHp <= 0)
            {
                Die();
            }
        }
    }

    //public void Chase(Vector3 pos)//나중에 리팩터링 해보기
    //{
    //    PatrolEnemyChaseState ChaseState = (PatrolEnemyChaseState)efsm.FindState(EnemyStates.PatrolEnemyChaseState);

    //    if (ChaseState.agent == null)
    //    {
    //        ChaseState.agent = gameObject.GetComponent<NavMeshAgent>();
    //    }

    //    ChaseState.pos.Enqueue(pos);

    //    float eta = ChaseState.agent.remainingDistance / ChaseState.agent.speed;
    //    efsm.ChangeState(efsm.FindState(EnemyStates.PatrolEnemyChaseState));
    //}

    public void Die()
    {
        efsm.ChangeState(efsm.FindState(EnemyStates.PatrolEnemyDeadState));
    }

    protected override void OnNoiseDetected(Vector3 noisePos)
    {
        isNoise = true;
        isArrivedNoisePlace = false;
        noiseLocation = noisePos;
    }
}