using NodeDefines;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyNPC : MonoBehaviour
{
    public EntityData entityData;
    protected EntityStats stats;
    protected EnemyStateMachine efsm;
    public Gun gun;

    public float fovAngle = 110f;    // 시야각 (부채꼴 각도)
    public LayerMask obstacleMask;  // 장애물 레이어 (Raycast에 사용)

    protected virtual IEnumerator Start()
    {
        yield return new WaitUntil(() => ResourceManager.GetInstance.IsLoaded);
        stats = new EntityStats(entityData);
        GameManager.GetInstance.NoneBattleTurn.RemoveStartPointer(TurnTypes.enemy, GameManager.GetInstance.NoneBattleTurn.NPCDefaultEnterPoint);
        GameManager.GetInstance.NoneBattleTurn.AddStartPointer(TurnTypes.enemy, CalculateBehaviour);
        stats.NodeUpdates(transform.position);
        gun = GetComponent<Gun>();
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
            TaskManager.GetInstance.AddTurnBehaviour(new TurnTask(() => { }, 0.5f));
            TaskManager.GetInstance.AddTurnBehaviour(new TurnTask(GameManager.GetInstance.NoneBattleTurn.ChangeState, 0f));
        }
        else
        {
            TaskManager.GetInstance.RemoveTurnBehaviour(new TurnTask(GameManager.GetInstance.BattleTurn.ChangeState, 1f));
            TaskManager.GetInstance.AddTurnBehaviour(new TurnTask(() => { }, 0.5f));
            TaskManager.GetInstance.AddTurnBehaviour(new TurnTask(GameManager.GetInstance.BattleTurn.ChangeState, 0f));
        }
    }
    
    public List<EntityStats> DetectVisibleTargets()
    {
        List<EntityStats> visibleTargets = new List<EntityStats>();

        // 적 자신의 노드
        Vector3Int enemyPos = stats.currNode.GetCenter;

        // 사정거리 안에 있는 모든 엔티티 가져오기
        List<EntityStats> targets = GameManager.GetInstance.GetEntitiesInRange(enemyPos, (int)stats.detectingDistance);

        // 리스트 상태 출력 (디버그용)
        if (targets == null || targets.Count == 0)
        {
            Debug.LogError("[DetectVisibleTargets] 타겟 없음!");
            return visibleTargets;
        }

        foreach (var target in targets)
        {
            // 플레이어만 공격 (적은 무시)
            if (target.entityTag != EntityTag.ally)
            {
                Debug.Log("1. 성");
                continue;
            }

            // 1. 거리 체크 (사거리 무제인지 확인 필요)
            float dist = Vector3.Distance(stats.currNode.GetCenter, target.currNode.GetCenter);
            if (dist > stats.detectingDistance)
            {
                Debug.Log("2. 준");
                continue;
            }

            // 2. 시야각 체크
            Vector3 dirToTarget = (target.currNode.GetCenter - transform.position).normalized;
            float angle = Vector3.Angle(transform.forward, dirToTarget);
            if (angle > fovAngle * 0.5f) //&& angle < 180f -(fovAngle * 0.5f)) //55~135도 사이에 있는 값인지 체크
            {
                Debug.Log("3. 우");
                continue;
            }

            // 3. 장애물 체크 후 보이는 플레이어들 리스트에 추가
            if (CheckRangeAttack(target.currNode.GetCenter))
            {
                visibleTargets.Add(target);
            }
        }

        Debug.Log($"[DetectVisibleTargets] {visibleTargets.Count}명 시야 내 감지됨");
        
        if (visibleTargets.Count > 0)
        {
            stats.secData.SetSecLevel(2);
            Debug.Log("세큐리티 레벨 2로 상승");
        }
        return visibleTargets;
    }

    public void TryAttack()
    {
        // 보이는 플레이어만 모은 리스트
        List<EntityStats> visibleTargets = DetectVisibleTargets();

        // 리스트에 아무도 없으면 리턴
        if (visibleTargets.Count == 0)
        {
            Debug.Log("공격 가능한 친구 없음");
            return;
        }

        // 랜덤으로 한 명 선택
        EntityStats chosenTarget = visibleTargets[Random.Range(0, visibleTargets.Count)];

        // 행동 포인트 확인 및 공격
        if (stats.ConsumeActionPoint(1))
        {
            gun.Shoot(chosenTarget.currNode.GetCenter, 1);
            Debug.Log($"{stats.characterName}이(가) {chosenTarget.characterName}을(를) 향해 사격!");
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
}