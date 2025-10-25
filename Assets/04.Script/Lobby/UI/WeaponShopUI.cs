using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponShopUI : MonoBehaviour
{
    public CharacterType[] characterTypes = {CharacterType.Bishop, CharacterType.Rook, CharacterType.Knight};
    [SerializeField] GunData[] gunDatas;
    [SerializeField] GameObject buttonPrefab;

    [SerializeField] Transform hgParent;
    [SerializeField] Transform smgParent;
    [SerializeField] Transform sgParent;
    [SerializeField] Transform arParent;
    [SerializeField] Transform srParent;

    public TextMeshProUGUI weaponTitle;
    public TextMeshProUGUI dpbText;
    public TextMeshProUGUI bpsText;
    public TextMeshProUGUI maxRoundText;
    public TextMeshProUGUI firstRange;
    public TextMeshProUGUI firstAccuracy;
    public TextMeshProUGUI secondRange;
    public TextMeshProUGUI secondAccuracy;
    public TextMeshProUGUI thirdRange;
    public TextMeshProUGUI thirdAccuracy;
    public TextMeshProUGUI awayAccuracy;
    public TextMeshProUGUI featureText;

    private int curIndex;

    public Button bishopButton;
    public TextMeshProUGUI bishopText;
    public WeaponCharacterButton bishopWCB;
    public Button rookButton;
    public TextMeshProUGUI rookText;
    public WeaponCharacterButton rookWCB;
    public Button knightButton;
    public TextMeshProUGUI knightText;
    public WeaponCharacterButton knightWCB;

    //private void OnEnable()
    //{
    //    DisplayGunData(0);
    //}

    private void Start()
    {
        for (int i = 0; i < gunDatas.Length; i++)
        {
            GunData data = gunDatas[i];
            Transform parent = null;

            switch (data.type)
            {
                case GunType.HandGun: parent = hgParent; break;
                case GunType.SubMachineGun: parent = smgParent; break;
                case GunType.ShotGun: parent = sgParent; break;
                case GunType.AssaultRifle: parent = arParent; break;
                case GunType.SniperRifle: parent = srParent; break;
            }

            if (parent != null)
                CreateButton(i, parent);
        }
        bishopWCB.Init(this);
        rookWCB.Init(this);
        knightWCB.Init(this);

    }

    private void CreateButton(int index, Transform parent)
    {
        GameObject btnObject = Instantiate(buttonPrefab, parent);
        WeaponShopButton btn = btnObject.GetComponent<WeaponShopButton>();

        // 버튼에 인덱스만 넘김
        btn.Initialize(this, index);
        btn.SetImage(gunDatas[index].WeaponSprite);
    }

    public void DisplayGunData(int index)
    {
        GunData gunData = gunDatas[index];

        weaponTitle.text = gunData.gunName;
        dpbText.text = $"{gunData.damagePerOneBulletMultiplier}";
        bpsText.text = $"{gunData.bulletPerOneShot}";
        maxRoundText.text = $"{gunData.maxRounds}";
        firstRange.text = $"0 ~ {gunData.firstRange}";
        firstAccuracy.text = $"{gunData.firstRangeAccuracy}";
        secondRange.text = $"{gunData.firstRange + 1} ~ {gunData.secondRange}";
        secondAccuracy.text = $"{gunData.secondRangeAccuracy}";
        thirdRange.text = $"{gunData.secondRange + 1} ~ {gunData.thirdRange}";
        thirdAccuracy.text = $"{gunData.thirdRangeAccuracy}";
        awayAccuracy.text = $"{gunData.awayRangeAccuracy}";
        featureText.text = gunData.gunDescription;
        curIndex = index;
    }

    public void SetCharacterButton(CharacterType type)
    {
        GunData gunData = gunDatas[curIndex];

        if(!WeaponManager.GetInstance.IsPurcahedWeapon(type, gunData))
        {
            switch (type)
            {
                case CharacterType.Bishop: 
                    bishopButton.onClick.AddListener(bishopWCB.OnPressPurchaseBtn);
                    if (Money.Get() < gunData.price) bishopButton.interactable = false;
                    bishopText.text = $"{gunData.price.ToString("N0")}\n구매하기";
                    bishopText.fontSize = 40;
                    break;
                case CharacterType.Rook:
                    rookButton.onClick.AddListener(rookWCB.OnPressPurchaseBtn);
                    if (Money.Get() < gunData.price) rookButton.interactable = false;
                    rookText.text = $"{gunData.price.ToString("N0")}\n구매하기";
                    rookText.fontSize = 40;
                    break;
                case CharacterType.Knight:
                    knightButton.onClick.AddListener(knightWCB.OnPressPurchaseBtn);
                    if (Money.Get() < gunData.price) knightButton.interactable = false;
                    knightText.text = $"{gunData.price.ToString("N0")}\n구매하기";
                    knightText.fontSize = 40;
                    break;
            }
            
        }
        else
        {
            switch (type)
            {
                case CharacterType.Bishop:
                    bishopButton.onClick.AddListener(bishopWCB.OnPressEquipBtn);
                    if (WeaponManager.GetInstance.GetEquipData(type) == gunData)
                    {
                        bishopButton.interactable = false;
                        bishopText.text = $"장착됨";
                    }
                    else
                    {
                        bishopButton.interactable = true;
                        bishopText.text = $"장착하기";
                    }
                    bishopText.fontSize = 50;
                    break;
                case CharacterType.Rook:
                    rookButton.onClick.AddListener(rookWCB.OnPressEquipBtn);
                    if (WeaponManager.GetInstance.GetEquipData(type) == gunData)
                    {
                        rookButton.interactable = false;
                        rookText.text = $"장착됨";
                    }
                    else
                    {
                        rookButton.interactable = true;
                        rookText.text = $"장착하기";
                    }
                    rookText.fontSize = 50;
                    break;
                case CharacterType.Knight:
                    knightButton.onClick.AddListener(knightWCB.OnPressEquipBtn);
                    if (WeaponManager.GetInstance.GetEquipData(type) == gunData)
                    {
                        knightButton.interactable = false;
                        knightText.text = $"장착됨";
                    }
                    else
                    {
                        knightButton.interactable = true;
                        knightText.text = $"장착하기";
                    }
                    knightText.fontSize = 50;
                    break;
            }
        }
    }

    public void DeBindingButton(CharacterType type)
    {
        switch (type)
        {
            case CharacterType.Bishop:
                bishopButton.onClick.RemoveAllListeners();
                bishopButton.interactable = true;
                break;
            case CharacterType.Rook:
                rookButton.onClick.RemoveAllListeners();
                rookButton.interactable = true;
                break;
            case CharacterType.Knight:
                knightButton.onClick.RemoveAllListeners();
                knightButton.interactable = true;
                break;
        }
    }

    public void Purchase(CharacterType type)
    {
        if (Money.TrySpend(gunDatas[curIndex].price))
        {
            WeaponManager.GetInstance.SetPurchase(type, gunDatas[curIndex]);
        }
    }

    public void Equip(CharacterType type)
    {
        WeaponManager.GetInstance.EquipWeapon(type, gunDatas[curIndex]);
    }
}