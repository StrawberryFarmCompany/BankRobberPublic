using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class NodePlayerManager : MonoBehaviour
{

    public static NodePlayerManager GetInstance { get; private set; }

    [SerializeField] private List<NodePlayerController> players = new List<NodePlayerController>();
    public int currentPlayerIndex = 0;

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
        players.AddRange(FindObjectsOfType<NodePlayerController>());
        SwitchToPlayer(0); // 첫 번째 플레이어로 시작
        UIManager.GetInstance.pip.HideAndSneakText();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            Debug.Log(GetCurrentPlayer()?.name);
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
        players[currentPlayerIndex].playerInput.ActivateInput();
        UIManager.GetInstance.pip.HideAndSneakText();
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
        players[currentPlayerIndex].playerInput.ActivateInput();
        GetCurrentPlayer().isEndReady = false;
        UIManager.GetInstance.pip.HideAndSneakText();
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
    /// 플레이어 제거
    /// </summary>
    public void UnregisterPlayer(NodePlayerController player)
    {
        if (players.Contains(player))
        {
            players.Remove(player);
            if (currentPlayerIndex >= players.Count)
            {
                currentPlayerIndex = 0;
            }
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
}