using NodeDefines;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

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

    private CharacterNumber currCharacter;
    public CharacterNumber CurrCharacter { get { return currCharacter; } set { currCharacter = value; } } //현재 조작중인 캐릭터

    private bool playerTurn;
    public bool PlayerTurn { get { return playerTurn; } set { playerTurn = value; } } //현재 플레이어의 턴인가?

    private bool isFirstCharacterEnd;
    public bool IsFirstCharacterEnd { get { return isFirstCharacterEnd; } set { isFirstCharacterEnd = value; } }
    private bool isSecondCharacterEnd;
    public bool IsSecondCharacterEnd { get { return isSecondCharacterEnd; } set { isSecondCharacterEnd = value; } }
    private bool isThirdCharacterEnd;
    public bool IsThirdCharacterEnd { get { return isThirdCharacterEnd; } set { isThirdCharacterEnd = value; } }

    //현재 팔방, 추후 4방이면 4방으로 바꿔야함
    private readonly Vector3Int[] nearNode = new Vector3Int[8] { Vector3Int.forward, Vector3Int.right, Vector3Int.back, Vector3Int.left, new Vector3Int(-1, 0, -1), new Vector3Int(1, 0, 1), new Vector3Int(-1, 0, 1), new Vector3Int(1, 0, -1) };
    public List<bool> isPlayerGetKeyCard = new List<bool>();
    public int endTurnCount = 0;

    private readonly Dictionary<CharacterNumber, NodePlayerController> _actors = new();
    public NodePlayerController CurrentActor { get; private set; }
    public PlayerStats CurrentStats => CurrentActor != null ? CurrentActor.playerStats : null;

    public void RegisterActor(NodePlayerController actor)
    {
        _actors[actor.characterNumber] = actor;
        if (CurrentActor == null && actor.characterNumber == CurrCharacter)
            SetCurrentCharacter(CurrCharacter);
    }

    public void UnregisterActor(NodePlayerController actor)
    {
        if (_actors.TryGetValue(actor.characterNumber, out var cur) && cur == actor)
            _actors.Remove(actor.characterNumber);
        if (CurrentActor == actor) CurrentActor = null;
    }

    public void SetCurrentCharacter(CharacterNumber num)
    {
        CurrCharacter = num;
        _actors.TryGetValue(num, out var actor);
        CurrentActor = actor;
    }

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
        isPlayerGetKeyCard = null;
        isPlayerGetKeyCard = new List<bool>();
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
    public void OnFirst(InputAction.CallbackContext context)
    {
        if(context.started && IsNoneBattlePhase())
            SetCurrentCharacter(CharacterNumber.Character_1);
    }

    public void OnSecond(InputAction.CallbackContext context)
    {
        if (context.started && IsNoneBattlePhase())
            SetCurrentCharacter(CharacterNumber.Character_2);
    }
    public void OnThird(InputAction.CallbackContext context)
    {
        if (context.started && IsNoneBattlePhase())
            SetCurrentCharacter(CharacterNumber.Character_3);

    }

    public void StartPlayerTurn()
    {
        if (IsNoneBattlePhase())
        {
            noneBattleTurn.ChangeState(noneBattleTurn.FindState(TurnTypes.allay));
            isFirstCharacterEnd = false;
            isSecondCharacterEnd = false;
            isThirdCharacterEnd = false;
            SetCurrentCharacter(CharacterNumber.Character_1);
        }
        else if (!IsNoneBattlePhase())
        {
            battleTurn.ChangeState();

            isFirstCharacterEnd = false;
            isSecondCharacterEnd = true;
            isThirdCharacterEnd = true;
            SetCurrentCharacter(CharacterNumber.Character_1);
        }
    }

    public void EndPlayerTurn()
    {
        if (IsNoneBattlePhase())
            noneBattleTurn.ChangeState(noneBattleTurn.FindState(TurnTypes.enemy));
        else if (!IsNoneBattlePhase())
            battleTurn.ChangeState();
    }

    public bool IsNoneBattlePhase()
    {
        return CurrentPhase == GamePhase.NoneBattle;
    }

    /// <summary>
    /// 인자 캐릭터의 활동 가능 조건을 false로 바꿔주고, 다음 캐릭터로 턴을 넘기거나, 모든 캐릭터가 턴을 종료했는지 확인. (잠입, 배틀 페이즈 알아서 처리)
    /// </summary>
    /// <param name="characterNumber"></param>
    public void EndCharacterTurn(CharacterNumber characterNumber)
    {
        var next = CurrCharacter;

        if (IsNoneBattlePhase())
        {
            switch (characterNumber)
            {
                case CharacterNumber.Character_1:
                    isFirstCharacterEnd = true;
                    if(!isSecondCharacterEnd)
                        next = CharacterNumber.Character_2;
                    else if (!isThirdCharacterEnd)
                        next = CharacterNumber.Character_3;
                    endTurnCount++;
                    break;
                case CharacterNumber.Character_2:
                    isSecondCharacterEnd = true;
                    if(!isFirstCharacterEnd)
                        next = CharacterNumber.Character_1;
                    else if (!isThirdCharacterEnd)
                        next = CharacterNumber.Character_3;
                    endTurnCount++;
                    break;
                case CharacterNumber.Character_3:
                    isThirdCharacterEnd = true;
                    if(!isFirstCharacterEnd)
                        next = CharacterNumber.Character_1;
                    else if (!isSecondCharacterEnd)
                        next = CharacterNumber.Character_2;
                    endTurnCount++;
                    break;
            }
        }
        else if (!IsNoneBattlePhase())
        {
            switch (characterNumber)
            {
                case CharacterNumber.Character_1:
                    isFirstCharacterEnd = true;
                    isSecondCharacterEnd = false;
                    next = CharacterNumber.Character_2;
                    endTurnCount++;
                    break;
                case CharacterNumber.Character_2:
                    isSecondCharacterEnd = true;
                    isThirdCharacterEnd = false;
                    next = CharacterNumber.Character_3;
                    endTurnCount++;
                    break;
                case CharacterNumber.Character_3:
                    isThirdCharacterEnd = true;
                    endTurnCount++;
                    break;
            }
        }

        SetCurrentCharacter(next);
        CheckAllCharacterEndTurn();

    }

    /// <summary>
    /// 앤드갯수 충족 시 플레이어 턴 종료
    /// </summary>
    public void CheckAllCharacterEndTurn()
    {
        if (endTurnCount >= System.Enum.GetValues(typeof(CharacterNumber)).Length)
        {
            //플레이어 턴 종료 로직 필요
            EndPlayerTurn();
            endTurnCount = 0;
        }
    }

    public bool IsCharacterTurn(CharacterNumber characterNumber)
    {
        switch(characterNumber)
        {
            case CharacterNumber.Character_1:
                return isFirstCharacterEnd;
            case CharacterNumber.Character_2:
                return isSecondCharacterEnd;
            case CharacterNumber.Character_3:
                return isThirdCharacterEnd;
            default:
                return false;
                break;
        }
    }
}
