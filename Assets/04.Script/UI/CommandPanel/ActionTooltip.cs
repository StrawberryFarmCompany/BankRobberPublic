using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ActionTooltip : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private TextMeshProUGUI text;

    private RectTransform panelRect;

    public bool IsShowing { get; private set; } = false;
    public RectTransform PanelRect => panelRect;

    private void Awake()
    {
        panelRect = panel.GetComponent<RectTransform>();
        panel.SetActive(false);
    }

    public void Show(string desc, RectTransform target)
    {
        text.text = desc;

        IsShowing = true;
        panel.SetActive(true);
        
        PositionTooltip(target);
    }

    public void ShowCooldown(int remain, RectTransform target)
    {
        text.text = $"쿨타임 {remain}턴 남음";

        IsShowing = true;
        panel.SetActive(true);
        
        PositionTooltip(target);
    }

    public void Hide()
    {
        IsShowing = false;
        panel.SetActive(false);
    }

    private void PositionTooltip(RectTransform target)
    {
        if (target == null) return;

        Vector3 offset = new Vector3(0f, 150f, 0f);
        panelRect.position = target.position + offset;
    }
}
