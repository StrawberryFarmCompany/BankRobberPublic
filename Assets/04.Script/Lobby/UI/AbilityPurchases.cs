using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
class AbilityPurchaseData
{
    public List<string> purchased = new List<string>();
}

public static class AbilityPurchases
{
    public static event Action OnChanged;

    const string FileName = "skills.json";
    static string FilePath => Path.Combine(Application.persistentDataPath, FileName);

    static AbilityPurchaseData data = new AbilityPurchaseData();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Init()
    {
        Load();
        OnChanged?.Invoke();
    }

    //스킬 구매 여부
    public static bool IsPurchased(string skillId)
    {
        if (string.IsNullOrEmpty(skillId)) return false;
        return data.purchased.Contains(skillId);
    }

    //스킬 구매 시도
    public static bool TryPurchase(string skillId, int price)
    {
        if (string.IsNullOrEmpty(skillId)) return false;
        if (IsPurchased(skillId)) return true;

        if (!Money.TrySpend(price)) return false;

        data.purchased.Add(skillId);
        Save();
        OnChanged?.Invoke();
        return true;
    }

    //구매된 스킬 목록 조회
    public static IReadOnlyList<string> GetAllPurchased()
    {
        return data.purchased;
    }

    //가격 계산
    public static int GetPrice(Skill skill)
    {
        if (skill == null) return 0;
        return Mathf.Max(0, skill.Price);
    }

    static void Save()
    {
        try
        {
            string json = JsonUtility.ToJson(data);
            File.WriteAllText(FilePath, json);
        }
        catch (Exception e)
        {
            Debug.LogError("[AbilityPurchases] 저장 실패: " + e);
        }
    }

    static void Load()
    {
        try
        {
            if (File.Exists(FilePath))
            {
                string json = File.ReadAllText(FilePath);
                AbilityPurchaseData loaded = JsonUtility.FromJson<AbilityPurchaseData>(json);
                data = (loaded != null) ? loaded : new AbilityPurchaseData();
                return;
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning("[AbilityPurchases] 불러오기 실패, 리셋: " + e);
        }
        data = new AbilityPurchaseData();
    }
}