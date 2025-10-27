using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SkillInfoTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Skill skill;
    private SkillInfo skillInfo;
    private RectTransform rect;

    public void Init(Skill skill, SkillInfo info)
    {
        this.skill = skill;
        this.skillInfo = info;
        rect = GetComponent<RectTransform>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        skillInfo?.Show(skill, rect);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        skillInfo?.Hide();
    }
}
