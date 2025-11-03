using BuffDefine;
using DG.Tweening;
using NodeDefines;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
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

    public NavMeshAgent agent;
    Queue<Vector3Int> pathQueue = new Queue<Vector3Int>();
    Vector3Int curTargetPos;
    bool canNextMove;
    bool isMoving;

    protected EntityStats lastTarget; // 이전 턴에 공격한 대상

    protected virtual IEnumerator Start()
    {
        if (ResourceManager.GetInstance.IsLoaded == false) yield return new WaitUntil(() => ResourceManager.GetInstance.IsLoaded);
        stats = new EntityStats(entityData,gameObject);
        stats.NodeUpdates(transform.position);
        gun = GetComponent<Gun>();
        GameManager.GetInstance.NoneBattleTurn.RemoveStartPointer(TurnTypes.enemy, GameManager.GetInstance.NoneBattleTurn.NPCDefaultEnterPoint);
        GameManager.GetInstance.NoneBattleTurn.AddStartPointer(TurnTypes.enemy, CalculateBehaviour);

        if (ResourceManager.GetInstance.GetBuffData.Count <= 0) yield return new WaitUntil(() => ResourceManager.GetInstance.GetBuffData.Count > 0);
        stats.CreateHpBar();
        stats.NodeUpdates(transform.position,true);
        SecurityLevel(0);
    }

    protected virtual void Update()
    {
        if (isMoving)
            SequentialMove();
    }

    protected virtual void CalculateBehaviour()
    {
        stats.ResetForNewTurn(); // 행동력 및 이동력 초기화

        if (GameManager.GetInstance.CurrentPhase == GamePhase.NoneBattle)   
        {
            TaskManager.GetInstance.RemoveTurnBehaviour(new TurnTask(GameManager.GetInstance.NoneBattleTurn.ChangeState, 1f));
            TaskManager.GetInstance.AddTurnBehaviour(new TurnTask(GameManager.GetInstance.NoneBattleTurn.ChangeState, 0f));
        }
        stats.NodeUpdates(transform.position);
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
            if (target.entityTag != EntityTag.Ally)
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
            }
        }

        if (visibleTargets.Count > 0 && stats.secData.GetSecLevel == 0)
        {
            // 거리 기준으로 정렬 (가까운 순)
            visibleTargets.Sort((a, b) =>
            Vector3.Distance(transform.position, a.currNode.GetCenter).CompareTo(Vector3.Distance(transform.position, b.currNode.GetCenter)));

            SecurityLevel(1);
            SecurityCall();
            Witness();
            Debug.LogError($"발견 된 쁠레이어 : {visibleTargets.Count}");

            // 첫 번째(가장 가까운) 대상
            EntityStats chosenTarget = visibleTargets[0];
            nearPlayerLocation = chosenTarget;
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
            nearPlayerLocation = null;
            Debug.Log($"{name}: 공격할 플레이어 없음.");
            return;
        }

        // 타깃이 null이거나 이미 사망했다면 새로 갱신
        if (nearPlayerLocation == null)
        {
            nearPlayerLocation = visibleTargets[0];
            Debug.Log($"{name}: 타깃 재지정 : {nearPlayerLocation.characterName}");
        }

        // 행동 포인트 확인
        if (!stats.ConsumeActionPoint(1))
        {
            Debug.Log($"{name}: 행동 포인트 부족으로 공격 불가");
            return;
        }

        // 총알이 있으면 공격, 없으면 장전
        if (gun.curRounds > 0)
        {
            if (nearPlayerLocation != null && nearPlayerLocation.currNode != null)
            {
                Vector3Int targetPos = nearPlayerLocation.currNode.GetCenter;
                TaskManager.GetInstance.AddTurnBehaviour(new TurnTask(() => {gun.Shoot(targetPos, 1);}, 0f));

                efsm.eta = 1f;
                efsm.ChangeState(efsm.FindState(EnemyStates.PatrolEnemyCombatState));
                Debug.Log($"{name}: {nearPlayerLocation.characterName} 공격!");
            }
            else
            {
                Debug.LogWarning($"{name}: 공격 대상 정보가 손상됨. 타깃 초기화");
                nearPlayerLocation = null;
            }
        }
        else
        {
            TaskManager.GetInstance.AddTurnBehaviour(new TurnTask(() => gun.Reload(), 0f));
            efsm.eta = 1f;
            efsm.ChangeState(efsm.FindState(EnemyStates.PatrolEnemyReloadState));
            Debug.Log($"{name}: 장전 중 ({gun.curRounds} 발 남음)");
        }
    }

    private bool CheckRangeAttack(Vector3 targetPos)
    {
        Vector3 start = transform.position + Vector3.up * 1.5f;
        Vector3 target = targetPos + Vector3.up * 1.5f;
        Vector3 direction = (target - start).normalized;
        float distance = Vector3.Distance(start, target);

        // 모든 레이어를 대상으로 쏘되 (LayerMask.None), Ray에 맞은 모든 걸 검사
        if (Physics.Raycast(start, direction, out RaycastHit hit, distance))
        {
            // 맞은 오브젝트에 NodePlayerController가 붙어 있다면 플레이어임
            NodePlayerController player = hit.collider.GetComponent<NodePlayerController>();
            if (player != null)
            {
                Debug.DrawLine(start, target, Color.green, 10f);
                Debug.Log($"플레이어 발견! ({hit.collider.name})");
                return true;
            }

            // 그 외는 시야를 가린 장애물
            Debug.DrawLine(start, target, Color.red, 10f);
            Debug.Log($"시야 차단: {hit.collider.name}");
        }
        return false;
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
                float roll = UnityEngine.Random.value;
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

    public void Move(Vector3 pos)
    {
        if (isMoving) return;
        Vector3Int targetPos = GameManager.GetInstance.GetVecInt(pos);

        // 플레이어가 있는 노드는 목적지로 하지 않도록 처리
        var playerNode = GameManager.GetInstance.GetNode(targetPos);
        if (playerNode != null && playerNode.Standing != null && playerNode.Standing.Count > 0)
        {
            // 플레이어 근처의 빈 노드 중 가장 가까운 곳 선택
            Vector3Int bestAdjacent = FindNearestWalkableNodeAround(GameManager.GetInstance.GetVecInt(playerNode.GetCenter));
            targetPos = bestAdjacent;
        }

        if (GameManager.GetInstance.GetNode(targetPos) == null)
        {
            return;
        }

        // 현재 좌표 (정수 격자 기준)
        Vector3Int start = GameManager.GetInstance.GetNode(transform.position).GetCenter;
        targetPos = GameManager.GetInstance.GetNode(targetPos).GetCenter;

        // 경로 배열 생성
        List<Vector3Int> path = GenerateChebyshevPath(start, targetPos);

        pathQueue.Clear();

        // 이동력만큼만 큐에 넣기
        foreach (var step in path)
        {
            if (stats.ConsumeMovement(1))
            {
                pathQueue.Enqueue(step);
            }
            else
            {
                Debug.Log($"이동 도중 이동력 부족. {step} 여기서 멈춤");
                break;
            }
        }

        if (pathQueue.Count > 0)
        {
            //최종 이동 구현
            isMoving = true;
            canNextMove = true;
        }
    }

    private List<Vector3Int> GenerateChebyshevPath(Vector3Int start, Vector3Int end)
    {
        // 도착지가 막혀 있다면 대체 노드 찾기
        if (!GameManager.GetInstance.Nodes.ContainsKey(end) ||
            GameManager.GetInstance.GetNode(end) == null ||
            !GameManager.GetInstance.GetNode(end).isWalkable ||
            GameManager.GetInstance.GetEntityAt(end) != null)
        {
            end = FindNearestWalkableNodeAround(end);
        }

        // BFS 탐색을 위한 큐
        Queue<Vector3Int> open = new Queue<Vector3Int>();
        Dictionary<Vector3Int, Vector3Int> cameFrom = new Dictionary<Vector3Int, Vector3Int>();

        open.Enqueue(start);
        cameFrom[start] = start;

        while (open.Count > 0)
        {
            Vector3Int current = open.Dequeue();

            // 목표에 도달하면 역추적해서 경로 반환
            if (current == end)
            {
                return ReconstructPath(cameFrom, start, end);
            }

            // 인접 노드 탐색 (대각선 포함 체비셰프)
            foreach (var dir in GameManager.GetInstance.nearNode)
            {
                Vector3Int next = current + dir;

                // 1) 노드 존재 여부 확인
                if (!GameManager.GetInstance.Nodes.ContainsKey(next)) continue;

                var node = GameManager.GetInstance.Nodes[next];

                // 2) 이동 가능한지 체크
                if (node == null) continue;
                if (!node.isWalkable) continue;
                //if (GameManager.GetInstance.GetEntityAt(next) != null) continue;

                // 3) 방문한 적 없는 경우만 추가
                if (!cameFrom.ContainsKey(next))
                {
                    cameFrom[next] = current;
                    open.Enqueue(next);
                }
            }
        }

        // 경로를 찾지 못한 경우
        Debug.Log("경로를 찾지 못했습니다.");
        return new List<Vector3Int>();
    }

    /// <summary>
    /// BFS 탐색 후 start→end까지 역추적
    /// </summary>
    private List<Vector3Int> ReconstructPath(Dictionary<Vector3Int, Vector3Int> cameFrom, Vector3Int start, Vector3Int end)
    {
        List<Vector3Int> path = new List<Vector3Int>();
        Vector3Int current = end;

        while (current != start)
        {
            path.Add(current);
            current = cameFrom[current];
        }

        path.Reverse();
        return path;
    }

    public void SequentialMove()
    {
        // 아직 목표가 없으면 다음 큐 꺼내기
        if (!isMoving) return;

        // 도착 판정 (== 대신 거리로 체크)
        if (Vector3.Distance(transform.position, curTargetPos) < 0.1f)
        {
            canNextMove = true;
        }

        if (canNextMove && pathQueue.Count > 0)
        {
            canNextMove = false;

            curTargetPos = pathQueue.Dequeue();
            agent.SetDestination(curTargetPos);
            stats.NodeUpdates(curTargetPos);
        }

        // 모든 경로 소모 시 이동 종료
        if (pathQueue.Count == 0 && Vector3.Distance(transform.position, curTargetPos) < 0.1f)
        {
            isMoving = false;
            if (nearPlayerLocation != null)
            {
                // LookAt 대신
                RotateToward(nearPlayerLocation.currNode.GetCenter, 0.3f);

                // 회전 끝난 후 공격
                DOVirtual.DelayedCall(0.3f, () => TryAttack());
            }
        }
    }

    // 가장 가까운 노드 찾기
    private Vector3Int FindNearestWalkableNodeAround(Vector3Int center)
    {
        Vector3Int best = center;
        float bestDist = float.MaxValue;

        foreach (var dir in GameManager.GetInstance.nearNode)
        {
            Vector3Int check = center + dir;
            if (!GameManager.GetInstance.Nodes.ContainsKey(check)) continue;

            var node = GameManager.GetInstance.Nodes[check];
            if (node == null || !node.isWalkable) continue;
            if (node.Standing != null && node.Standing.Count > 0) continue;

            float dist = Vector3.Distance(check, GameManager.GetInstance.GetNode(transform.position).GetCenter);
            if (dist < bestDist)
            {
                best = check;
                bestDist = dist;
            }
        }

        return best;
    }

    protected virtual void RotateToward(Vector3 targetPos, float duration = 0.3f)
    {
        // XZ 평면 기준 상대 방향 계산
        Vector2 relPos = new Vector2(targetPos.x, targetPos.z) - new Vector2(transform.position.x, transform.position.z);

        // 각도 계산 (라디안 → 도 단위)
        float radian = Mathf.Atan2(relPos.x, relPos.y);
        float angle = Mathf.Rad2Deg * radian;

        // 현재 회전 중이라면 중첩 방지
        transform.DOKill();

        // DOTween으로 부드럽게 회전
        transform.DORotate(Vector3.up * angle, duration).SetEase(Ease.OutQuad);
    }

    protected NodePlayerController GetClosestAlivePlayer(List<NodePlayerController> players)
    {
        NodePlayerController closest = null;
        float minDist = float.MaxValue;

        foreach (var p in players)
        {
            if (p == null) continue; // 사망 등
            float dist = Vector3.Distance(transform.position, p.playerStats.GetPosition());
            if (dist < minDist)
            {
                minDist = dist;
                closest = p;
            }
        }

        return closest;
    }

    private void OnDestroy()
    {
        transform.DOKill(false);
        stats.DestroyEntity();
    }
}