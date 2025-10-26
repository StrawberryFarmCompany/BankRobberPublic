using UnityEngine;
using UnityEngine.UI;

public class WeaponShopButton : MonoBehaviour
{
    private WeaponShopUI main;
    private int index;
    [SerializeField] private Image gunImage;
    private Button button;

    public void Initialize(WeaponShopUI mainUI, int idx)
    {
        main = mainUI;
        index = idx;

        // 버튼 캐싱
        button = GetComponent<Button>();
        button.onClick.AddListener(OnPressBtn);
    }

    public void SetImage(Sprite sprite)
    {
        gunImage.sprite = sprite;
    }

    private void OnPressBtn()
    {

        main.DisplayGunData(index);
        foreach(var type in main.characterTypes)
        {
            //main.DeBindingButton(type);
            main.SetCharacterButton(type);
        }
    }
}
