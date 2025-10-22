using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public enum EscapeCondition
{
    None,       // 현재 게임에 참가 안 함
    Heisting,   // 습격중
    Arrest,      // 잡힘
    Escape,     // 탈출
    SuccessHeist //돈 가방 가지고 탈출
}

public enum GameResult
{
    Failed,
    Succeeded,
    Perfect
}

public class NodePlayerManager : MonoBehaviour
{

    public static NodePlayerManager GetInstance { get; private set; }

    [SerializeField] private List<NodePlayerController> players = new List<NodePlayerController>();
    public int currentPlayerIndex = 0;

    private int heistMemberCount;
    public EscapeCondition isEscapeBishop;
    public EscapeCondition isEscapeRook;
    public EscapeCondition isEscapeKnight;

    private void Awake()
    {
        if (GetInstance == null)
        {
            GetInstance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 플레이어 리스트 자동 등록 (씬에 배치된 모든 NodePlayerController)
    private void Start()
    {
        EscapeConditionReset();
        UIManager.GetInstance.SetCharacterResultUI(players);
        UIManager.GetInstance.TurnOffGameEndPanel();
        UIManager.GetInstance.pip.HideAndSneakText();

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            Debug.Log(GetCurrentPlayer()?.name);
        }
    }

    public void PlayerTurnReset()
    {
        for (int i = 0; i < players.Count; i++)
        {
            players[i].isEndReady = false;
        }
    }

    /// <summary>
    /// 현재 조종 중인 플레이어 반환
    /// </summary>
    public NodePlayerController GetCurrentPlayer()
    {
        if (players.Count == 0) return null;
        return players[currentPlayerIndex];
    }

    /// <summary>
    /// 다음 플레이어로 전환 및 캐릭터 엔드 대기
    /// </summary>
    public void SwitchToNextPlayer()
    {
        if (UIManager.GetInstance != null && UIManager.GetInstance.SelectionLocked) return;
        if (players.Count == 0)
            return;

        players[currentPlayerIndex].playerInput.DeactivateInput();
        currentPlayerIndex = (currentPlayerIndex + 1) % players.Count;
        players[currentPlayerIndex].TurnOnHighlighter();
        CameraManager.GetInstance.SwitchToPlayerCamera(GetCurrentPlayer().gameObject);
        players[currentPlayerIndex].playerInput.ActivateInput();
        UIManager.GetInstance.pip.HideAndSneakText();
        UIManager.GetInstance.leftInteractionPanel.OnInteractionRefresh();
    }

    /// <summary>
    /// 특정 플레이어로 전환
    /// </summary>
    public void SwitchToPlayer(int index)
    {
        if (UIManager.GetInstance != null && UIManager.GetInstance.SelectionLocked) return;
        if (index < 0 || index >= players.Count) return;
        players[currentPlayerIndex].playerInput.DeactivateInput();
        currentPlayerIndex = index;
        players[currentPlayerIndex].TurnOnHighlighter();
        CameraManager.GetInstance.SwitchToPlayerCamera(GetCurrentPlayer().gameObject);
        players[currentPlayerIndex].playerInput.ActivateInput();
        GetCurrentPlayer().isEndReady = false;
        UIManager.GetInstance.pip.HideAndSneakText();
        UIManager.GetInstance.leftInteractionPanel.OnInteractionRefresh();
    }

    public void OnFirst(InputAction.CallbackContext context)
    {
        if (context.started && GameManager.GetInstance.IsNoneBattlePhase())
            SwitchToPlayer(0);
    }

    public void OnSecond(InputAction.CallbackContext context)
    {
        if (context.started && GameManager.GetInstance.IsNoneBattlePhase())
            SwitchToPlayer(1);
    }
    public void OnThird(InputAction.CallbackContext context)
    {
        if (context.started && GameManager.GetInstance.IsNoneBattlePhase())
            SwitchToPlayer(2);
    }

    public void OnEndTurn(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            UIManager.GetInstance.ShowActionPanel(true);
            NotifyPlayerEndTurn(GetCurrentPlayer());
        }
    }

