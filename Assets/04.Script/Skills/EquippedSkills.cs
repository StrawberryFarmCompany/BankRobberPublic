using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

[Serializable]
class EquippedSkillData
{
    public List<string> equipped = new List<string>();
}

public static class EquippedSkills
{
    public static event Action OnChanged;

    const string FileName = "equippedSkills.json";
    static string FilePath => Path.Combine(Application.persistentDataPath, FileName);

    static EquippedSkillData data = new EquippedSkillData();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Init()
    {
        Load();
        OnChanged?.Invoke();
    }

    //스킬 장착 여부 확인
    public static bool IsEquipped(string skillId)
    {
        if (string.IsNullOrEmpty(skillId)) return false;
        return data.equipped.Contains(skillId);
    }

    //스킬 장착 시도 (같은 Kind(Active/Passive/Upgrade)는 하나만)
    public static void Equip(Skill skill)
    {
        if (skill == null) return;
        string key = skill.GetKey();

        if (skill.kind == Kind.Upgrade)
        {
            //idNum
            int parentActiveId = ((skill.idNum - 1) / 2) + 1;
            string parentActiveKey = $"{skill.group}.Active.{parentActiveId}";

            if (!IsEquipped(parentActiveKey))
            {
                Debug.LogWarning($"[EquippedSkills] {skill.title} 장착 실패 — 부모 액티브({parentActiveKey})가 장착되어 있지 않습니다.");
                return;
            }
        }

        data.equipped.RemoveAll(k => k.StartsWith($"{skill.group}.{skill.kind}."));

        //액티브 바꿀 때 업그레이드 해제
        if (skill.kind == Kind.Active)
            data.equipped.RemoveAll(k => k.StartsWith($"{skill.group}.Upgrade."));

        //패시브 교체
        if (skill.kind == Kind.Passive)
            data.equipped.RemoveAll(k => k.StartsWith($"{skill.group}.Passive."));

        data.equipped.Add(key);
        Save();
        OnChanged?.Invoke();

        if (skill.kind == Kind.Passive)
        {
            NodePlayerController currentPlayer = NodePlayerManager.GetInstance?.GetCurrentPlayer();
            if (currentPlayer != null && currentPlayer.playerStats != null)
            {
                ApplyEquippedPassives(currentPlayer.playerStats);
                Debug.Log($"[EquippedSkills] 새 패시브 적용됨: {skill.title}");
            }
        }
    }

    //스킬 해제
    public static void Unequip(Skill skill)
    {
        if (skill == null) return;
        string key = skill.GetKey();

        bool removed = data.equipped.Remove(key);

        if (skill.kind == Kind.Active) data.equipped.RemoveAll(k => k.StartsWith($"{skill.group}.Upgrade."));

        Save();
        OnChanged?.Invoke();
    }

    public static SkillState GetSkillState(string group)
    {
        bool hasActive = data.equipped.Any(k => k.StartsWith($"{group}.Active."));
        bool hasUpgrade1 = data.equipped.Contains($"{group}.Upgrade.1");
        bool hasUpgrade2 = data.equipped.Contains($"{group}.Upgrade.2");

        if (!hasActive) return SkillState.Locked;
        if (hasUpgrade1) return SkillState.UpgradeA;
        if (hasUpgrade2) return SkillState.UpgradeB;
        return SkillState.Basic;
    }

    public static List<string> GetEquippedPassives()
    {
        return data.equipped.Where(k => k.Contains(".Passive.")).ToList();
    }

    public static void ApplyEquippedPassives(EntityStats target)
    {
        if (target == null) return;

        target.UnequipPassive(null);

        string key = data.equipped.FirstOrDefault(k => k.Contains(".Passive."));
        if (string.IsNullOrEmpty(key)) return;

        PassiveSkill passive = PassiveSkillManager.GetPassive(key);
        if (passive == null) return;

        target.EquipPassive(passive);
        Debug.Log($"[EquippedSkills] 패시브 적용됨: {passive.skillName}");
    }

    //모든 장착된 스킬 조회
    public static IReadOnlyList<string> GetAllEquipped()
    {
        return data.equipped;
    }

    //저장
    static void Save()
    {
        try
        {
            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(FilePath, json);
        }
        catch (Exception e)
        {
            Debug.LogError("[EquippedSkills] 저장 실패: " + e);
        }
    }

    //로드
    static void Load()
    {
        try
        {
            if (File.Exists(FilePath))
            {
                string json = File.ReadAllText(FilePath);
                EquippedSkillData loaded = JsonUtility.FromJson<EquippedSkillData>(json);
                data = (loaded != null) ? loaded : new EquippedSkillData();
                return;
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning("[EquippedSkills] 불러오기 실패, 리셋: " + e);
        }

        data = new EquippedSkillData();
    }
}

public enum SkillState
{
    Locked,    // 액티브 미장착
    Basic,     // 액티브만 장착
    UpgradeA,  // 강화A 장착
    UpgradeB   // 강화B 장착
}
