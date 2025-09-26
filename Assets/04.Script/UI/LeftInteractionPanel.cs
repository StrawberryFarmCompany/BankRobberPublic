using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NodeDefines;

public class LeftInteractionPanel : MonoBehaviour
{
    [Header("Existing UI objects (under Canvas)")]
    [SerializeField] private GameObject panelRoot; // ← 이미 있는 UIPanel GameObject
    [SerializeField] private Image icon;           // ← UIPanel 안의 Image(아이콘)
    [SerializeField] private Button actionButton;  // ← UIPanel 안의 Button(실행 버튼)

    [Header("Icon map (node image key → sprite)")]
    [SerializeField] private List<KeySprite> iconMap = new List<KeySprite>();
    [SerializeField] private string defaultIconKey = "default";

    [Serializable]
    public struct KeySprite { public string key; public Sprite sprite; }

    private Dictionary<string, Sprite> _iconDict;

    void Awake()
    {
        BuildIconDict();
        Hide(); // 시작은 숨김
    }

    void BuildIconDict()
    {
        _iconDict = new Dictionary<string, Sprite>(StringComparer.OrdinalIgnoreCase);
        foreach (var kv in iconMap)
        {
            if (!string.IsNullOrEmpty(kv.key) && kv.sprite) _iconDict[kv.key] = kv.sprite;
        }
    }

    /// <summary>현재 플레이어 발밑 노드 기준으로 패널 갱신</summary>
    public void RefreshForPlayer(NodePlayerController player)
    {
        if (!player) { Hide(); return; }

        var node = GameManager.GetInstance.GetNode(player.transform.position);
        if (node == null || !node.HasAnyInteraction())
        {
            Hide();
            return;
        }

        // 아이콘: Node.GetPrimaryImageKey() 사용 (없으면 default)
        if (icon != null)
        {
            var key = node.GetPrimaryImageKey();
            if (!string.IsNullOrEmpty(key) && _iconDict.TryGetValue(key, out var sp)) icon.sprite = sp;
            else if (_iconDict.TryGetValue(defaultIconKey, out var def)) icon.sprite = def;
        }

        // 버튼: 항상 인터랙터블, 클릭 시 노드 이벤트 실행
        if (actionButton != null)
        {
            actionButton.interactable = true;
            actionButton.onClick.RemoveAllListeners();
            actionButton.onClick.AddListener(() =>
            {
                node.InvokeEvent(player.playerStats);
                Hide(); // 실행 후 닫기(원하면 주석 처리)
            });
        }

        Show();
    }

    public void Hide()
    {
        if (actionButton != null) actionButton.onClick.RemoveAllListeners();
        if (panelRoot != null && panelRoot.activeSelf) panelRoot.SetActive(false);
    }

    void Show()
    {
        if (panelRoot != null && !panelRoot.activeSelf) panelRoot.SetActive(true);
    }
}
