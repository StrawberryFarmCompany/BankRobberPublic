using BuffDefine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class SkillEffectManager : MonoSingleTon<SkillEffectManager>
{
    private Dictionary<PlayerSkill, int> cooldowns = new();
    private ShotEffect shotEffect;
    public ShotEffect ShotEffect 
    { 
        get 
        {
            if (shotEffect == null) shotEffect = new ShotEffect();
            return shotEffect;
        }
    }
    public void ReduceCooldowns()
    {
        List<PlayerSkill> keys = new List<PlayerSkill>(cooldowns.Keys);
        foreach (PlayerSkill key in keys)
        {
            if (cooldowns[key] > 0)
                cooldowns[key]--;
        }
    }

    public bool CanUse(PlayerSkill skill)
    {
        return !cooldowns.ContainsKey(skill) || cooldowns[skill] <= 0;
    }

    public void SetCooldown(PlayerSkill skill, int turns)
    {
        cooldowns[skill] = turns;
    }

    public int GetRemainingCooldown(PlayerSkill skill)
    {
        return cooldowns.ContainsKey(skill) ? cooldowns[skill] : 0;
    }

    public void UseSkill(NodePlayerController player, Vector3 mousePos)
    {
        Debug.Log($"[SkillEffectManager] 현재 스킬: {player.playerStats.playerSkill}");
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
                if (!player.playerStats.ConsumeActionPoint(2)) return;
                player.playerStats.HealHealthPoint(1);
                player.animationController.HealState();
                SetCooldown(skill, 3);
                break;

            //체력 회복 강화A (3칸 회복)
            case PlayerSkill.Heal_A:
                if (!player.playerStats.ConsumeActionPoint(2)) return;
                player.playerStats.HealHealthPoint(3);
                player.animationController.HealState();
                SetCooldown(skill, 3);
                break;

            //체력 회복 강화B (모든 아군 회복)
            case PlayerSkill.Heal_B:
                if (!player.playerStats.ConsumeActionPoint(2)) return;
                foreach (var ally in NodePlayerManager.GetInstance.GetAllPlayers())
                    ally.playerStats.HealHealthPoint(1);
                player.animationController.HealState();
                SetCooldown(skill, 3);
                break;

            //암습
            case PlayerSkill.SneakAttack:

                player.CheckSneakAttack(mousePos);

                SetCooldown(skill, 1);
                break;

            //암습 강화A (성공 시 이동력 회복)
            case PlayerSkill.SneakAttack_A:
                player.CheckSneakAttack(mousePos);
                player.playerStats.HealMovement(5);
                SetCooldown(skill, 1);
                break;

            //암습 강화B (성공 확률 증가)
            case PlayerSkill.SneakAttack_B:

                Debug.Log("[암습 강화B] 암습 2회 판정 시작");

                player.CheckSneakAttack(mousePos, true);

                player.StartCoroutine(DelayedSneakAttack(player, mousePos, 0.3f, false));
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
                }

                SetCooldown(skill, 5);
                break;

            //소음 제거 강화A (지속 턴 증가)
            case PlayerSkill.Silence_A:
                if (!player.playerStats.ConsumeActionPoint(2)) return;
                BuffDefine.ParsingBuffData silenceBuffData = new BuffDefine.ParsingBuffData
                {
                    key = 1101,
                    statusName = "Silence Mode",
                    disStatusNameEng = "Silence",
                    disStatusNameKor = "소음 제거",
                    descEng = "Removes noise detection for 3 turns",
                    descKor = "3턴간 소음 제거 유지",
                    duration = 3,
                    statusValue = 0,
                    statusType = BuffDefine.BuffType.none,
                    colorType = BuffDefine.BuffColorType.none
                };
                player.playerStats.RegistBuff(new BuffDefine.BuffData(silenceBuffData));
                Debug.Log("[소음 제거 강화A] 3턴간 소음 제거 상태 유지");
                SetCooldown(skill, 5);
                break;

            //소음 제거 강화B (모든 아군 소음 제거)
            case PlayerSkill.Silence_B:
                if (!player.playerStats.ConsumeActionPoint(2)) return;
                foreach (var ally in NodePlayerManager.GetInstance.GetAllPlayers())
                {
                    hideMethod = typeof(NodePlayerController)
                        .GetMethod("HideMode", BindingFlags.NonPublic | BindingFlags.Instance);
                    hideMethod?.Invoke(ally, null);
                }
                SetCooldown(skill, 5);
                break;

            //이중 타격
            case PlayerSkill.DoubleAttack:
                if (!player.playerStats.ConsumeActionPoint(1)) return;

                Vector3Int targetNode = player.GetNodeVector3ByRay(mousePos, (1 << 8), true);
                if (targetNode == new Vector3Int(-999, -999, -999))
                {
                    Debug.LogWarning("[이중 타격] 유효하지 않은 타겟");
                    return;
                }

                if (!player.CheckRangeAndEntity(targetNode, (int)player.playerStats.attackRange))
                {
                    Debug.Log("[이중 타격] 범위 밖이거나 적이 없음");
                    return;
                }
                StartCoroutine(DoubleAttackRoutine(player, mousePos));

                SetCooldown(skill, 3);
                break;

            //이중 타격 강화A (3타격)
            case PlayerSkill.DoubleAttack_A:
                if (!player.playerStats.ConsumeActionPoint(1)) return;

                targetNode = player.GetNodeVector3ByRay(mousePos, (1 << 8), true);
                if (targetNode == new Vector3Int(-999, -999, -999))
                {
                    Debug.LogWarning("[3중 타격] 유효하지 않은 타겟");
                    return;
                }

                if (!player.CheckRangeAndEntity(targetNode, (int)player.playerStats.attackRange))
                {
                    Debug.Log("[3중 타격] 범위 밖이거나 적이 없음");
                    return;
                }

                StartCoroutine(TripleAttackRoutine(player, mousePos));
                SetCooldown(skill, 3);
                break;

            //이중 타격 강화B (공격력 +1 보정)
            case PlayerSkill.DoubleAttack_B:
                if (!player.playerStats.ConsumeActionPoint(1)) return;

                if (!player.playerStats.hasPermanentAttackBuff)
                {
                    player.playerStats.accuracyModifier += 1;
                    player.playerStats.hasPermanentAttackBuff = true;
                }

                targetNode = player.GetNodeVector3ByRay(mousePos, (1 << 8), true);
                if (targetNode == new Vector3Int(-999, -999, -999)) return;
                if (!player.CheckRangeAndEntity(targetNode, (int)player.playerStats.attackRange)) return;

                StartCoroutine(DoubleAttackRoutine(player, mousePos));
                SetCooldown(skill, 3);
                break;

            //행동력 회복
            case PlayerSkill.Ready:
                if (!player.playerStats.ConsumeActionPoint(1)) return;
                player.playerStats.HealActionPoint(3);
                player.animationController.ReadyState();
                SetCooldown(skill, 3);
                break;

            //행동력 회복 강화A (전부 회복)
            case PlayerSkill.Ready_A:
                if (!player.playerStats.ConsumeActionPoint(1)) return;
                player.playerStats.HealActionPoint(player.playerStats.actionPoint);
                SetCooldown(skill, 3);
                break;

            //행동력 회복 강화B (모든 아군 +3)
            case PlayerSkill.Ready_B:
                if (!player.playerStats.ConsumeActionPoint(1)) return;
                foreach (var ally in NodePlayerManager.GetInstance.GetAllPlayers())
                    ally.playerStats.HealActionPoint(3);
                SetCooldown(skill, 3);
                break;

            //회피율 증가 버프
            case PlayerSkill.Evasion:
                if (!player.playerStats.ConsumeActionPoint(2)) return;

                BuffDefine.ParsingBuffData evasion = new BuffDefine.ParsingBuffData
                {
                    key = 999,
                    statusName = "Evasion Buff",
                    disStatusNameKor = "회피율 증가",
                    descKor = "3턴간 회피율 +2",
                    duration = 3,
                    statusValue = 2,
                    statusType = BuffDefine.BuffType.aiming,
                    colorType = BuffDefine.BuffColorType.green
                };

                BuffDefine.BuffData evasionBuff = new BuffDefine.BuffData(evasion);
                player.playerStats.RegistBuff(evasionBuff);

                SetCooldown(skill, 5);
                break;

            //회피율 증가 강화A (+4)
            case PlayerSkill.Evasion_A:
                if (!player.playerStats.ConsumeActionPoint(2)) return;
                BuffDefine.ParsingBuffData evasionA = new BuffDefine.ParsingBuffData
                {
                    key = 1001,
                    statusName = "Evasion Buff Strong",
                    disStatusNameKor = "회피율 대폭 증가",
                    descKor = "3턴간 회피율 +4",
                    duration = 3,
                    statusValue = 4,
                    statusType = BuffDefine.BuffType.aiming,
                    colorType = BuffDefine.BuffColorType.green
                };
                player.playerStats.RegistBuff(new BuffDefine.BuffData(evasionA));
                SetCooldown(skill, 5);
                break;

            //회피율 증가 강화B (모든 아군 +2)
            case PlayerSkill.Evasion_B:
                if (!player.playerStats.ConsumeActionPoint(2)) return;
                BuffDefine.ParsingBuffData evasionB = new BuffDefine.ParsingBuffData
                {
                    key = 1002,
                    statusName = "Team Evasion Buff",
                    disStatusNameKor = "모든 아군 회피율 증가",
                    descKor = "3턴간 모든 아군 회피율 +2",
                    duration = 3,
                    statusValue = 2,
                    statusType = BuffDefine.BuffType.aiming,
                    colorType = BuffDefine.BuffColorType.green
                };

                foreach (var ally in NodePlayerManager.GetInstance.GetAllPlayers())
                    ally.playerStats.RegistBuff(new BuffDefine.BuffData(evasionB));

                Debug.Log("[회피율 증가 강화B] 모든 아군 회피율 +2");
                SetCooldown(skill, 5);
                break;
        }
    
        Debug.Log($"[{skill}] 사용 완료. 쿨타임 {cooldowns[skill]}턴");
    }

    private IEnumerator DoubleAttackRoutine(NodePlayerController player, Vector3 mousePos)
    {
        player.CheckRangeAttack(mousePos);

        yield return new WaitForSeconds(0.5f);

        player.CheckRangeAttack(mousePos);
    }

    private IEnumerator TripleAttackRoutine(NodePlayerController player, Vector3 mousePos)
    {
        Vector3Int targetNode = player.GetNodeVector3ByRay(mousePos, (1 << 8), true);
        if (targetNode == new Vector3Int(-999, -999, -999))
        {
            yield break;
        }

        for (int i = 0; i < 3; i++)
        {
            Debug.Log($"[TripleAttack] {i + 1}번째 공격 시도");
            player.CheckRangeAttackPos(targetNode);
            yield return new WaitForSeconds(0.5f);
        }
    }

    private IEnumerator DelayedSneakAttack(NodePlayerController player, Vector3 mousePos, float delay, bool consumeAction = true)
    {
        yield return new WaitForSeconds(delay);
        player.CheckSneakAttack(mousePos, consumeAction);
    }
}