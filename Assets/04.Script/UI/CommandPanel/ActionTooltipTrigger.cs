using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ActionTooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [TextArea]
    public string description;   // 버튼마다 직접 입력

    private RectTransform rect;

    void Awake()
    {
        rect = GetComponent<RectTransform>();

        if (gameObject.name == "SpecialActionButton")
            UIManager.GetInstance.specialSkillTooltip = this;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        UIManager.GetInstance.actionTooltip.Show(description, rect);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        UIManager.GetInstance.actionTooltip.Hide();
    }
}
