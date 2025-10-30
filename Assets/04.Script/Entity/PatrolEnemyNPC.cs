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

    [SerializeField] private Vector3 homeLocation;
    [SerializeField] private Vector3 firstLocation;
    [SerializeField] private Vector3 noiseLocation;

    private float eta;
    public NavMeshAgent agent;
    Queue<Vector3Int> pathQueue = new Queue<Vector3Int>();
    Vector3Int curTargetPos;
    bool isMoving;
    bool canNextMove;
    Animator animator;

    protected override IEnumerator Start()
    {
        animator = GetComponent<Animator>();
        StartCoroutine(base.Start());
        yield return new WaitUntil(() => ResourceManager.GetInstance.IsLoaded);
        // 상태머신 초기화 (기본 상태)
        efsm = new EnemyStateMachine(this, transform.GetComponentInChildren<Animator>() ,EnemyStates.PatrolEnemyIdleRotationState);
        stats.OnDead += DeadAnimator;
    }
    
    private void Update()
    {
        if (isMoving)
        {
            SequentialMove();
        }
    }

    // 턴마다 실행될 매서드
    protected override void CalculateBehaviour()
    {
        DetectNoise();
        DetectVisibleTargets();

        if (stats.secData.GetSecLevel == 0)
        {
            if (isNoise == true && isArrivedNoisePlace == false)
            {
                TaskManager.GetInstance.AddTurnBehaviour(new TurnTask(() => { Move(noiseLocation); }, 0f));
                efsm.eta = 3;
                efsm.ChangeState(efsm.FindState(EnemyStates.PatrolEnemyInvestigateState));

                if (this.gameObject.transform.position == noiseLocation)
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

            else if (departurePoint == true && destinationPoint == false)
            {
                TaskManager.GetInstance.AddTurnBehaviour(new TurnTask(() => { Move(firstLocation); },0f));
                efsm.eta = 3;
                efsm.ChangeState(efsm.FindState(EnemyStates.PatrolEnemyPatrolState));
                
                //이 부분 코루틴 같은걸로 시간 줘야 아래 이프문이 돌아갈 것 같음
                if (this.gameObject.transform.position == firstLocation)
                {
                    LookAround();
                    departurePoint = false;     // 출발 지점 true, false로 계속 바꿔주기
                    destinationPoint = true;    // 도착 지점 true, false로 계속 바꿔주기
                }
            }

            else if (departurePoint == false && destinationPoint == true)
            {
                TaskManager.GetInstance.AddTurnBehaviour(new TurnTask(() => { Move(firstLocation); }, 0f));
                efsm.eta = 3;
                efsm.ChangeState(efsm.FindState(EnemyStates.PatrolEnemyPatrolState));

                if (this.gameObject.transform.position == homeLocation)
                {
                    LookAround();
                    departurePoint = true;      // 출발 지점 true, false로 계속 바꿔주기
                    destinationPoint = false;   // 도착 지점 true, false로 계속 바꿔주기
                }
            }
        }

        else if(stats.secData.GetSecLevel >= 1)
        {
            DetectVisibleTargets();
            if (nearPlayerLocation.currNode.GetCenter != null)
            {
                transform.LookAt(nearPlayerLocation.currNode.GetCenter);
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

    public void OnDrawGizmos()
    {
        Gizmos.DrawCube(homeLocation, Vector3.one);
        Gizmos.color = Color.yellow;
        Gizmos.DrawCube(firstLocation, Vector3.one);
        Gizmos.color = Color.red;
    }
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

            if (efsm.currentState != null)
            {
                efsm.Current.duration = eta;
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
            if (nearPlayerLocation != null)
            {
                transform.LookAt(nearPlayerLocation.currNode.GetCenter);
            }
            TryAttack();
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

    protected override void OnNoiseDetected(Vector3 noisePos)
    {
        isNoise = true;
        isArrivedNoisePlace = false;
        noiseLocation = noisePos;
    }
}