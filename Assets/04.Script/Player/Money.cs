using System;
using UnityEngine;
using System.IO;
using TMPro;

class MoneyData
{
    public int money = 0;
}

public class Money : SingleTon<Money>
{
    private int moneyValue;
    public int MoneyValue { get { return moneyValue; } set { moneyValue = value; } }

    public event Action<int> OnChanged;

    const string FileName = "money.json";
    static string FilePath => Path.Combine(Application.persistentDataPath, FileName);

    public MoneyUI moneyUI;

    protected override void Init()
    {
        Load();
        OnChanged?.Invoke(MoneyValue);
    }

    public void Set(int amount)
    {
        if (amount < 0) amount = 0;
        MoneyValue = amount;
        Save();
        OnChanged?.Invoke(MoneyValue);
        moneyUI.UpdateGoldText();
    }

    public void Add(int amount)
    {
        if (amount <= 0) return;
        long sum = (long)MoneyValue + amount;
        MoneyValue = (sum > int.MaxValue) ? int.MaxValue : (int)sum;
        Save();
        OnChanged?.Invoke(MoneyValue);
        moneyUI?.UpdateGoldText();
    }

    public int Get()
    {
        Load();
        return MoneyValue;
    }

    public bool TrySpend(int price)
    {
        if (price <= 0) return true;
        if (MoneyValue < price) return false;
        MoneyValue -= price;
        Save();
        OnChanged?.Invoke(MoneyValue);
        moneyUI?.UpdateGoldText();
        return true;
    }

    public void Save()
    {
        try
        {
            string json = JsonUtility.ToJson(new MoneyData { money = MoneyValue });
            File.WriteAllText(FilePath, json);
        }
        catch (Exception e)
        {
            Debug.LogError("[Money] 저장 실패: " + e);
        }
    }

    public void Load()
    {
        try
        {
            if (File.Exists(FilePath))
            {
                string json = File.ReadAllText(FilePath);
                MoneyData data = JsonUtility.FromJson<MoneyData>(json);
                MoneyValue = (data != null) ? Mathf.Max(0, data.money) : 0;
                return;
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning("[Money] 불러오기 실패, 리셋: " + e);
        }
        MoneyValue = 0;
        Save();
        if(moneyUI != null) moneyUI.UpdateGoldText();
    }
}

