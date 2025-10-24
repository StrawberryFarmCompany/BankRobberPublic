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
        //SecurityLevel1();
        if (ResourceManager.GetInstance.IsLoaded == false) yield return new WaitUntil(() => ResourceManager.GetInstance.IsLoaded);
        stats = new EntityStats(entityData);
        stats.NodeUpdates(transform.position);
        gun = GetComponent<Gun>();
        GameManager.GetInstance.NoneBattleTurn.RemoveStartPointer(TurnTypes.enemy, GameManager.GetInstance.NoneBattleTurn.NPCDefaultEnterPoint);
        GameManager.GetInstance.NoneBattleTurn.AddStartPointer(TurnTypes.enemy, CalculateBehaviour);
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
                Witness();
                SecurityCall();
                SecurityLevel2();
            }
        }
        
        if (visibleTargets.Count > 0)
        {
            stats.secData.SetSecLevel(1);
            Debug.LogError("씨ㅣㅣ바라라라라랄ㄹㄹㄹㄹ");
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
                gun.Shoot(chosenTarget.currNode.GetCenter, 1);
            }

            else
            {
                gun.Reload();
                Debug.Log($"장전 완료 남은 총알 {gun.curRounds}발");
            }
        }
        else
        {
            Debug.Log("행동 포인트 부족");
        }
    }

    private bool CheckRangeAttack(Vector3Int targetPos)
    {
        Vector3 start = transform.position;
        Vector3 target = targetPos;
        // 1. NavMesh 상에서 장애물 체크
        if (NavMesh.Raycast(start, target, out NavMeshHit hit, NavMesh.AllAreas))
        {
            Debug.DrawRay(start, (hit.position - start), Color.red, 10f);
            Debug.Log("무언가로 막혀있음");
            return false;
        }
        else
        {
            Debug.Log("NavMesh 상에서 막히지 않음");
            // 막히지 않았다면 초록색 선
            Debug.DrawRay(start, (target - start), Color.green, 10f);
            return true;
        }
    }

    public void SetNoise(Vector3 noisePos)
    {
        curNoise = noisePos;
    }

    public void SecurityLevel1()
    {
        if (ResourceManager.GetInstance.GetBuffData.TryGetValue(6000, out BuffData item))
        {
            stats.RegistBuff(item);
        }

        else
        {
            Debug.Log("키 값 조회 실패");
        }
    }

    public void SecurityLevel2()
    {
        if (ResourceManager.GetInstance.GetBuffData.TryGetValue(6001, out BuffData item))
        {
            stats.RegistBuff(item);
        }

        else
        {
            Debug.Log("키 값 조회 실패");
        }
    }

    public void SecurityLevel3()
    {
        if (ResourceManager.GetInstance.GetBuffData.TryGetValue(6002, out BuffData item))
        {
            stats.RegistBuff(item);
        }

        else
        {
            Debug.Log("키 값 조회 실패");
        }
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

}