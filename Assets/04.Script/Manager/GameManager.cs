using NodeDefines;
using System.Collections.Generic;
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

    private bool playerTurn;
    public bool PlayerTurn { get { return playerTurn; } set { playerTurn = value; } } //현재 플레이어의 턴인가?

    // 캐릭터 턴 종료 여부 대신 → NodePlayerController 내부 상태로 관리
    // private bool isFirstCharacterEnd;
    // private bool isSecondCharacterEnd;
    // private bool isThirdCharacterEnd;


    //현재 팔방, 추후 4방이면 4방으로 바꿔야함
    private readonly Vector3Int[] nearNode = new Vector3Int[8]
    {
        Vector3Int.forward, Vector3Int.right, Vector3Int.back, Vector3Int.left,
        new Vector3Int(-1, 0, -1), new Vector3Int(1, 0, 1),
        new Vector3Int(-1, 0, 1), new Vector3Int(1, 0, -1)
    };

    public bool isPlayerGeyKeyCard;

    protected override void Init()
    {
        base.Init();
        nodes = new Dictionary<Vector3Int, Node>();
        noneBattleTurn = new NoneBattleTurnStateMachine();
        isPlayerGeyKeyCard = false;
    }

    protected override void Reset()
    {
        base.Reset();
        nodes.Clear();
        noneBattleTurn = null;
        noneBattleTurn = new NoneBattleTurnStateMachine();
        isPlayerGeyKeyCard = false;
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

    public void RegistEvent(Vector3 pos, Interaction a, string interactionName)
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

    public void RegistEvent(Vector3Int[] poses, Interaction a, string interactionName)
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

    public void RemoveEvent(Vector3 pos, Interaction a, string interactionName)
    {
        Vector3Int convertedPos = GetVecInt(pos);
        for (int i = 0; i < nearNode.Length; i++)
        {
            if (nodes.TryGetValue(nearNode[i] + convertedPos, out Node node))
            {
                node.RemoveInteraction(a, interactionName);
            }
        }
    }

    public List<Vector3Int> GetNearNodes(Vector3Int convertedPos)
    {
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

    // 삭제: OnFirst/OnSecond/OnThird → NodePlayerManager에서 인덱스로 전환
    /*
    public void OnFirst(InputAction.CallbackContext context) { ... }
    public void OnSecond(InputAction.CallbackContext context) { ... }
    public void OnThird(InputAction.CallbackContext context) { ... }
    */

    public void StartPlayerTurn()
    {
        if (IsNoneBattlePhase())
        {
            noneBattleTurn.ChangeState(noneBattleTurn.FindState(TurnTypes.ally));

            // NodePlayerManager에서 모든 플레이어 초기화
            foreach (var player in NodePlayerManager.GetInstance.GetAllPlayers())
            {
                player.playerCondition.ResetForNewTurn();
            }
            NodePlayerManager.GetInstance.SwitchToPlayer(0);
        }
        else
        {
            battleTurn.ChangeState(battleTurn.FindState(TurnTypes.ally));

            foreach (var player in NodePlayerManager.GetInstance.GetAllPlayers())
            {
                player.playerCondition.ResetForNewTurn();
            }
            NodePlayerManager.GetInstance.SwitchToPlayer(0);
        }
    }

    public void EndPlayerTurn()
    {
        if (IsNoneBattlePhase())
            noneBattleTurn.ChangeState(noneBattleTurn.FindState(TurnTypes.enemy));
        else
            battleTurn.ChangeState(battleTurn.FindState(TurnTypes.enemy));
    }

    public bool IsNoneBattlePhase()
    {
        return CurrentPhase == GamePhase.NoneBattle;
    }


    public void CheckAllCharacterEndTurn()
    {

        foreach (var player in NodePlayerManager.GetInstance.GetAllPlayers())
        {
            if (!player.isEndReady)
                return;
        } 

        Debug.Log($"다 끝나고 플레이어 턴 엔드");

        EndPlayerTurn();

        foreach (var player in NodePlayerManager.GetInstance.GetAllPlayers())
        {
            player.isEndReady = false;
        }
    }
}
