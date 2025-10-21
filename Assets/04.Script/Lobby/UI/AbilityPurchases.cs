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

    public static bool IsPurchased(string skillId)
    {
        if (string.IsNullOrEmpty(skillId)) return false;
        return data.purchased.Contains(skillId);
    }

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

    public static IReadOnlyList<string> GetAllPurchased()
    {
        return data.purchased;
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
            Debug.LogError("[AbilityPurchases] Save failed: " + e);
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
            Debug.LogWarning("[AbilityPurchases] Load failed, reset empty: " + e);
        }
        data = new AbilityPurchaseData();
    }
}