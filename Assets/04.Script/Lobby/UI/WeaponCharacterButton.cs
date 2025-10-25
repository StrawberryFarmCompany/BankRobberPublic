using UnityEngine;
using UnityEngine.UI;

public class WeaponCharacterButton : MonoBehaviour
{
    public WeaponShopUI main;
    public CharacterType characterType;

    public void Init(WeaponShopUI mainUI)
    {
        main = mainUI;
    }


    public void OnPressPurchaseBtn()
    {
        //자체 바인딩, 무기 구매
        main.Purchase(characterType);
        main.SetCharacterButton(characterType);
    }

    public void OnPressEquipBtn()
    {
        //자체 바인딩, 장착
        main.Equip(characterType);
        main.SetCharacterButton(characterType);
    }

}
