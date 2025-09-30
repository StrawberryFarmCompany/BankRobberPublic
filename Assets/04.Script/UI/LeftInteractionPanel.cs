using NodeDefines;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LeftInteractionPanel : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject panelRoot;
    [SerializeField] private Button actionButton;
    private Transform buttonsRoot;

    static readonly string[] WhitelistParts = { "Window", "Door" };

    static readonly Vector3Int[] Cross4 =
    {
        new Vector3Int( 1,0, 0),
        new Vector3Int(-1,0, 0),
        new Vector3Int( 0,0, 1),
        new Vector3Int( 0,0,-1),
    };

    NodePlayerController _lastPlayer;
    Vector3Int _lastCenter;
    int _lastPlayerIndex = -1;

    void Awake()
    {
        buttonsRoot = panelRoot ? panelRoot.transform : transform;
        if (actionButton) actionButton.gameObject.SetActive(false);
        Hide(); // 시작은 숨김
    }

    void OnDisable()
    {
        // 컴포넌트 비활성 시 잔여 버튼 정리
        ClearButtons();
    }

    void Update()
    {
        var npm = NodePlayerManager.GetInstance;
        if (npm == null) { Hide(); return; }

        var curPlayer = npm.GetCurrentPlayer();
        if (curPlayer == null) { Hide(); return; }

        bool playerChanged = (_lastPlayer != curPlayer) || (_lastPlayerIndex != npm.currentPlayerIndex);

        var gm = GameManager.GetInstance;
        var node = gm?.GetNode(curPlayer.transform.position);
        node.GetPrimaryImageKey();
        if (node == null) { Hide(); return; }
        var center = node.GetCenter;

        bool centerChanged = center != _lastCenter;

        //플레이어가 바뀌었거나, 현재 센터 타일이 바뀌었을 때만 갱신
        if (playerChanged || centerChanged)
        {
            RefreshForPlayer(curPlayer);
            _lastPlayer = curPlayer;
            _lastPlayerIndex = npm.currentPlayerIndex;
            _lastCenter = center;
        }
    }

    public void RefreshForPlayer(NodePlayerController player)
    {
        if (!player) { Hide(); return; }

        var pairs = CollectPairs(player);   //노드 키 목록
        BuildButtons(pairs, player);        //버튼 생성

        if (pairs.Count > 0) Show();
        else Hide();
    }

    bool IsFrontWallPattern(Vector3Int playerC, Vector3Int wallC, GameManager gm)
    {
        var d = wallC - playerC;
        //1칸(상하좌우), 대각선/2칸 제외
        if (Mathf.Abs(d.x) + Mathf.Abs(d.z) != 1) return false;

        var wall = gm.GetNode(wallC);
        if (wall == null || wall.isWalkable) return false;

        var land = gm.GetNode(wallC + d); //벽 반대편
        if (land == null || !land.isWalkable) return false;

        return true;
    }


    private List<(Node, string)> CollectPairs(NodePlayerController player)
    {
        var list = new List<(Node, string)>();
        var gm = GameManager.GetInstance;
        var cur = gm.GetNode(player.transform.position);
        if (cur == null || !cur.HasAnyInteraction()) return list;

        foreach (var k in cur.EnumerateInteractionKeys())
            if (IsWhitelisted(k)) list.Add((cur, k));

        list.Sort((a, b) => 
        { 
            int pa = PriorityIndex(a.Item2); 
            int pb = PriorityIndex(b.Item2); 
            int c = pa.CompareTo(pb); 
            return c != 0 ? c : string.Compare(a.Item2, b.Item2, StringComparison.OrdinalIgnoreCase);
        });

        return list;
    }

    private bool IsWhitelisted(string key)
    {
        foreach (var part in WhitelistParts)
            if (key.IndexOf(part, StringComparison.OrdinalIgnoreCase) >= 0)
                return true;
        return false;
    }

    private int PriorityIndex(string key)
    {
        for (int i = 0; i < WhitelistParts.Length; i++)
            if (key.IndexOf(WhitelistParts[i], StringComparison.OrdinalIgnoreCase) >= 0)
                return i;
        return int.MaxValue;
    }

    //버튼 생성
    void BuildButtons(List<(Node node, string key)> pairs, NodePlayerController player)
    {
        ClearButtons();

        if (!actionButton || !buttonsRoot) return;

        foreach (var (node, key) in pairs)
        {
            var btn = Instantiate(actionButton, buttonsRoot);
            btn.gameObject.SetActive(true);

            var tmp = btn.GetComponentInChildren<TextMeshProUGUI>(true);
            if (tmp) tmp.text = KeyToLabel(key);
            else
            {
                var legacy = btn.GetComponentInChildren<Text>(true);
                if (legacy) legacy.text = KeyToLabel(key);
            }

            btn.interactable = true;
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() =>
            {
                bool ok = node.TryInvokeInteraction(key, player.playerStats);

                RefreshForPlayer(player);
                if (!ok)
                {
                    try { UIManager.GetInstance?.SetSelectionLocked(false); } catch { }
                }
            });
        }
    }

    string KeyToLabel(string key)
    {
        if (key.IndexOf("Window", StringComparison.OrdinalIgnoreCase) >= 0) return "window";    //나중에 한글로 바꿔주기 
        if (key.IndexOf("Door", StringComparison.OrdinalIgnoreCase) >= 0) return "door";
        return key;
    }

    void ClearButtons()
    {
        if (!buttonsRoot) return;

        for (int i = buttonsRoot.childCount - 1; i >= 0; i--)
        {
            var child = buttonsRoot.GetChild(i);
            if (actionButton && child == actionButton.transform) continue;
            Destroy(child.gameObject);
        }
    }

    public void Hide()
    {
        ClearButtons();
        if (panelRoot && panelRoot.activeSelf) panelRoot.SetActive(false);
    }

    void Show()
    {
        if (panelRoot && !panelRoot.activeSelf) panelRoot.SetActive(true);
    }
}
