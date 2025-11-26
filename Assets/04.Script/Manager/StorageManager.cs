using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public static class StorageManager
{
    public static void LoadAll()
    {
        Money.GetInstance.Load();
        AbilityPurchases.Load();
        EquippedSkills.Load();
    }

    public static void ResetAll()
    {
        ResetMoney();
        ResetPurchasedSkills();
        ResetEquippedSkills();
    }

    public static bool HasAnySave()
    {
        return File.Exists(MoneyFilePath()) || File.Exists(AbilityFilePath()) || File.Exists(EquippedFilePath());
    }

    static void ResetMoney()
    {
        if (File.Exists(MoneyFilePath()))
            File.Delete(MoneyFilePath());

        Money.GetInstance.MoneyValue = 0;
        Money.GetInstance.Save();
    }

    static void ResetPurchasedSkills()
    {
        if (File.Exists(AbilityFilePath()))
            File.Delete(AbilityFilePath());

        AbilityPurchases.ClearData();
    }

    static void ResetEquippedSkills()
    {
        if (File.Exists(EquippedFilePath()))
            File.Delete(EquippedFilePath());

        EquippedSkills.ClearData();
    }

    static string MoneyFilePath()
    {
        return Path.Combine(Application.persistentDataPath, "money.json");
    }

    static string AbilityFilePath()
    {
        return Path.Combine(Application.persistentDataPath, "skills.json");
    }

    static string EquippedFilePath()
    {
        return Path.Combine(Application.persistentDataPath, "equippedSkills.json");
    }
}
