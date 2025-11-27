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
        WeaponManager.GetInstance.LoadWeapon();
        ScoreManager.GetInstance.LoadScores();
    }

    public static void ResetAll()
    {
        ResetMoney();
        ResetPurchasedSkills();
        ResetEquippedSkills();
        ResetWeapons();
        ResetScores();
        ResetNPCStates();
    }

    public static bool HasAnySave()
    {
        return File.Exists(MoneyFilePath())
            || File.Exists(AbilityFilePath())
            || File.Exists(EquippedFilePath())
            || File.Exists(WeaponFilePath())
            || File.Exists(ScoreFilePath())
            || File.Exists(NPCStatesFilePath());
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

    static void ResetWeapons()
    {
        if (File.Exists(WeaponFilePath()))
            File.Delete(WeaponFilePath());

        WeaponManager.GetInstance.LoadWeapon();
    }

    static void ResetScores()
    {
        if (File.Exists(ScoreFilePath()))
            File.Delete(ScoreFilePath());

        ScoreManager.GetInstance.LoadScores();
    }

    static void ResetNPCStates()
    {
        string npcPath = NPCStatesFilePath();
        if (File.Exists(npcPath))
            File.Delete(npcPath);

        if (NPCDialogueDataManager.Instance != null)
        {
            NPCDialogueDataManager.Instance.npcDialogueStates.Clear();
        }
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

    static string WeaponFilePath()
    {
        return Path.Combine(Application.persistentDataPath, "weapons.json");
    }

    static string ScoreFilePath()
    {
        return Path.Combine(Application.persistentDataPath, "scores.json");
    }

    static string NPCStatesFilePath()
    {
        return Path.Combine(Application.persistentDataPath, "npcStates.json");
    }
}
