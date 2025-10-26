using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[Serializable]
public class WeaponEquipData
{
    public CharacterType characterType;
    public GunData equipGunData;
    public List<GunData> purchasedData = new List<GunData>();
}

[Serializable]
public class WeaponEquipSaveData
{
    public List<WeaponEquipData> weaponDatas = new List<WeaponEquipData>();
}

public class WeaponManager : SingleTon<WeaponManager>
{
    private Dictionary<CharacterType, WeaponEquipData> WeaponDataDict = new Dictionary<CharacterType, WeaponEquipData>();
    private string savePath;
    public GunData basicGun = Resources.Load<GunData>("Guns/" + "1911");

    protected override void Init()
    {
        savePath = Application.persistentDataPath + "/weapons.json";
        LoadScores();
    }

    public void EquipWeapon(CharacterType characterType, GunData gun)
    {
        if (!WeaponDataDict.ContainsKey(characterType))
        {
            WeaponDataDict[characterType] = new WeaponEquipData
            {
                characterType = characterType,
                equipGunData = basicGun,
                purchasedData = new List<GunData>()
            };
            WeaponDataDict[characterType].purchasedData.Add(basicGun);
        }

        WeaponDataDict[characterType].equipGunData = gun;
        SaveWeapon();
    }

    public GunData GetEquipData(CharacterType characterType)
    {
        return WeaponDataDict.TryGetValue(characterType, out var data) ? data.equipGunData : null;
    }

    public bool IsPurcahedWeapon(CharacterType characterType, GunData gun)
    {
        return WeaponDataDict.TryGetValue(characterType, out var data) && data.purchasedData.Contains(gun);
    }

    public void SetPurchase(CharacterType characterType, GunData gun)
    {
        if (!WeaponDataDict.ContainsKey(characterType))
        {
            WeaponDataDict[characterType] = new WeaponEquipData
            {
                characterType = characterType,
                equipGunData = basicGun,
                purchasedData = new List<GunData>()
            };
            WeaponDataDict[characterType].purchasedData.Add(basicGun);
        }
        WeaponDataDict[characterType].purchasedData.Add(gun);
        SaveWeapon();
    }

    public void SaveWeapon()
    {
        var saveData = new WeaponEquipSaveData();
        foreach (var pair in WeaponDataDict)
        {
            saveData.weaponDatas.Add(pair.Value);
        }

        string json = JsonUtility.ToJson(saveData, true);
        File.WriteAllText(savePath, json);
        Debug.Log($"무기 정보 저장 완료 → {savePath}");
    }

    public void LoadScores()
    {
        WeaponDataDict.Clear();

        if (!File.Exists(savePath))
        {
            Debug.Log("저장된 무기정보가 없어 새로 생성합니다.");
            foreach (CharacterType type in Enum.GetValues(typeof(CharacterType)))
            {
                WeaponDataDict[type] = new WeaponEquipData
                {
                    characterType = type,
                    equipGunData = basicGun,
                    purchasedData = new List<GunData>()
                };
                WeaponDataDict[type].purchasedData.Add(basicGun);
            }
            SaveWeapon();
            return;
        }

        string json = File.ReadAllText(savePath);
        var saveData = JsonUtility.FromJson<WeaponEquipSaveData>(json);

        foreach (var data in saveData.weaponDatas)
        {
            WeaponDataDict[data.characterType] = data;
            Debug.Log($"{data.characterType} = {data.equipGunData}, 구매 목록: {data.purchasedData}");
        }

        Debug.Log("무기 정보 불러오기 완료");
    }

}
