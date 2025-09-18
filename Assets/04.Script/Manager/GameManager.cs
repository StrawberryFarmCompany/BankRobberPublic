using NodeDefines;
using System.Collections.Generic;
using UnityEngine;

class GameManager : SingleTon<GameManager>
{
    private Dictionary<Vector3Int, Node> nodes;
    public Dictionary<Vector3Int, Node> Nodes => nodes;
    private NoneBattleTurnStateMachine noneBattleTurn;
    public NoneBattleTurnStateMachine NoneBattleTurn { get { return noneBattleTurn; } }

    private NoneBattleTurnStateMachine battleTurn;
    public NoneBattleTurnStateMachine BattleTurn { get { return battleTurn; } }
    //현재 팔방, 추후 4방이면 4방으로 바꿔야함
    private readonly Vector3Int[] nearNode = new Vector3Int[8] { Vector3Int.forward, Vector3Int.right, Vector3Int.back, Vector3Int.left, new Vector3Int(-1, 0, -1), new Vector3Int(1, 0, 1), new Vector3Int(-1, 0, 1), new Vector3Int(1, 0, -1) };
    protected override void Init()
    {
        base.Init();
        nodes = new Dictionary<Vector3Int, Node>();
        noneBattleTurn = new NoneBattleTurnStateMachine();
    }
    protected override void Reset()
    {
        base.Reset();
        nodes.Clear();
        noneBattleTurn = null;
        noneBattleTurn = new NoneBattleTurnStateMachine();
    }
    public void RegistNode(Vector3Int vec, bool isWalkable)
    {
        nodes.TryAdd(vec, new Node(vec, isWalkable));
    }
    public Node GetNode(Vector3 pos)
    {
        nodes.TryGetValue(GetVecInt(pos), out Node result);
        return result;
    }
    public Vector3Int GetVecInt(Vector3 pos)
    {
        float x = pos.x % 1f;
        float z = pos.z % 1f;
        return new Vector3Int(
            x < 0f ? (x <= -0.5f ? Mathf.FloorToInt(pos.x) : Mathf.CeilToInt(pos.x)) : (x <= 0.5f ? Mathf.FloorToInt(pos.x) : Mathf.CeilToInt(pos.x)),
            Mathf.FloorToInt(pos.y),
            z < 0f ? (z <= -0.5f ? Mathf.FloorToInt(pos.z) : Mathf.CeilToInt(pos.z)) : (z <= 0.5f ? Mathf.FloorToInt(pos.z) : Mathf.CeilToInt(pos.z)));
    }
    public bool IsExistNode(Vector3Int vec)
    {
        return nodes.ContainsKey(vec);
    }
    public void RegistEvent(Vector3 pos, Interaction a)
    {
        for (int i = 0; i < nearNode.Length; i++)
        {
            if (nodes.TryGetValue(nearNode[i], out Node node))
            {
                node.AddInteraction(a);
            }
        }
    }

    private readonly List<Transform> enemies = new List<Transform>();

    public void RegisterEnemy(Transform t)
    {
        if (t != null && !enemies.Contains(t))
            enemies.Add(t);
    }

    public void UnregisterEnemy(Transform t)
    {
        if (t != null)
            enemies.Remove(t);
    }

    public IReadOnlyList<Transform> GetEnemies() => enemies;

    // 타일 간 원형 거리(사거리/타게팅용)
    public float TileDistance(Vector3Int a, Vector3Int b)
    {
        int dx = a.x - b.x;
        int dz = a.z - b.z;
        return Mathf.Sqrt(dx * dx + dz * dz);
    }

    public bool HasLineOfSight(Vector3Int from, Vector3Int to, bool allowEndBlocked = true)
    {
        var start = new Vector3(from.x, 1.2f, from.z);
        var end = new Vector3(to.x, 1.2f, to.z);

        if (Physics.Linecast(start, end, out var hit))
        {
            if (allowEndBlocked)
            {
                var hx = Mathf.RoundToInt(hit.point.x);
                var hz = Mathf.RoundToInt(hit.point.z);
                if (hx == to.x && hz == to.z) return true;
            }
            return false;
        }
        return true;
    }
}
