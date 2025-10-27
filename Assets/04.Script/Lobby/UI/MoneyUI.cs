using UnityEngine;
using TMPro;

public class MoneyUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI moneyText;

    private void Start()
    {
        Money.moneyUI = this;
        UpdateGoldText();
    }

    public void UpdateGoldText()
    {
        int amount = Money.Get();
        moneyText.text = amount.ToString("N0");
    }


}