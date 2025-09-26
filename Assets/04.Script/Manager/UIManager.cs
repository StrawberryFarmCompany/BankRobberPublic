using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    public void ShowInteractionCanvas(string imageKey, bool interactable, Action onClick = null)
    {
        EnsureInteractionCanvas();

        //아이콘 세팅
        var img = interactionCanvasInstance.GetComponentInChildren<Image>(true);
        if (img != null && !string.IsNullOrEmpty(imageKey) && iconDict != null && iconDict.TryGetValue(imageKey, out var sp))
            img.sprite = sp;

        //버튼 활성/콜백
        var btn = interactionCanvasInstance.GetComponentInChildren<Button>(true);
        if (btn != null)
        {
            btn.interactable = interactable;
            btn.onClick.RemoveAllListeners();
            if (onClick != null) btn.onClick.AddListener(() => onClick());
        }

        interactionCanvasInstance.SetActive(true);

        //선택 잠금 ON
        SetSelectionLocked(true);
    }

    public void HideInteractionCanvas()
    {
        if (interactionCanvasInstance != null)
            interactionCanvasInstance.SetActive(false);

        //선택 잠금 OFF
        SetSelectionLocked(false);
    }

    public void SetSelectionLocked(bool locked)
    {
        SelectionLocked = locked;
    }

    //노드에서 바로 UI 갱신
    public void TryUpdateInteractionFromNode(NodeDefines.Node node)
    {
        if (node == null)
        {
            HideInteractionCanvas();
            return;
        }

        //상호작용 없으면 숨김
        if (!node.HasAnyInteraction())
        {
            HideInteractionCanvas();
            return;
        }

        var key = node.GetPrimaryImageKey();
        //버튼은 기본 숨김/비활성로 두고, 실제 인터랙트 입력은 컨트롤러에서 처리
        ShowInteractionCanvas(key, interactable: true, onClick: null);
    }

    private void EnsureInteractionCanvas()
    {
        if (interactionCanvasInstance != null) return;

        var root = canvasRoot != null ? canvasRoot : (actionPanel != null ? actionPanel.transform.parent : transform);
        interactionCanvasInstance = Instantiate(interactionCanvasPrefab, root);
        interactionCanvasInstance.SetActive(false);
    }
}