    /// <summary>
    /// 플레이어 등록
    /// </summary>
    public void RegisterPlayer(NodePlayerController player)
    {
        if (!players.Contains(player))
        {
            players.Add(player);
        }
    }

    /// <summary>
    /// 플레이어 제거 및 다음 플레이어로 전환 처리
    /// </summary>
    public void UnregisterPlayer(NodePlayerController player)
    {
        if (players.Contains(player))
        {
            int removedIndex = players.IndexOf(player);
            players.Remove(player);

            Debug.Log($"남은 플레이어 : {players.Count}");

            // 플레이어가 전부 제거된 경우
            if (players.Count == 0)
            {
                currentPlayerIndex = 0;
                GameManager.GetInstance.GameEnd();
                return;
            }

            // 현재 인덱스가 제거된 플레이어보다 크거나 같다면 한 칸 앞으로 당김
            if (currentPlayerIndex >= removedIndex)
            {
                currentPlayerIndex--;
                if (currentPlayerIndex < 0)
                    currentPlayerIndex = 0;
            }

            // 인덱스 범위 보정
            if (currentPlayerIndex >= players.Count)
                currentPlayerIndex = 0;
            SwitchToPlayer(currentPlayerIndex);

        }
    }

    /// <summary>
    /// 모든 플레이어 반환
    /// </summary>
    public List<NodePlayerController> GetAllPlayers()
    {
        return players;
    }

    /// <summary>
    /// 플레이어가 턴을 종료했음을 매니저에 알림
    /// </summary>
    public void NotifyPlayerEndTurn(NodePlayerController player)
    {
        player.isEndReady = true;
        GameManager.GetInstance.CheckAllCharacterEndTurn();
        SwitchToNextPlayer();
    }

    public void EscapeConditionReset()
    {
        heistMemberCount = 0;
        isEscapeBishop = EscapeCondition.None;
        isEscapeRook = EscapeCondition.None;
        isEscapeKnight = EscapeCondition.None;

        foreach (NodePlayerController player in players)
        {
            switch (player.playerStats.characterType)
            {
                case (CharacterType.Bishop):
                    isEscapeBishop = EscapeCondition.Heisting;
                    heistMemberCount += 1;
                    break;
                case (CharacterType.Rook):
                    isEscapeRook = EscapeCondition.Heisting;
                    heistMemberCount += 1;
                    break;
                case (CharacterType.Knight):
                    isEscapeKnight = EscapeCondition.Heisting;
                    heistMemberCount += 1;
                    break;
                default:
                    break;
            }
        }
    }

    public void SetEscapeCondition(EntityStats stats)
    {
        switch (stats.characterType) 
        {
            case (CharacterType.Bishop):
                isEscapeBishop = EscapeCondition.Escape;
                break;
            case (CharacterType.Rook):
                isEscapeRook= EscapeCondition.Escape;
                break;
            case (CharacterType.Knight):
                isEscapeKnight = EscapeCondition.Escape;
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// 현재 스테이지의 성공 여부를 판단. GameResult 열거형을 반환
    /// </summary>
    /// <returns></returns>
    public GameResult GetEscapeSuccess()
    {
        int result = CalculateEscape(isEscapeBishop) + CalculateEscape(isEscapeRook) + CalculateEscape(isEscapeKnight);
        if (result == heistMemberCount * 2)
        {
            return GameResult.Perfect;
        }
        else if (result > 0) 
        {
            return GameResult.Succeeded;
        }
        else
        {
            return GameResult.Failed;
        }
    }

    private int CalculateEscape(EscapeCondition condition) 
    {
        switch (condition)
        {
            case (EscapeCondition.SuccessHeist):
                return 2;
            case(EscapeCondition.Escape):
                return 1;
            case (EscapeCondition.Arrest):
                return 0;
            case (EscapeCondition.None):
                return 0;
            default:
                return 0;
        }
    }
}