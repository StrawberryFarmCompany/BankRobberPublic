using NodeDefines;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.Runtime.CompilerServices;

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

    private BattleTurnStateMachine battleTurn;
    public BattleTurnStateMachine BattleTurn { get { return battleTurn; } }

    public GamePhase CurrentPhase { get; private set; } = GamePhase.NoneBattle;

    private bool playerTurn;
    public bool PlayerTurn { get { return playerTurn; } set { playerTurn = value; } } //현재 플레이어의 턴인가?


    //현재 팔방, 추후 4방이면 4방으로 바꿔야함
    public readonly Vector3Int[] nearNode = new Vector3Int[24] { 
        /*동일층*/Vector3Int.forward, Vector3Int.right, Vector3Int.back, Vector3Int.left, new Vector3Int(-1, 0, -1), new Vector3Int(1, 0, 1), new Vector3Int(-1, 0, 1), new Vector3Int(1, 0, -1),
        /*-1층*/new Vector3Int(0,-1,1), new Vector3Int(1,-1,0), new Vector3Int(0,-1,-1), new Vector3Int(-1,-1,0), new Vector3Int(-1, -1, -1), new Vector3Int(1, -1, 1), new Vector3Int(-1, -1, 1), new Vector3Int(1, -1, -1),
        new Vector3Int(0,1,1), new Vector3Int(1,1,0), new Vector3Int(0,1,-1), new Vector3Int(-1,1,0), new Vector3Int(-1, 1, -1), new Vector3Int(1, 1, 1), new Vector3Int(-1, 1, 1), new Vector3Int(1, 1, -1)
    };
    public List<bool> isPlayerGetKeyCard = new List<bool>();
    public int endTurnCount = 0;

    //private readonly Dictionary<CharacterNumber, NodePlayerController> _actors = new();
    public NodePlayerController CurrentActor { get; private set; }
    public EntityStats CurrentStats => CurrentActor != null ? CurrentActor.playerStats : null;

    //public void RegisterActor(NodePlayerController actor)
    //{
    //    _actors[actor.characterNumber] = actor;
    //    if (CurrentActor == null && actor.characterNumber == CurrCharacter)
    //        SetCurrentCharacter(CurrCharacter);
    //}

    //public void UnregisterActor(NodePlayerController actor)
    //{
    //    if (_actors.TryGetValue(actor.characterNumber, out var cur) && cur == actor)
    //        _actors.Remove(actor.characterNumber);
    //    if (CurrentActor == actor) CurrentActor = null;
    //}

    //public void SetCurrentCharacter(CharacterNumber num)
    //{
    //    CurrCharacter = num;
    //    _actors.TryGetValue(num, out var actor);
    //    CurrentActor = actor;
    //}

    // 
    private List<EntityStats> entities = new List<EntityStats>();
    public void SetGamePhase(GamePhase phase)
    {
        CurrentPhase = phase;
        //추후 해당 페이즈시 추가
        switch (phase)
        {
            case GamePhase.NoneBattle:
                break;
            case GamePhase.Battle:
                break;
            default:
                break;
        }
    }

    protected override void Init()
    {
        base.Init();
        nodes = new Dictionary<Vector3Int, Node>();
        noneBattleTurn = new NoneBattleTurnStateMachine();
        //noneBattleTurn.AddStartPointer(TurnTypes.ally, StartPlayerTurn);
        battleTurn = new BattleTurnStateMachine();
    }

    protected override void Reset()
    {
        base.Reset();
        SecurityData.Reset();
        OnNodeReset();
        noneBattleTurn.OnSceneChange();
        OnEntityReset();
        battleTurn = new BattleTurnStateMachine();
        isPlayerGetKeyCard = null;
        isPlayerGetKeyCard = new List<bool>();
    }
    public void OnEntityReset()
    {
        if (entities == null)
        {
            entities = new List<EntityStats>();
            return;
        }
        for (int i = 0; i<entities.Count; i++)
        {
            entities[i].OnDamaged = null;
            entities[i].ForceMove = null;
        }
        entities.Clear();
    }
    public void OnNodeReset()
    {
        if(nodes == null)
        {
            nodes = new Dictionary<Vector3Int, Node>();
            return;
        }
        foreach (Node item in nodes.Values)
        {
            item.ResetEvent();
            item.ResetInteraction();
        }
        nodes.Clear();
    }
    public void RegistNode(Vector3Int vec, bool isWalkable = false)
    {
        if (nodes == null) nodes = new Dictionary<Vector3Int, Node>();
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
            noneBattleTurn.ForceSet((int)TurnTypes.ally);

            // NodePlayerManager에서 모든 플레이어 초기화
            foreach (var player in NodePlayerManager.GetInstance.GetAllPlayers())
            {
                player.playerStats.ResetForNewTurn();
            }
            NodePlayerManager.GetInstance.SwitchToPlayer(0);
        }
        else
        {
            battleTurn.ChangeState();
            endTurnCount = 0;

            foreach (var player in NodePlayerManager.GetInstance.GetAllPlayers())
            {
                player.playerStats.ResetForNewTurn();
            }
            NodePlayerManager.GetInstance.SwitchToPlayer(0);
        }
    }

    public bool IsNoneBattlePhase()
    {
        return CurrentPhase == GamePhase.NoneBattle;
    }


    public void CheckAllCharacterEndTurn()
    {
        List<NodePlayerController> players = NodePlayerManager.GetInstance.GetAllPlayers();
        for (int i = 0; i < players.Count; i++)
        {
            if (!players[i].isEndReady) return;
        }

        Debug.Log($"다 끝나고 플레이어 턴 엔드");
        for (int i = 0; i < players.Count; i++)
        {
            players[i].isEndReady = false;
        }

        EndPlayerTurn();

    }

    public void EndPlayerTurn()
    {
        if (IsNoneBattlePhase())
            noneBattleTurn.ChangeState();
        else
            battleTurn.ChangeState();
    }



    public void RegisterEntity(EntityStats entity)
    {
        if (!entities.Contains(entity))
            entities.Add(entity);
    }

    public void UnregisterEntity(EntityStats entity)
    {
        if (entities.Contains(entity))
            entities.Remove(entity);
    }

    // 특정 좌표에 있는 엔티티 찾기
    public EntityStats GetEntityAt(Vector3Int pos)
    {
        if (nodes.ContainsKey(pos))
        {
            Node currNode = nodes[pos];
            for (int i = 0; i < currNode.Standing.Count; i++)
            {
                if (currNode.Standing[i] != null) return currNode.Standing[i];
            }
        }
        return null;
    }

    // 범위 내 엔티티들 반환 (예: 스킬 범위 공격)
    public List<EntityStats> GetEntitiesInRange(Vector3Int center, int range)
    {
        List<EntityStats> result = new List<EntityStats>();
        foreach (var e in entities)
        {
            int dist = Mathf.Abs(center.x - e.currNode.GetCenter.x) +
                       Mathf.Abs(center.z - e.currNode.GetCenter.z);
            if (dist <= range)
            {
                result.Add(e);
                Debug.Log("result.Add");
            }
        }
        return result;
    }

    public void GameEnd()
    {
        //Reset();
        UIManager.GetInstance.gameEndUI.TurnOnPanel();
        if (NodePlayerManager.GetInstance.GetEscapeSuccess() == GameResult.Perfect)
        {
            UIManager.GetInstance.gameEndUI.SetPerfect();
        }
        else if (NodePlayerManager.GetInstance.GetEscapeSuccess() == GameResult.Failed)
        {
            UIManager.GetInstance.gameEndUI.SetFail();
        }
        else
        {
            UIManager.GetInstance.gameEndUI.SetSuccess();
        }
        Debug.Log("게임 끝");
    }
}
