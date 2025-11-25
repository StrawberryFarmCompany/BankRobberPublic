using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ActionTooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [TextArea]
    public string description;

    public string baseDescription;
    public int cooldownRemain;
    public bool useCooldownTooltip = false;

    private RectTransform rect;

    void Awake()
    {
        rect = GetComponent<RectTransform>();

        if (gameObject.name == "SpecialActionButton")
            UIManager.GetInstance.specialSkillTooltip = this;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        var ui = UIManager.GetInstance;

        if (ui == null || ui.turnActionInput == null || ui.actionTooltip == null)
            return;

        ui.turnActionInput.RefreshHideUI();

        if (useCooldownTooltip)
            ui.actionTooltip.ShowCooldown(cooldownRemain, rect);
        else
            ui.actionTooltip.Show(baseDescription, rect);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        var ui = UIManager.GetInstance;

        if (ui == null || ui.actionTooltip == null)
            return;

        ui.actionTooltip.Hide();
    }
}
