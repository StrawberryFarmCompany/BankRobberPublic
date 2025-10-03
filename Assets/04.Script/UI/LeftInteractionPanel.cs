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

        BuildButtons(player);        //버튼 생성
    }

    //버튼 생성
    void BuildButtons(NodePlayerController player)
    {
        ClearButtons();

        if (!actionButton || !buttonsRoot) return;
        Node node = GameManager.GetInstance.GetNode(player.transform.position);
        string[] strArray = node.GetInteractionNameArray();

        if (strArray == null)
        {
            Hide();
            return;
        }

        for (int i = 0; i < strArray.Length; i++)
        {
            Button btn = Instantiate(actionButton, buttonsRoot);
            btn.gameObject.SetActive(true);
            btn.transform.name = "Btn_" + strArray[i];

            var tmp = btn.GetComponentInChildren<TextMeshProUGUI>(true);
            if (tmp) tmp.text = KeyToLabel(strArray[i]);

            btn.interactable = true;
            btn.onClick.RemoveAllListeners();
            int index = i;
            btn.onClick.AddListener(() =>
            {
                bool ok = node.TryInvokeInteraction(strArray[index], player.playerStats);

                RefreshForPlayer(player);
                if (!ok)
                {
                    try { UIManager.GetInstance?.SetSelectionLocked(false); } catch { }
                }
            });
        }

        if (strArray.Length > 0) Show();
        else Hide();
    }

    string KeyToLabel(string key)
    {
        if (key.IndexOf("Window", StringComparison.OrdinalIgnoreCase) >= 0) return "창문 넘기";    //나중에 한글로 바꿔주기 
        if (key.IndexOf("Door", StringComparison.OrdinalIgnoreCase) >= 0) return "문";
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
