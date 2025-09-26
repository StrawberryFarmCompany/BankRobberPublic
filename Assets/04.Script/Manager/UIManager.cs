using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using NodeDefines;

public class UIManager : MonoBehaviour
{
    public static UIManager GetInstance;
    [SerializeField] private GameObject actionPanel;
    [SerializeField] private GameObject cancelPanel;

    public AutoPipBar pip;

    [Header("인터랙션 캔버스")]
    [SerializeField] private GameObject interactionCanvasPrefab;
    [SerializeField] private Transform canvasRoot;
    private GameObject interactionCanvasInstance;

    [Header("인터랙션 버튼 생성용")]
    [SerializeField] private Transform buttonsRoot;
    [SerializeField] private Button interactionButtonPrefab;

    [Header("노드 이미지 키 > 스프라이트")]
    [SerializeField] private List<KeySprite> iconMap = new List<KeySprite>();
    private Dictionary<string, Sprite> iconDict;

    public bool SelectionLocked { get; private set; }

    [Serializable]
    public struct KeySprite
    {
        public string key;
        public Sprite sprite;
    }

    private void Awake()
    {
        GetInstance = this;
        BuildIconDict(); 
    }

    private void BuildIconDict()
    {
        iconDict = new Dictionary<string, Sprite>(StringComparer.OrdinalIgnoreCase);
        foreach (var kv in iconMap)
        {
            if (!string.IsNullOrEmpty(kv.key) && kv.sprite != null)
                iconDict[kv.key] = kv.sprite;
        }
    }

    public void ShowActionPanel(bool show)
    {
        actionPanel.SetActive(show);
        cancelPanel.SetActive(!show);
    }

    public void SetSelectionLocked(bool locked) => SelectionLocked = locked;

    public void TryUpdateInteractionFromNode(Node node, EntityStats actor)
    {
        if (node == null || !node.HasAnyInteraction())
        {
            HideInteractionCanvas();
            return;
        }

        EnsureInteractionCanvas();
        BuildButtonsForNode(node, actor);
        interactionCanvasInstance.SetActive(true);
        // [중요] 패널이 열렸다고 잠그지 않음 (요구사항)
    }

    public void HideInteractionCanvas()
    {
        if (interactionCanvasInstance != null)
            interactionCanvasInstance.SetActive(false);
    }

    private void EnsureInteractionCanvas()
    {
        if (interactionCanvasInstance != null) return;

        var root = canvasRoot != null ? canvasRoot : (actionPanel != null ? actionPanel.transform.parent : transform);
        interactionCanvasInstance = Instantiate(interactionCanvasPrefab, root);
        interactionCanvasInstance.SetActive(false);

        if (!buttonsRoot)
        {
            // VerticalLayoutGroup이 패널 안에 있다면 자동 탐색
            var vlg = interactionCanvasInstance.GetComponentInChildren<UnityEngine.UI.VerticalLayoutGroup>(true);
            buttonsRoot = vlg ? vlg.transform : interactionCanvasInstance.transform;
        }
    }

    private void ClearButtons()
    {
        if (!buttonsRoot) return;
        for (int i = buttonsRoot.childCount - 1; i >= 0; i--)
            Destroy(buttonsRoot.GetChild(i).gameObject);
    }

    private void BuildButtonsForNode(Node node, EntityStats actor)
    {
        ClearButtons();

        foreach (var key in node.EnumerateInteractionKeys())
        {
            var btn = Instantiate(interactionButtonPrefab, buttonsRoot);

            // 아이콘/라벨 세팅(프리팹 구조에 맞게)
            var img = btn.GetComponentInChildren<Image>(true);
            if (img && iconDict != null && iconDict.TryGetValue(key, out var sp)) img.sprite = sp;

            var tmp = btn.GetComponentInChildren<TMPro.TextMeshProUGUI>(true);
            if (tmp) tmp.text = key;
            else
            {
                var legacy = btn.GetComponentInChildren<Text>(true);
                if (legacy) legacy.text = key;
            }

            btn.interactable = true;
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() =>
            {
                // 버튼을 누르는 동안만 입력/선택 잠금
                SetSelectionLocked(true);

                bool ok = node.TryInvokeInteraction(key, actor);

                // 상호작용 구현부(문/창문/골드바)가 끝에서 반드시 false로 풀어주면 가장 정확.
                // 즉시형(동기)일 경우 대비 안전망:
                if (!ok) SetSelectionLocked(false);
            });
        }
    }
}
