using NodeDefines;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyNPC : MonoBehaviour
{
    public EntityData entityData;
    protected EntityStats stats;
    protected EnemyStateMachine efsm;
    public Node currNode;

    public float fovAngle = 110f;    // 시야각 (부채꼴 각도)
    public LayerMask obstacleMask;  // 장애물 레이어 (Raycast에 사용)

    protected virtual void Awake()
    {
        stats = new EntityStats(entityData);
        GameManager.GetInstance.NoneBattleTurn.AddStartPointer(TurnTypes.enemy, CalculateBehaviour);
    }

    protected virtual void Start()
    {
        stats.currNode = GameManager.GetInstance.GetNode(transform.position);
    }

    protected virtual void FixedUpdate()
    {
        stats.NodeUpdates(transform.position);
    }


    protected virtual void CalculateBehaviour()
    {
        Debug.Log("ㅁㄴㅇㄹ");
        if (GameManager.GetInstance.CurrentPhase == GamePhase.NoneBattle)
        {
            TaskManager.GetInstance.RemoveTurnBehaviour(new TurnTask(GameManager.GetInstance.NoneBattleTurn.ChangeState, 0.1f));
            TaskManager.GetInstance.AddTurnBehaviour(new TurnTask(GameManager.GetInstance.NoneBattleTurn.ChangeState, 0.1f));
        }

        else
        {
            TaskManager.GetInstance.RemoveTurnBehaviour(new TurnTask(GameManager.GetInstance.BattleTurn.ChangeState, 0.1f));
            TaskManager.GetInstance.AddTurnBehaviour(new TurnTask(GameManager.GetInstance.BattleTurn.ChangeState, 0.1f));
        }
    }
    
    public void TryAttack()
    {
        // 적 자신의 노드
        Vector3Int enemyPos = stats.currNode.GetCenter;

        // 사정거리 안에 있는 모든 엔티티 가져오기
        List<EntityStats> targets = GameManager.GetInstance.GetEntitiesInRange(enemyPos, (int)stats.attackRange);

        foreach (var target in targets)
        {
            // 플레이어만 공격 (적은 무시)
            if (target.entityTag != EntityTag.ally)
                continue;

            // 1. 거리 체크 (사거리 무제인지 확인 필요)
            float dist = Vector3.Distance(stats.currNode.GetCenter, target.currNode.GetCenter);
            if (dist > stats.attackRange)
                continue;

            // 2. 시야각 체크
            Vector3 dirToTarget = (target.currNode.GetCenter - transform.position).normalized;
            float angle = Vector3.Angle(transform.forward, dirToTarget);
            if (angle > fovAngle * 0.5f) //&& angle < 180f -(fovAngle * 0.5f)) //55~135도 사이에 있는 값인지 체크
                continue;

            // 3. 장애물(Line of Sight) 체크
            if (!HasLineOfSight(target))
                continue;

            // 4. 행동력 확인 후 공격
            if (stats.ConsumeActionPoint(1))
            {
                PerformAttack(target);
                return; // 한 번 공격하면 종료 (여러 명 공격하지 않음)
            }
        }
    }

    private bool HasLineOfSight(EntityStats target)
    {
        Vector3 start = transform.position + Vector3.up * 1.5f;
        Vector3 end = target.currNode.GetCenter + Vector3.up * 1.5f;
        Vector3 dir = (end - start).normalized;

        if (Physics.Raycast(start, dir, out RaycastHit hit, Vector3.Distance(start, end), obstacleMask))
        {
            // 맞은 게 플레이어면 Line of Sight OK
            return hit.transform.CompareTag("Player");
        }
        return false;
    }

    private void PerformAttack(EntityStats target)
    {
        // 예시: 주사위 굴려서 명중 판정
        float dice = Random.Range(1, 20);
        bool isHit = stats.IsHit(dice, 0, target);

        if (isHit)
        {
            target.Damaged(10); // 데미지 10 (예시)
            Debug.Log($"{stats.characterName} → {target.characterName} 명중! 현재 HP: {target.CurHp}");
        }
        else
        {
            Debug.Log($"{stats.characterName} → {target.characterName} 빗나감!");
        }
    }
}