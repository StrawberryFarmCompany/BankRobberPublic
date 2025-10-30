using BuffDefine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class CitizenNPC : NeutralNPC
{
    public bool isDetection = false;
    [SerializeField] private Vector3 exitArea;

    public NavMeshAgent agent;
    Queue<Vector3Int> pathQueue = new Queue<Vector3Int>();
    Vector3Int curTargetPos;
    public bool isMoving;
    bool canNextMove;
    Animator animator;
    protected override IEnumerator Start()
    {
        animator = GetComponent<Animator>();
        StartCoroutine(base.Start());
        yield return new WaitUntil(() => ResourceManager.GetInstance.IsLoaded);
        nfsm = new NeutralStateMachine(this, transform.GetComponentInChildren<Animator>(), NeutralStates.CitizenIdleState);
        stats.OnDead += DeadAnimator;
    }

    private void Update()
    {
        if (isMoving)
        {
            SequentialMove();
        }
    }

    protected override void CalculateBehaviour()
    {
        List<EntityStats> visibleTargets = DetectVisibleTargets();

        if (visibleTargets.Count > 0 && isDetection == false)
        {
            isDetection = true;
            CitizenWitness();
        }

        if (stats.secData.GetSecLevel >= 2)
        {
            Debug.Log("개쫄은상태");
            ChangeToCowerState();
        }

        else if (isDetection == true)//플레이어 발각시
        {
            Debug.Log("존나 튀는 상태");
            TaskManager.GetInstance.AddTurnBehaviour(new TurnTask(() => { Move(exitArea); }, 0f));
            nfsm.eta = 3;
            nfsm.ChangeState(nfsm.FindState(NeutralStates.CitizenFleeState));
        }

        else
        {
            Debug.Log("대기상태");
            ChangeToIdle();
        }

        base.CalculateBehaviour();
    }

    public void ChangeToIdle()
    {

    }

    public void ChangeToCowerState()
    {
        nfsm.ChangeState(nfsm.FindState(NeutralStates.CitizenCowerState));
    }

    public void ChangeToDead()
    {
        nfsm.ChangeState(nfsm.FindState(NeutralStates.CitizenDeadState));
    }

    public void DeadAnimator()
    {
        animator.Play("Dead_Fwd");
    }

    public void DestroyObject()
    {
        GameManager.GetInstance.NoneBattleTurn.RemoveStartPointer(TurnTypes.neutral, CalculateBehaviour);
        Destroy(gameObject);
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

        //if (!GameManager.GetInstance.GetNode(targetPos).isWalkable || GameManager.GetInstance.GetEntityAt(GameManager.GetInstance.GetNode(targetPos).GetCenter) != null)
        //{
        //    Debug.Log("갈 수 없는 곳이거나, 엔티티가 있다.");
        //    return;
        //}

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
                pathQueue.Enqueue((Vector3Int)step);
            }
            else
            {
                Debug.Log($"이동 도중 이동력 부족. {step} 여기서 멈춤");
                break;
            }
        }

        if (pathQueue.Count > 0)
        {
            float totalDistance = 0f;
            Vector3 lastPos = transform.position;
            foreach (var step in pathQueue)
            {
                totalDistance += Vector3.Distance(lastPos, step);
                lastPos = step;
            }

            if (agent == null)
                agent = GetComponent<NavMeshAgent>();

            if (agent.speed <= 0f)
                agent.speed = 2f;

            float eta = totalDistance / agent.speed;

            if (nfsm.currentState != null)
            {
                nfsm.Current.duration = eta;
                Debug.Log($"[ETA] {eta:F2}초 / 거리 {totalDistance:F2} / 속도 {agent.speed:F2}");
            }
            else
            {
                Debug.LogWarning("efsm.Current가 null입니다.");
            }

            //최종 이동 구현
            isMoving = true;
            canNextMove = true;
        }
        //else 
        //{
        //    Debug.LogWarning("pathQueue가 비어있어 ETA계산 불가");
        //}

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
        }

    }

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

}
