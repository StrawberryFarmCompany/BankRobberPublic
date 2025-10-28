using BuffDefine;
using NodeDefines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyNPC : MonoBehaviour
{
    public EntityData entityData;
    protected EntityStats stats;
    protected EnemyStateMachine efsm;
    public Gun gun;

    public float fovAngle = 110f;    // 시야각 (부채꼴 각도)

    [SerializeField] protected EntityStats nearPlayerLocation;

    public Vector3? curNoise = null;


    protected virtual IEnumerator Start()
    {
        if (ResourceManager.GetInstance.IsLoaded == false) yield return new WaitUntil(() => ResourceManager.GetInstance.IsLoaded);
        stats = new EntityStats(entityData,gameObject);
        stats.NodeUpdates(transform.position);
        gun = GetComponent<Gun>();
        GameManager.GetInstance.NoneBattleTurn.RemoveStartPointer(TurnTypes.enemy, GameManager.GetInstance.NoneBattleTurn.NPCDefaultEnterPoint);
        GameManager.GetInstance.NoneBattleTurn.AddStartPointer(TurnTypes.enemy, CalculateBehaviour);

        yield return new WaitUntil(() => ResourceManager.GetInstance.GetBuffData.Count > 0);

        SecurityLevel(0);
    }

    protected virtual void FixedUpdate()
    {
        if (stats == null) return;
        stats.NodeUpdates(transform.position);
    }

    protected virtual void CalculateBehaviour()
    {
        stats.ResetForNewTurn(); // 행동력 및 이동력 초기화

        if (GameManager.GetInstance.CurrentPhase == GamePhase.NoneBattle)   
        {
            TaskManager.GetInstance.RemoveTurnBehaviour(new TurnTask(GameManager.GetInstance.NoneBattleTurn.ChangeState, 1f));
            TaskManager.GetInstance.AddTurnBehaviour(new TurnTask(GameManager.GetInstance.NoneBattleTurn.ChangeState, 0f));
        }
        else
        {
            //TaskManager.GetInstance.RemoveTurnBehaviour(new TurnTask(GameManager.GetInstance.BattleTurn.ChangeState, 1f));
            //TaskManager.GetInstance.AddTurnBehaviour(new TurnTask(GameManager.GetInstance.BattleTurn.ChangeState, 0f));
        }
    }
    
    public List<EntityStats> DetectVisibleTargets()
    {
        List<EntityStats> visibleTargets = new List<EntityStats>();
        
        // 적 자신의 노드
        Vector3Int enemyPos = stats.currNode.GetCenter;

        // 사정거리 안에 있는 모든 엔티티 가져오기
        List<EntityStats> targets = GameManager.GetInstance.GetEntitiesInRange(enemyPos, (int)stats.detectingDistance);//(int)stats.detectingDistance

        // 리스트 상태 출력 (디버그용)
        if (targets == null || targets.Count == 0)
        {
            Debug.LogError("[DetectVisibleTargets] 타겟 없음!");
            return visibleTargets;
        }

        foreach (var target in targets)
        {
            // 플레이어만 검출(적 무시)
            if (target.entityTag != EntityTag.ally)
            {
                continue;
            }

            // 1. 거리 체크 (사거리 무제인지 확인 필요)
            float dist = Vector3.Distance(stats.currNode.GetCenter, target.currNode.GetCenter);
            if (dist > stats.attackRange)
            {
                continue;
            }

            // 2. 시야각 체크
            Vector3 dirToTarget = (target.currNode.GetCenter - transform.position).normalized;
            float angle = Vector3.Angle(transform.forward, dirToTarget);
            if (angle > fovAngle * 0.5f) //&& angle < 180f -(fovAngle * 0.5f)) //55~135도 사이에 있는 값인지 체크
            {
                continue;
            }

            // 3. 장애물 체크 후 보이는 플레이어들 리스트에 추가
            if (CheckRangeAttack(target.currNode.GetCenter))
            {
                visibleTargets.Add(target);
                nearPlayerLocation = target;
                Debug.Log(nearPlayerLocation);
            }
        }
        
        if (visibleTargets.Count > 0 && stats.secData.GetSecLevel == 0)
        {
            SecurityLevel(1);
            SecurityCall();
            Witness();
            Debug.LogError($"발견 된 쁠레이어 : {visibleTargets.Count}");
        }
        return visibleTargets;
    }

    public void TryAttack() // TryAttack NavMesh.Raycast 를 Raycast로 바꿔야함
    {
        // 보이는 플레이어만 모은 리스트
        List<EntityStats> visibleTargets = DetectVisibleTargets();

        // 리스트에 아무도 없으면 리턴
        if (visibleTargets.Count == 0)
        {
            return;
        }

        // 랜덤으로 한 명 선택
        EntityStats chosenTarget = visibleTargets[Random.Range(0, visibleTargets.Count)];

        // 행동 포인트 확인 및 공격
        if (stats.ConsumeActionPoint(1))
        {
            if (gun.curRounds > 0)
            {
                TaskManager.GetInstance.AddTurnBehaviour(new TurnTask(() => { gun.Shoot(chosenTarget.currNode.GetCenter, 1); }, 0f));
                efsm.eta = 1f;
                efsm.ChangeState(efsm.FindState(EnemyStates.PatrolEnemyCombatState));
            }

            else
            {
                TaskManager.GetInstance.AddTurnBehaviour(new TurnTask(() => { gun.Reload(); }, 0f));
                efsm.eta = 1f;
                efsm.ChangeState(efsm.FindState(EnemyStates.PatrolEnemyReloadState));
                Debug.Log($"장전 완료 남은 총알 {gun.curRounds}발");
            }
        }
        else
        {
            Debug.Log("행동 포인트 부족");
        }
    }

    private bool CheckRangeAttack(Vector3 targetPos)
    {
        Vector3 start = transform.position + Vector3.up * 1.5f; // 적기준 몸통?에서 발싸
        Vector3 target = targetPos + Vector3.up * 1.5f;         // 플레이어 몸통 높이 정도에서 맞기.
        Vector3 direction = (target - start).normalized;
        float distance = Vector3.Distance(start, target);

        int layerMask = ~LayerMask.GetMask("Entity"); // Entity레이어만 제외하고 다

        // Raycast 상에서 장애물 체크
        if (Physics.Raycast(start, direction, out RaycastHit hit, distance, layerMask))
        {
            // 맞은 오브젝트 로그
            Debug.Log($"[CheckRangeAttack] Raycast hit: {hit.collider.name}");

            // 1️. 맞은 대상에서 EntityStats 가져오기
            EntityStats hitStats = hit.collider.GetComponent<EntityStats>();

            if (hitStats != null)
            {
                // 2️. EntityType으로 분기
                if (hitStats.entityTag == EntityTag.ally)
                {
                    Debug.DrawLine(start, direction * distance, Color.green, 10f);
                    Debug.Log("플레이어에 맞음");
                }
                else if (hitStats.entityTag == EntityTag.enemy)
                {
                    Debug.DrawLine(start, direction * distance, Color.red, 10f);
                    Debug.Log("적에 맞음");
                }
                else
                {
                    Debug.DrawLine(start, direction * distance, Color.yellow, 10f);
                    Debug.Log("기타 오브젝트에 맞음");
                }
            }
            else
            {
                Debug.DrawLine(start, direction * distance, Color.blue, 10f);
                Debug.Log("EntityStats가 없는 기물에 맞음 (예: 벽, 장애물 등)");
            }

            // 3️. 장애물 판정 (플레이어에 맞으면 통과, 벽이면 차단)
            Node node = GameManager.GetInstance.GetNode(hit.point);
            if (node != null && node.GetCenter == targetPos)
            {
                
                return true; // 시야 확보됨
            }
            
            return false; // 막힘
        }

        return true; // 아무것도 안 맞았으면 시야 확보됨
    }

    public void SecurityLevel(ushort level)
    {
        stats.secData.SetSecLevel(level);
    }

    public void SecurityCall()
    {
        if (ResourceManager.GetInstance.GetBuffData.TryGetValue(6004, out BuffData item))
        {
            stats.RegistBuff(item);
        }

        else
        {
            Debug.Log("키 값 조회 실패");
        }
    }

    public void CopCall()
    {
        if (ResourceManager.GetInstance.GetBuffData.TryGetValue(6005, out BuffData item))
        {
            stats.RegistBuff(item);
        }

        else
        {
            Debug.Log("키 값 조회 실패");
        }
    }
    
    public void Witness()
    {
        if (ResourceManager.GetInstance.GetBuffData.TryGetValue(6008, out BuffData item))
        {
            stats.RegistBuff(item);
        }

        else
        {
            Debug.Log("키 값 조회 실패");
        }
    }

    protected virtual bool DetectNoise()
    {
        var noises = NoiseManager.GetActiveNoises();
        if (noises == null || noises.Count == 0) return false;

        foreach (var noise in noises)
        {
            float distance = Vector3.Distance(transform.position, noise.pos);
            if (distance <= noise.radius)
            {
                float roll = Random.value;
                if (roll <= 0.5f) // 50% 확률 감지
                {
                    Debug.Log($"[{name}] 소음 감지 성공!");
                    OnNoiseDetected(noise.pos);
                    return true;
                }
                else
                {
                    Debug.Log($"[{name}] 소리를 듣지 못함.");
                }
            }
        }
        return false;
    }

    // 자식에서 오버라이드
    protected virtual void OnNoiseDetected(Vector3 noisePos)
    {
        // 기본은 아무 것도 안 함
    }
}