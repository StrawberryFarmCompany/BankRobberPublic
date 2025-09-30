using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class CopEnemyNPC : EnemyNPC
{
    bool isDetection = false;
    bool isHit = false;

    [SerializeField] Vector3 nearPlayerLocation;

    public NavMeshAgent agent;
    Queue<Vector3Int> pathQueue = new Queue<Vector3Int>();
    Vector3Int curTargetPos;
    bool isMoving;
    bool canNextMove;

    protected override IEnumerator Start()
    {
        StartCoroutine(base.Start());
        yield return new WaitUntil(() => ResourceManager.GetInstance.IsLoaded);
        efsm = new EnemyStateMachine(this, EnemyStates.CopEnemyChaseState);
        yield return null;
    }

    private void Update()
    {
        if (isMoving)
        {
            SequentialMove();
        }
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    protected override void CalculateBehaviour()
    {
        TryAttack();
        Debug.Log("죽어잇!");

        // 공격이 실패했거나 이동력이 남았으면 추적
        if (stats.movement > 0)
        {
            efsm.ChangeState(efsm.FindState(EnemyStates.CopEnemyChaseState));
            Move(nearPlayerLocation);
        }
        base.CalculateBehaviour();
    }

    //public void ChangeToChase()
    //{
    //    efsm.ChangeState(efsm.FindState(EnemyStates.CopEnemyChaseState));
    //}

    //public void ChangeToCombat()
    //{
    //    efsm.ChangeState(efsm.FindState(EnemyStates.CopEnemyCombatState));
    //}

    //public void ChangeToDamaged()
    //{
    //    efsm.ChangeState(efsm.FindState(EnemyStates.CopEnemyDamagedState));
    //}

    //public void ChangeToDead()
    //{
    //    efsm.ChangeState(efsm.FindState(EnemyStates.CopEnemyDeadState));
    //}

    public void Move(Vector3 pos)
    {
        if (isMoving) return;
        Vector3Int targetPos = GameManager.GetInstance.GetVecInt(pos);

        if (GameManager.GetInstance.GetNode(targetPos) == null)
        {
            Debug.Log("노드가 아니다.");
            return;
        }

        if (!GameManager.GetInstance.GetNode(targetPos).isWalkable || GameManager.GetInstance.GetEntityAt(GameManager.GetInstance.GetNode(targetPos).GetCenter) != null)
        {
            Debug.Log("갈 수 없는 곳이거나, 엔티티가 있다.");
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
            //최종 이동 구현
            isMoving = true;
            canNextMove = true;
        }

    }

    private List<Vector3Int> GenerateChebyshevPath(Vector3Int start, Vector3Int end)
    {
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
                if (GameManager.GetInstance.GetEntityAt(next) != null) continue;

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

}
