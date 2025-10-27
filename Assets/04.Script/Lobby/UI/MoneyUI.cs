using UnityEngine;
using TMPro;

public class MoneyUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI moneyText;

    private void Awake()
    {
        Money.GetInstance.moneyUI = this;
        UpdateGoldText();
    }

    public void UpdateGoldText()
    {
        int amount = Money.GetInstance.Get();
        moneyText.text = amount.ToString("N0");
    }


}