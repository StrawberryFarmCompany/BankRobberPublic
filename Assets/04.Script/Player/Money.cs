using System;
using UnityEngine;
using System.IO;
using TMPro;

class MoneyData
{
    public int money = 0;
}

public static class Money
{
    public static int Value { get; private set; }

    public static event Action<int> OnChanged;

    const string FileName = "money.json";
    static string FilePath => Path.Combine(Application.persistentDataPath, FileName);

    public static MoneyUI moneyUI;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Init()
    {
        Load();
        OnChanged?.Invoke(Value);
    }

    public static void Set(int amount)
    {
        if (amount < 0) amount = 0;
        Value = amount;
        Save();
        OnChanged?.Invoke(Value);
        moneyUI.UpdateGoldText();
    }

    public static void Add(int amount)
    {
        if (amount <= 0) return;
        long sum = (long)Value + amount;
        Value = (sum > int.MaxValue) ? int.MaxValue : (int)sum;
        Save();
        OnChanged?.Invoke(Value);
        moneyUI.UpdateGoldText();
    }

    public static int Get()
    {
        Load();
        return Value;
    }

    public static bool TrySpend(int price)
    {
        if (price <= 0) return true;
        if (Value < price) return false;
        Value -= price;
        Save();
        OnChanged?.Invoke(Value);
        moneyUI.UpdateGoldText();
        return true;
    }

    static void Save()
    {
        try
        {
            string json = JsonUtility.ToJson(new MoneyData { money = Value });
            File.WriteAllText(FilePath, json);
        }
        catch (Exception e)
        {
            Debug.LogError("[Money] 저장 실패: " + e);
        }
    }

    static void Load()
    {
        try
        {
            if (File.Exists(FilePath))
            {
                string json = File.ReadAllText(FilePath);
                MoneyData data = JsonUtility.FromJson<MoneyData>(json);
                Value = (data != null) ? Mathf.Max(0, data.money) : 0;
                return;
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning("[Money] 불러오기 실패, 리셋: " + e);
        }
        Value = 0;
        moneyUI.UpdateGoldText();
    }
}

