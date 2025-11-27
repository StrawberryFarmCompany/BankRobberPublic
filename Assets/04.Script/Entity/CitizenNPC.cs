using BuffDefine;
using DG.Tweening;
using NodeDefines;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;
public class CitizenNPC : NeutralNPC
{
    public bool isDetection = false;
    public bool isCowered = false;
    [SerializeField] private Vector3 exitArea;
    public float eta = 0f;

    Queue<Vector3Int> pathQueue = new Queue<Vector3Int>();
    private EnemySitePreviewer sitePreviewer;

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

    protected override void CalculateBehaviour()
    {
        List<EntityStats> visibleTargets = DetectVisibleTargets();

        if (visibleTargets.Count > 0 && isDetection == false)
        {
            isDetection = true;
            CitizenWitness();
        }

        if (stats.secData.GetSecLevel >= 3 && isCowered == false)
        {
            Debug.Log("겁먹은 상태");
            ChangeToCowerState();
            isCowered = true;
        }

        else if (isDetection == true && isCowered == false)//플레이어 발각시
        {
            Debug.Log("도망가는 상태");
            TaskManager.GetInstance.AddTurnBehaviour(new TurnTask(() => { Move(exitArea); }, 0f));
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
            isMoving = false;
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

        if (pathQueue.Count == 0)
        {
            //최종 이동 구현
            isMoving = false;
            return;
        }

        TurnTask task = new TurnTask(SequentialMove, GetPathTime(0.3f, 0.2f));
        task.Action += () => nfsm.ChangeState(nfsm.FindState(NeutralStates.CitizenFleeState));
        TaskManager.GetInstance.AddActionBehaviour(task);

        isMoving = true;
        canNextMove = true;
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
    /// 
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
        if (pathQueue.Count > 0)
        {
            Vector3Int targetPos = pathQueue.Dequeue();
            Node node = GameManager.GetInstance.GetNode(targetPos);
            if (node != null && node.Standing.Count > 0)
            {
                stats.HealMovement(pathQueue.Count + 1);
                pathQueue.Clear();
                stats.NodeUpdates(transform.position);
                stats.GetTileInteraction(transform.position);
                return;
            }
            else if (pathQueue.Count <= 1)
            {
                eta = DoMoveAndRotate(Ease.Unset, targetPos, 0.2f, 0.3f, () =>
                {
                    stats.NodeUpdates(transform.position);
                    sitePreviewer.SetMesh(stats.currNode.GetCenter, fovAngle * 0.5f, transform.eulerAngles.y, stats.attackRange);
                    stats.GetTileInteraction(transform.position);
                    SequentialMove();
                });
            }
            else
            {
                eta = DoMoveAndRotate(Ease.Unset, targetPos, 0.2f, 0.3f, () =>
                {
                    stats.NodeUpdates(transform.position);
                    sitePreviewer.SetMesh(stats.currNode.GetCenter, fovAngle * 0.5f, transform.eulerAngles.y, stats.attackRange);
                    stats.GetTileInteraction(transform.position);
                    SequentialMove();
                });
            }
        }
        else
        {
            isMoving = false;
            eta = 0f;
            nfsm.ChangeState(nfsm.FindState(NeutralStates.CitizenIdleState));

        }
    }

    private float GetPathTime(float moveDuration, float rotationDuration)
    {
        Queue<Vector3Int> copyQ = new Queue<Vector3Int>(pathQueue.ToArray());
        float time = 0f;
        Vector3 currPos = transform.position;
        float currRot = transform.eulerAngles.y;
        while (copyQ.Count > 0)
        {
            Vector3 nextPos = copyQ.Dequeue();
            Vector2 relPos = new Vector2(nextPos.x, nextPos.z) - new Vector2(currPos.x, currPos.z);
            float radian = Mathf.Atan2(relPos.x, relPos.y);
            float angle = (Mathf.Rad2Deg * radian);


            float minAngle = (Mathf.Min(angle, currRot) + 180) % 360f;
            float maxAngle = (Mathf.Max(angle, currRot) + 180) % 360f;

            currRot = angle;
            currPos = nextPos;

            float rotAngle = (maxAngle - minAngle) / 360f;
            float currRotDuration = rotationDuration;
            if (rotAngle == 0)
            {
                currRotDuration = 0;
            }
            else
            {
                float originRotDur = currRotDuration;
                currRotDuration = originRotDur * rotAngle;
                currRotDuration = MathF.Abs(currRotDuration);
            }
            time += currRotDuration + moveDuration;
        }

        return time;
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

    private float DoMoveAndRotate(Ease ease, Vector3Int pos, float moveDuration, float rotationDuration, Action action = null)
    {
        transform.DOComplete(true);

        Vector2 relPos = new Vector2(pos.x, pos.z) - new Vector2(transform.position.x, transform.position.z);
        float radian = Mathf.Atan2(relPos.x, relPos.y);
        float angle = (Mathf.Rad2Deg * radian);


        float minAngle = (Mathf.Min(angle, transform.eulerAngles.y) + 180) % 360f;
        float maxAngle = (Mathf.Max(angle, transform.eulerAngles.y) + 180) % 360f;

        float rotAngle = (maxAngle - minAngle) / 360f;
        if (rotAngle == 0)
        {
            rotationDuration = 0;
        }
        else
        {
            float originRotDur = rotationDuration;
            rotationDuration = originRotDur * rotAngle;
            rotationDuration = MathF.Abs(rotationDuration);
        }
        transform.DORotate(Vector3.up * angle, rotationDuration).OnComplete(() =>
        {
            transform.DOMove(pos, moveDuration).SetEase(ease).OnComplete(() =>
            {
                if (stats == null) return;
                action?.Invoke();
            });
        });
        return moveDuration + rotationDuration;
    }

}
