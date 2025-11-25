using BuffDefine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class SkillEffectManager : MonoSingleTon<SkillEffectManager>
{
    private Dictionary<NodePlayerController, Dictionary<PlayerSkill, int>> cooldowns = new Dictionary<NodePlayerController, Dictionary<PlayerSkill, int>>();
    private ShotEffect shotEffect;
    public ShotEffect ShotEffect 
    { 
        get 
        {
            if (shotEffect == null) shotEffect = new ShotEffect();
            return shotEffect;
        }
    }
    public override void OnSceneChange()
    {
        shotEffect = null;
        if (LoadSceneManager.GetInstance.curSceneType == SceneType.MainTitleScene || LoadSceneManager.GetInstance.curSceneType == SceneType.LobbyScene)
        {
            cooldowns.Clear();
        }
    }

    public int GetRemainingCooldown(NodePlayerController player, PlayerSkill skill)
    {
        if (!cooldowns.ContainsKey(player)) return 0;
        if (!cooldowns[player].ContainsKey(skill)) return 0;
        return cooldowns[player][skill];
    }

    public void SetCooldown(NodePlayerController player, PlayerSkill skill, int turns)
    {
        if (!cooldowns.ContainsKey(player))
            cooldowns[player] = new Dictionary<PlayerSkill, int>();

        cooldowns[player][skill] = turns;
    }

    public bool CanUse(NodePlayerController player, PlayerSkill skill)
    {
        return GetRemainingCooldown(player, skill) <= 0;
    }

    public void ReduceCooldowns()
    {
        foreach (var playerEntry in cooldowns)
        {
            var player = playerEntry.Key;
            var skills = playerEntry.Value;

            List<PlayerSkill> keys = new List<PlayerSkill>(skills.Keys);

            foreach (var skill in keys)
            {
                if (skills[skill] > 0)
                    skills[skill]--;
            }
        }
    }

    public void UseSkill(NodePlayerController player, Vector3 mousePos)
    {
        PlayerSkill skill = player.playerStats.playerSkill;

        if (skill == PlayerSkill.None)
        {
            Debug.Log("스킬 미장착");
            return;
        }

        if (!CanUse(player, skill))
        {
            Debug.Log($"[{skill}] 쿨타임 남은 턴: {GetRemainingCooldown(player, skill)}");
            return;
        }

        switch (skill)
        {
            //체력 회복
            case PlayerSkill.Heal:
                if (!player.playerStats.ConsumeActionPoint(2)) return;
                player.playerStats.HealHealthPoint(1);
                player.animationController.HealState();
                SetCooldown(player, skill, 3);
                break;

            //체력 회복 강화A (3칸 회복)
            case PlayerSkill.Heal_A:
                if (!player.playerStats.ConsumeActionPoint(2)) return;
                player.playerStats.HealHealthPoint(3);
                player.animationController.HealState();
                SetCooldown(player, skill, 3);
                break;

            //체력 회복 강화B (모든 아군 회복)
            case PlayerSkill.Heal_B:
                if (!player.playerStats.ConsumeActionPoint(2)) return;
                foreach (var ally in NodePlayerManager.GetInstance.GetAllPlayers())
                    ally.playerStats.HealHealthPoint(1);
                player.animationController.HealState();
                SetCooldown(player, skill, 3);
                break;

            //암습
            case PlayerSkill.SneakAttack:
                Vector3Int before = player.targetNodePos;

                player.CheckSneakAttack(mousePos);

                if (player.targetNodePos != before)
                {
                    SetCooldown(player, skill, 1);
                }
                else
                {
                    return;
                }
                break;

            //암습 강화A (성공 시 이동력 회복)
            case PlayerSkill.SneakAttack_A:
                Vector3Int beforeA = player.targetNodePos;

                player.CheckSneakAttack(mousePos);

                if (player.targetNodePos != beforeA)
                {
                    player.playerStats.HealMovement(5);
                    SetCooldown(player, skill, 1);
                }
                else
                {
                    return;
                }
                break;

            //암습 강화B (성공 확률 증가)
            case PlayerSkill.SneakAttack_B:
                Vector3Int beforeB = player.targetNodePos;

                player.CheckSneakAttack(mousePos, true);

                if (player.targetNodePos != beforeB)
                {
                    player.StartCoroutine(DelayedSneakAttack(player, mousePos, 0.3f, false));
                    SetCooldown(player, skill, 1);
                }
                else
                {
                    return;
                }
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

                SetCooldown(player, skill, 5);
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
                SetCooldown(player, skill, 5);
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
                SetCooldown(player, skill, 5);
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

                SetCooldown(player, skill, 3);
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

                SetCooldown(player, skill, 3);
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

                SetCooldown(player, skill, 3);
                break;

            //행동력 회복
            case PlayerSkill.Ready:
                if (!player.playerStats.ConsumeActionPoint(1)) return;
                player.playerStats.HealActionPoint(3);
                player.animationController.ReadyState();
                SetCooldown(player, skill, 3);
                break;

            //행동력 회복 강화A (전부 회복)
            case PlayerSkill.Ready_A:
                if (!player.playerStats.ConsumeActionPoint(1)) return;
                player.playerStats.HealActionPoint(player.playerStats.actionPoint);
                SetCooldown(player, skill, 3);
                break;

            //행동력 회복 강화B (모든 아군 +3)
            case PlayerSkill.Ready_B:
                if (!player.playerStats.ConsumeActionPoint(1)) return;
                foreach (var ally in NodePlayerManager.GetInstance.GetAllPlayers())
                    ally.playerStats.HealActionPoint(3);
                SetCooldown(player, skill, 3);
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

                SetCooldown(player, skill, 5);
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
                SetCooldown(player, skill, 5);
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
                SetCooldown(player, skill, 5);
                break;
        }
        UIManager.GetInstance?.GetComponentInChildren<SpecialSkillCooldown>(true)?.RefreshCooldownUI();
    }

    private IEnumerator DoubleAttackRoutine(NodePlayerController player, Vector3 mousePos)
    {
        player.IsSkillAttack = true;

        Vector3Int targetNode = player.GetNodeVector3ByRay(mousePos, (1 << 8), true);
        if (targetNode == new Vector3Int(-999, -999, -999))
        {
            player.IsSkillAttack = false;
            yield break;
        }

        player.CheckRangeAttackPos(targetNode);
        yield return new WaitForSeconds(0.5f);

        player.CheckRangeAttackPos(targetNode);
        player.IsSkillAttack = false;
    }

    private IEnumerator TripleAttackRoutine(NodePlayerController player, Vector3 mousePos)
    {
        player.IsSkillAttack = true;

        Vector3Int targetNode = player.GetNodeVector3ByRay(mousePos, (1 << 8), true);
        if (targetNode == new Vector3Int(-999, -999, -999))
        {
            player.IsSkillAttack = false;
            yield break;
        }

        for (int i = 0; i < 3; i++)
        {
            Debug.Log($"[TripleAttack] {i + 1}번째 공격 시도");
            player.CheckRangeAttackPos(targetNode);
            yield return new WaitForSeconds(0.5f);
        }

        player.IsSkillAttack = false;
    }

    private IEnumerator DelayedSneakAttack(NodePlayerController player, Vector3 mousePos, float delay, bool consumeAction = true)
    {
        yield return new WaitForSeconds(delay);
        player.CheckSneakAttack(mousePos, consumeAction);
    }
}

