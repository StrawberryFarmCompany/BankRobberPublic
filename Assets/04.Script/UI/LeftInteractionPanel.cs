using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NodeDefines;
using TMPro;

public class LeftInteractionPanel : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject panelRoot; // ← 이미 있는 UIPanel GameObject
    [SerializeField] private Button actionButton;  // ← UIPanel 안의 Button(실행 버튼)

    private bool useCross4Scan = true;
    private bool onlyNonWalkable = false;

    void Awake()
    {
        Hide(); // 시작은 숨김
    }

    public void RefreshForPlayer(NodePlayerController player)
    {
        if (!player) { Hide(); return; }

        if (!TryPickInteractionTarget(player, out Node targetNode, out string targetKey))
        {
            Hide();
            return;
        }

        if (actionButton != null)
        {
            var tmp = actionButton.GetComponentInChildren<TextMeshProUGUI>(true);
            if (tmp) tmp.text = string.IsNullOrEmpty(targetKey) ? "상호작용" : targetKey;

            actionButton.interactable = true;
            actionButton.onClick.RemoveAllListeners();
            actionButton.onClick.AddListener(() =>
            {
                bool ok = true;
                if (!string.IsNullOrEmpty(targetKey))
                    ok = targetNode.TryInvokeInteraction(targetKey, player.playerStats);
                else
                {
                    try { targetNode.InvokeEvent(player.playerStats); }
                    catch { ok = false; }
                }

                if (!ok)
                {
                    try { UIManager.GetInstance?.SetSelectionLocked(false); } catch { }
                }

                Hide();
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

    bool TryPickInteractionTarget(NodePlayerController player, out Node outNode, out string outKey)
    {
        outNode = null;
        outKey = null;

        var gm = GameManager.GetInstance;
        var curNode = gm.GetNode(player.transform.position);
        if (curNode == null) return false;

        var c = curNode.GetCenter;

        if (useCross4Scan)
        {
            Vector3Int[] dirs =
            {
                new Vector3Int( 1,0, 0),
                new Vector3Int(-1,0, 0),
                new Vector3Int( 0,0, 1),
                new Vector3Int( 0,0,-1),
            };

            foreach (var d in dirs)
            {
                var n = gm.GetNode(c + d);
                if (n == null) continue;
                if (onlyNonWalkable && n.isWalkable) continue;

                foreach (var k in n.EnumerateInteractionKeys())
                {
                    outNode = n; outKey = k;
                    return true;
                }

                if (n.HasAnyInteraction())
                {
                    outNode = n; outKey = null;
                    return true;
                }
            }
            return false;
        }
        else
        {
            var n = curNode;

            foreach (var k in n.EnumerateInteractionKeys())
            {
                outNode = n; outKey = k;
                return true;
            }
            if (n.HasAnyInteraction())
            {
                outNode = n; outKey = null;
                return true;
            }
            return false;
        }
    }
}
