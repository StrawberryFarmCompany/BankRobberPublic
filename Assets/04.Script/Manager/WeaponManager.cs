using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using JetBrains.Annotations;

//[Serializable]
//public class WeaponTypeAndKey
//{
//    public GunType gunType;
//    public int gunKey;
//}

[Serializable]
public class WeaponEquipData
{
    public CharacterType characterType;
    public string equipGunData;
    public List<string> purchasedData = new List<string>();
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
        LoadWeapon();
    }

    public void EquipWeapon(CharacterType characterType, string gun)
    {
        if (!WeaponDataDict.ContainsKey(characterType))
        {
            WeaponDataDict[characterType] = new WeaponEquipData
            {
                characterType = characterType,
                equipGunData = "1911",
                purchasedData = new List<string>()
            };
            WeaponDataDict[characterType].purchasedData.Add("1911");
        }

        WeaponDataDict[characterType].equipGunData = gun;
        SaveWeapon();
    }

    public GunData GetEquipData(CharacterType characterType)
    {
        return Resources.Load<GunData>("Guns/" + (WeaponDataDict.TryGetValue(characterType, out var data) ? data.equipGunData : null));
    }

    public bool IsPurcahedWeapon(CharacterType characterType, string gun)
    {
        return WeaponDataDict.TryGetValue(characterType, out var data) && data.purchasedData.Contains(gun);
    }

    public void SetPurchase(CharacterType characterType, string gun)
    {
        if (!WeaponDataDict.ContainsKey(characterType))
        {
            WeaponDataDict[characterType] = new WeaponEquipData
            {
                characterType = characterType,
                equipGunData = "1911",
                purchasedData = new List<string>()
            };
            WeaponDataDict[characterType].purchasedData.Add("1911");
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

    public void LoadWeapon()
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
                    equipGunData = "1911",
                    purchasedData = new List<string>()
                };
                WeaponDataDict[type].purchasedData.Add("1911");
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
