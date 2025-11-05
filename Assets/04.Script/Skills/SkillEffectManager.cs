using BuffDefine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class SkillEffectManager : MonoBehaviour
{
    private static Dictionary<PlayerSkill, int> cooldowns = new();

    public static void ReduceCooldowns()
    {
        List<PlayerSkill> keys = new List<PlayerSkill>(cooldowns.Keys);
        foreach (PlayerSkill key in keys)
        {
            if (cooldowns[key] > 0)
                cooldowns[key]--;
        }
    }

    public static bool CanUse(PlayerSkill skill)
    {
        return !cooldowns.ContainsKey(skill) || cooldowns[skill] <= 0;
    }

    public static void SetCooldown(PlayerSkill skill, int turns)
    {
        cooldowns[skill] = turns;
    }

    public static int GetRemainingCooldown(PlayerSkill skill)
    {
        return cooldowns.ContainsKey(skill) ? cooldowns[skill] : 0;
    }

    public static void UseSkill(NodePlayerController player, Vector3 mousePos)
    {
        PlayerSkill skill = player.playerStats.playerSkill;
        if (skill == PlayerSkill.None)
        {
            Debug.Log("스킬 미장착");
            return;
        }

        if (!CanUse(skill))
        {
            Debug.Log($"[{skill}] 쿨타임 남은 턴: {cooldowns[skill]}");
            return;
        }

        switch (skill)
        {
            //체력 회복
            case PlayerSkill.Heal:
                if (!player.playerStats.ConsumeActionPoint(1)) return;
                player.playerStats.HealHealthPoint(1);
                player.animationController.HealState();
                SetCooldown(skill, 3);
                break;

            //암습
            case PlayerSkill.SneakAttack:
                if (!player.playerStats.ConsumeActionPoint(1)) return;

                player.CheckSneakAttack(mousePos);

                SetCooldown(skill, 1);
                break;

            //소음 제거
            case PlayerSkill.Silence:
                if (!player.playerStats.ConsumeActionPoint(2)) return;

                MethodInfo hideMethod = typeof(NodePlayerController)
                    .GetMethod("HideMode", BindingFlags.NonPublic | BindingFlags.Instance);
                if (hideMethod != null)
                {
                    hideMethod.Invoke(player, null);
                    Debug.Log("[소음 제거] 은신 성공");
                }
                else
                {
                    Debug.LogWarning("[소음 제거] 호출 실패");
                }

                SetCooldown(skill, 5);
                break;

            //이중 타격
            case PlayerSkill.DoubleAttack:
                if (!player.playerStats.ConsumeActionPoint(2)) return;

                player.gun.Shoot(player.targetNodePos, player.hitBonus);
                Debug.Log("[이중 타격] 첫 번째 공격");
                player.gun.Shoot(player.targetNodePos, player.hitBonus);
                Debug.Log("[이중 타격] 두 번째 공격");

                SetCooldown(skill, 3);
                break;

            //행동력 회복
            case PlayerSkill.Ready:
                if (!player.playerStats.ConsumeActionPoint(1)) return;
                player.playerStats.HealActionPoint(3);
                player.animationController.ReadyState();
                Debug.Log("[행동력 회복] 행동력 +3");
                SetCooldown(skill, 3);
                break;

            //회피율 증가 버프
            case PlayerSkill.Evasion:
                if (!player.playerStats.ConsumeActionPoint(2)) return;

                BuffDefine.ParsingBuffData tempData = new BuffDefine.ParsingBuffData
                {
                    key = 999,
                    statusName = "Evasion Buff",
                    disStatusNameEng = "Evasion",
                    disStatusNameKor = "회피율 증가",
                    descEng = "Increases evasion rate for 3 turns",
                    descKor = "3턴간 회피율 +2",
                    duration = 3,
                    statusValue = 2,
                    statusType = BuffDefine.BuffType.aiming,
                    colorType = BuffDefine.BuffColorType.green
                };

                BuffDefine.BuffData evasionBuff = new BuffDefine.BuffData(tempData);
                player.playerStats.RegistBuff(evasionBuff);

                Debug.Log("[회피율 증가] 버프 적용");
                SetCooldown(skill, 5);
                break;
        }

        Debug.Log($"[{skill}] 사용 완료. 쿨타임 {cooldowns[skill]}턴");
    }
}