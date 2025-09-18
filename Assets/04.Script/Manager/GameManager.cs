using NodeDefines;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public enum GamePhase
{
    NoneBattle, // 잠입
    Battle      // 전투
}

class GameManager : SingleTon<GameManager>
{
    private Dictionary<Vector3Int, Node> nodes;
    public Dictionary<Vector3Int, Node> Nodes => nodes;
    private NoneBattleTurnStateMachine noneBattleTurn;
    public NoneBattleTurnStateMachine NoneBattleTurn { get { return noneBattleTurn; } }

    private NoneBattleTurnStateMachine battleTurn;
    public NoneBattleTurnStateMachine BattleTurn { get { return battleTurn; } }

    public BattleTurnStateMachine TurnMachine { get; private set; }

    public GamePhase CurrentPhase { get; private set; } = GamePhase.NoneBattle;

    private CharacterNumber currCharacter;
    public CharacterNumber CurrCharacter { get { return currCharacter; } set { currCharacter = value; } }
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
    public void RegistNode(Vector3Int vec, bool isWalkable = false)
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
    public void RegistEvent(Vector3 pos, Interaction a,string interactionName)
    {
        Vector3Int convertedPos = GetVecInt(pos);
        List<Vector3Int> vectors = GetNearNodes(pos);
        for (int i = 0; i < vectors.Count; i++)
        {
            if (nodes.TryGetValue(vectors[i], out Node node))
            {
                node.AddInteraction(a, interactionName);
            }
        }
    }
    public void RegistEvent(Vector3Int[] poses,Interaction a,string interactionName)
    {
        for (int i = 0; i < poses.Length; i++)
        {
            if (nodes.TryGetValue(poses[i], out Node node))
            {
                node.AddInteraction(a, interactionName);
            }
        }
    }
    public List<Vector3Int> GetNearNodes(Vector3 pos)
    {
        Vector3Int convertedPos = GetVecInt(pos);
        List<Vector3Int> poses = new List<Vector3Int>();
        poses.Add(convertedPos);
        for (int i = 0; i < nearNode.Length; i++)
        {
            if (nodes.ContainsKey(nearNode[i] + convertedPos))
            {
                poses.Add(nearNode[i] + convertedPos);
            }
        }
        return poses;
    }
    public void RemoveEvent(Vector3 pos, Interaction a,string interactionName)
    {
        Vector3Int convertedPos = GetVecInt(pos);
        for (int i = 0; i < nearNode.Length; i++)
        {
            if (nodes.TryGetValue(nearNode[i]+ convertedPos, out Node node))
            {
                node.RemoveInteraction(a, interactionName);
            }
        }
    }

    public void OnFirst(InputAction.CallbackContext context)
    {
        if(context.started && IsNoneBattlePhase())
            currCharacter = CharacterNumber.Character_1;
    }

    public void OnSecond(InputAction.CallbackContext context)
    {
        if (context.started && IsNoneBattlePhase())
            currCharacter = CharacterNumber.Character_2;
    }
    public void OnThird(InputAction.CallbackContext context)
    {
        if (context.started && IsNoneBattlePhase())
            currCharacter = CharacterNumber.Character_3;

    }
    public void EndTurn()
    {
        noneBattleTurn.ChangeState(noneBattleTurn.FindState(TurnTypes.enemy));
    }

    public bool IsNoneBattlePhase()
    {
        return CurrentPhase == GamePhase.NoneBattle;
    }
}
