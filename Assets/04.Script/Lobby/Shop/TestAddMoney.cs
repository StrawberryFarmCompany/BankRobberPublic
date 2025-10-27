using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class TestAddMoney : MonoBehaviour
{
    [SerializeField] private KeyCode addKey = KeyCode.M;
    [SerializeField] private int addAmount = 100000;

    void Update()
    {
        if (Input.GetKeyDown(addKey))
        {
            Money.GetInstance.Add(addAmount);
            Debug.Log($"[테스트] {addAmount:N0}원 추가됨 → 현재 잔액: {Money.GetInstance.MoneyValue:N0}");
        }
    }
}
