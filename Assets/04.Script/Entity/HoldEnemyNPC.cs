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

    public NavMeshAgent agent;
    Queue<Vector3Int> pathQueue = new Queue<Vector3Int>();
    Vector3Int curTargetPos;
    bool isMoving;
    bool canNextMove;

    protected override IEnumerator Start()
    {
        StartCoroutine(base.Start());
        yield return new WaitUntil(() => ResourceManager.GetInstance.IsLoaded);
        efsm = new EnemyStateMachine(this,transform.GetComponentInChildren<Animator>(), EnemyStates.HoldEnemyIdleState);
    }

    private void Update()
    {
        if (isMoving)
            SequentialMove();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    protected override void CalculateBehaviour()
    {
        DetectNoise();

        DetectVisibleTargets();

        if (stats.secData.GetSecLevel == 0)
        {

            if (isNoise == false && isHomePlace == true) //소음 감지가 false라면
            {
                efsm.ChangeState(efsm.FindState(EnemyStates.HoldEnemyIdleState)); //대기 상태
            }

            else if (isNoise == false && isHomePlace == false)
            {
                efsm.ChangeState(efsm.FindState(EnemyStates.HoldEnemyMoveReturnState));
                Move(homeLocation);
                if(this.gameObject.transform.position == homeLocation)
                {
                    isHomePlace = true;
                }
            }

            else if (isNoise == true && isNoisePlace == false)
            {
                efsm.ChangeState(efsm.FindState(EnemyStates.HoldEnemyInvestigateState));
                Move(noiseLocation);//소음 재 감지시 외부에서 isNoise를 true로 만들어주기
                if (this.gameObject.transform.position == noiseLocation)
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

        else if (stats.secData.GetSecLevel >= 2)
        {
            efsm.ChangeState(efsm.FindState(EnemyStates.HoldEnemyCombatState));

            DetectVisibleTargets();

            transform.LookAt(nearPlayerLocation.currNode.GetCenter);

            TryAttack();

            //공격이 실패했거나 행동력이 남았으면 추적 후 공격
            if (stats.curActionPoint > 0)
            {
                efsm.ChangeState(efsm.FindState(EnemyStates.HoldEnemyChaseState));
                if (nearPlayerLocation != null)
                {
                    Move(nearPlayerLocation.GetPosition());
                }

                else
                {
                    Debug.LogError($"플레이어 로케이션이 지정되지 않았습니다 : {gameObject.name}");
                }

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
                transform.LookAt(nearPlayerLocation.currNode.GetCenter);
            }
            TryAttack();
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

    private void DetectNoise()
    {
        var noises = NoiseManager.GetActiveNoises();
        if (noises == null || noises.Count == 0) return;

        foreach (var noise in noises)
        {
            float distance = Vector3.Distance(transform.position, noise.pos);

            // 🔸 감지 반경 내에 들어왔다면
            if (distance <= noise.radius)
            {
                // 이미 감지 상태라면 무시
                if (isNoise) return;

                isNoise = true;
                noiseLocation = noise.pos;

                Debug.Log($"{gameObject.name} 가 {noise.pos}에서 소음 감지함.");
                break;
            }
        }
    }
}