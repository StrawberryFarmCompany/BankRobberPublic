using System.Collections.Generic;
using UnityEngine;

public class NodePlayerManager : MonoBehaviour
{
    public static NodePlayerManager Instance { get; private set; }

    [SerializeField] private List<NodePlayerController> players = new List<NodePlayerController>();
    private int currentPlayerIndex = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
    }

    // 플레이어 리스트 자동 등록 (씬에 배치된 모든 NodePlayerController)
    private void Start()
    {
        if (players.Count == 0)
        {
            players.AddRange(FindObjectsOfType<NodePlayerController>());
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
    /// 다음 플레이어로 전환
    /// </summary>
    public void SwitchToNextPlayer()
    {
        if (players.Count == 0) return;
        currentPlayerIndex = (currentPlayerIndex + 1) % players.Count;
    }

    /// <summary>
    /// 특정 플레이어로 전환
    /// </summary>
    public void SwitchToPlayer(int index)
    {
        if (index < 0 || index >= players.Count) return;
        currentPlayerIndex = index;
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
        // 만약 모든 플레이어가 턴을 종료했는지 확인하는 로직
        bool allEnded = true;
        foreach (var p in players)
        {
            if (!p.IsEndTurn) // NodePlayerController에 public bool IsEndTurn 프로퍼티 필요
            {
                allEnded = false;
                break;
            }
        }

        if (allEnded)
        {
            Debug.Log("모든 플레이어 턴 종료 → 다음 페이즈로 전환 가능");
            // 여기서 GameManager 같은 상위 시스템에 페이즈 전환 알림
        }
        else
        {
            // 아직 남은 플레이어가 있다면 다음 플레이어 턴으로
            SwitchToNextPlayer();
            GetCurrentPlayer()?.StartTurn();
        }
    }
}
