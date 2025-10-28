using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillInfo : MonoBehaviour
{
    [Header("설명UI")]

    [SerializeField] private GameObject panelRoot;
    [SerializeField] private TextMeshProUGUI effectText;
    [SerializeField] private TextMeshProUGUI coolTimeText;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private RectTransform backgroundRect;

    private Canvas canvas;

    public Vector2 offset = new Vector2(50f, 0f);

    private void Awake()
    {
        canvas = GetComponentInParent<Canvas>();
        Hide();
    }

    public void Show(Skill skill, RectTransform buttonRect)
    {
        if (skill == null || buttonRect == null) return;

        effectText.text = skill.effect;

        if (!string.IsNullOrEmpty(skill.coolTime))
        {
            coolTimeText.text = skill.coolTime;
            coolTimeText.gameObject.SetActive(true);
        }
        else coolTimeText.gameObject.SetActive(false);

        if (!string.IsNullOrEmpty(skill.cost))
        {
            costText.text = skill.cost;
            costText.gameObject.SetActive(true);
        }
        else costText.gameObject.SetActive(false);

        panelRoot.SetActive(true);

        Vector3 worldPos = buttonRect.TransformPoint(new Vector3(buttonRect.rect.width / 2f, 0f, 0f));

        Camera cam = (canvas.renderMode == RenderMode.ScreenSpaceOverlay) ? null : canvas.worldCamera;
        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(cam, worldPos);

        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, screenPos, cam, out Vector2 anchoredPos);

        Vector2 finalOffset;
        if (skill.group == Group.Support)
            finalOffset = new Vector2(-Mathf.Abs(offset.x + 700f), offset.y);
        else
            finalOffset = new Vector2(Mathf.Abs(offset.x), offset.y);

        backgroundRect.anchoredPosition = anchoredPos + finalOffset;

        LayoutRebuilder.ForceRebuildLayoutImmediate(backgroundRect);
    }

    public void Hide()
    {
        panelRoot.SetActive(false);
    }
}