public static class SkillSOMapper
{
    private static Dictionary<PlayerSkill, Skill> map = new();
    private static bool initialized = false;

    public static void Init()
    {
        if (initialized) return;
        initialized = true;

        //Combat
        map[PlayerSkill.Heal] = Load("Skills/Combat/Active/CombatActive1");
        map[PlayerSkill.Heal_A] = Load("Skills/Combat/Upgrade/Upgrade1/CombatUpgrade1 1");
        map[PlayerSkill.Heal_B] = Load("Skills/Combat/Upgrade/Upgrade1/CombatUpgrade1 2");

        map[PlayerSkill.DoubleAttack] = Load("Skills/Combat/Active/CombatActive2");
        map[PlayerSkill.DoubleAttack_A] = Load("Skills/Combat/Upgrade/Upgrade2/CombatUpgrade2 1");
        map[PlayerSkill.DoubleAttack_B] = Load("Skills/Combat/Upgrade/Upgrade2/CombatUpgrade2 2");

        //Stealth
        map[PlayerSkill.SneakAttack] = Load("Skills/Stealth/Active/StealthActive1");
        map[PlayerSkill.SneakAttack_A] = Load("Skills/Stealth/Upgrade/Upgrade1/StealthUpgrade1 1");
        map[PlayerSkill.SneakAttack_B] = Load("Skills/Stealth/Upgrade/Upgrade1/StealthUpgrade1 2");

        map[PlayerSkill.Silence] = Load("Skills/Stealth/Active/StealthActive2");
        map[PlayerSkill.Silence_A] = Load("Skills/Stealth/Upgrade/Upgrade2/StealthUpgrade2 1");
        map[PlayerSkill.Silence_B] = Load("Skills/Stealth/Upgrade/Upgrade2/StealthUpgrade2 2");

        //Support
        map[PlayerSkill.Ready] = Load("Skills/Support/Active/SupportActive1");
        map[PlayerSkill.Ready_A] = Load("Skills/Support/Upgrade/Upgrade1/SupportUpgrade1 1");
        map[PlayerSkill.Ready_B] = Load("Skills/Support/Upgrade/Upgrade1/SupportUpgrade1 2");

        map[PlayerSkill.Evasion] = Load("Skills/Support/Active/SupportActive2");
        map[PlayerSkill.Evasion_A] = Load("Skills/Support/Upgrade/Upgrade2/SupportUpgrade2 1");
        map[PlayerSkill.Evasion_B] = Load("Skills/Support/Upgrade/Upgrade2/SupportUpgrade2 2");
    }

    private static Skill Load(string path)
    {
        Skill so = Resources.Load<Skill>(path);
        if (so == null)
            Debug.LogError($"[SkillSOMapper] 경로 오류: {path}");
        return so;
    }

    public static Skill Get(PlayerSkill ps)
    {
        if (!initialized) Init();
        return map.TryGetValue(ps, out Skill so) ? so : null;
    }
}