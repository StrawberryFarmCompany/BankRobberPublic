using BuffDefine;
using NodeDefines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NeutralNPC : MonoBehaviour
{
    public EntityData entityData;
    protected EntityStats stats;
    protected NeutralStateMachine nfsm;

    public float fovAngle = 110f;

    protected virtual IEnumerator Start()
    {
        if (ResourceManager.GetInstance.IsLoaded == false) yield return new WaitUntil(() => ResourceManager.GetInstance.IsLoaded);
        stats = new EntityStats(entityData);
        stats.NodeUpdates(transform.position);
        GameManager.GetInstance.NoneBattleTurn.RemoveStartPointer(TurnTypes.enemy, GameManager.GetInstance.NoneBattleTurn.NPCDefaultEnterPoint);
        GameManager.GetInstance.NoneBattleTurn.AddStartPointer(TurnTypes.neutral, CalculateBehaviour);

        yield return new WaitUntil(() => ResourceManager.GetInstance.GetBuffData.Count > 0);
        stats.secData.SetSecLevel(0);

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
/*        else
        {
            TaskManager.GetInstance.RemoveTurnBehaviour(new TurnTask(GameManager.GetInstance.BattleTurn.ChangeState, 1f));
            TaskManager.GetInstance.AddTurnBehaviour(new TurnTask(() => { }, 1f));
            TaskManager.GetInstance.AddTurnBehaviour(new TurnTask(GameManager.GetInstance.BattleTurn.ChangeState, 0f));
        }*/
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
            if (dist > stats.detectingDistance)
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
            }
        }

        if (visibleTargets.Count > 0 && stats.secData.GetSecLevel == 0)
        {
            SecurityLevel(1);
        }
        return visibleTargets;
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

    public void SecurityLevel(ushort level)
    {
        stats.secData.SetSecLevel(level);
    }

    public void CitizenWitness()
    {
        if (ResourceManager.GetInstance.GetBuffData.TryGetValue(6006, out BuffData item))
        {
            stats.RegistBuff(item);
        }

        else
        {
            Debug.Log("키 값 조회 실패");
        }
    }

    public void BankManagerWitness()
    {
        if (ResourceManager.GetInstance.GetBuffData.TryGetValue(6007, out BuffData item))
        {
            stats.RegistBuff(item);
        }

        else
        {
            Debug.Log("키 값 조회 실패");
        }
    }
}