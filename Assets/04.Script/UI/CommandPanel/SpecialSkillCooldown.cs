using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SpecialSkillCooldown : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private Image buttonImage;

    private ActionTooltipTrigger tooltip;
    private PlayerSkill currentSkill;

    private void Awake()
    {
        tooltip = GetComponent<ActionTooltipTrigger>();
    }

    private void OnEnable()
    {
        GameManager.GetInstance.NoneBattleTurn.AddStartPointer(TurnTypes.ally, RefreshCooldownUI);
        EquippedSkills.OnChanged += RefreshCooldownUI;
        NodePlayerManager.OnPlayerChanged += RefreshCooldownUI;   // 캐릭터 변경 시 갱신
    }

    private void OnDisable()
    {
        GameManager.GetInstance.NoneBattleTurn.RemoveStartPointer(TurnTypes.ally, RefreshCooldownUI);
        EquippedSkills.OnChanged -= RefreshCooldownUI;
        NodePlayerManager.OnPlayerChanged -= RefreshCooldownUI;
    }

    public void RefreshCooldownUI()
    {
        var player = NodePlayerManager.GetInstance?.GetCurrentPlayer();
        if (player == null) return;

        currentSkill = player.playerStats.playerSkill;

        // 기본 설명 갱신
        Skill so = SkillSOMapper.Get(currentSkill);
        tooltip.baseDescription = so != null ? so.effect : "장착된 스킬 없음";

        // 쿨타임 계산
        int remain = SkillEffectManager.GetInstance.GetRemainingCooldown(player, currentSkill);

        if (remain > 0)
        {
            buttonImage.color = new Color(0.4f, 0.4f, 0.4f);
            button.interactable = false;

            tooltip.cooldownRemain = remain;      // 트리거로 전달
            tooltip.useCooldownTooltip = true;    // 쿨타임 UI 사용
        }
        else
        {
            buttonImage.color = Color.white;
            button.interactable = true;

            tooltip.useCooldownTooltip = false;  // 기본 UI 사용
        }
    }
}
