using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AbilityShop : MonoBehaviour
{
    [Header("특성 상점UI 프리팹")]
    [SerializeField] private GameObject skillButtonPrefab;  // 버튼 프리팹
    [SerializeField] private TextMeshProUGUI moneyText;     // 돈 표시 텍스트

    [Header("부모 오브젝트")]
    [SerializeField] private Transform combatParent;
    [SerializeField] private Transform stealthParent;
    [SerializeField] private Transform supportParent;

    [Header("스킬 데이터")]
    [SerializeField] private List<Skill> allSkills;

    private readonly Dictionary<string, Button> buttonMap = new();

    private Action<int> onMoneyChanged;

    void Start()
    {
        GenerateButtons();
        RefreshAllButtons();
        RefreshMoney();

        AbilityPurchases.OnChanged += RefreshAllButtons;
        onMoneyChanged = (int value) => RefreshMoney();
        Money.OnChanged += onMoneyChanged;
    }

    void OnDestroy()
    {
        AbilityPurchases.OnChanged -= RefreshAllButtons;
        if (onMoneyChanged != null) Money.OnChanged -= onMoneyChanged;
    }

    //버튼 동적 생성
    private void GenerateButtons()
    {
        foreach (Skill skill in allSkills)
        {
            Transform parent = GetParent(skill.group);
            if (parent == null) continue;

            GameObject btnObj = Instantiate(skillButtonPrefab, parent);
            Button btn = btnObj.GetComponent<Button>();
            TMP_Text label = btnObj.GetComponentInChildren<TMP_Text>();

            string key = skill.GetKey();
            int price = AbilityPurchases.GetPrice(skill);

            label.text = $"{skill.title}\n${price:N0}";
            buttonMap[key] = btn;

            btn.onClick.AddListener(() => OnClickPurchase(skill, btn));
        }
    }

    //그룹별 부모 오브젝트 반환
    private Transform GetParent(Group group)
    {
        switch (group)
        {
            case Group.Combat: return combatParent;
            case Group.Stealth: return stealthParent;
            case Group.Support: return supportParent;
            default: return null;
        }
    }

    //구매 버튼 클릭 처리
    private void OnClickPurchase(Skill skill, Button btn)
    {
        string key = skill.GetKey();
        int price = AbilityPurchases.GetPrice(skill);

        //이미 구매된 스킬
        if (AbilityPurchases.IsPurchased(key))
        {
            Debug.Log($"[AbilityShop] 이미 구매: {skill.title}");
            return;
        }

        //돈 부족
        if (!AbilityPurchases.TryPurchase(key, price))
        {
            Debug.Log($"[AbilityShop] 돈 없엉 {skill.title}");
            return;
        }

        RefreshButtonState(skill, btn);
        RefreshMoney();
    }

    //개별 버튼 상태 갱신
    private void RefreshButtonState(Skill skill, Button btn)
    {
        string key = skill.GetKey();
        TMP_Text label = btn.GetComponentInChildren<TMP_Text>();
        Image bgImage = btn.GetComponent<Image>();

        bool purchased = AbilityPurchases.IsPurchased(key);

        label.text = GetButtonLabel(skill, purchased);
        btn.interactable = !purchased;

        //구매 상태에 따른 버튼 색상 변경
        if (bgImage != null)
        {
            if (purchased)
                bgImage.color = new Color(0.8f, 0.8f, 0.8f, 1f);
            else
                bgImage.color = Color.white;
        }
    }

    private string GetButtonLabel(Skill skill, bool purchased)
    {
        if (purchased)
            return $"{skill.title}\n구매완료";
        else
            return $"{skill.title}\n${AbilityPurchases.GetPrice(skill):N0}";
    }

    //전체 버튼 갱신
    private void RefreshAllButtons()
    {
        foreach (Skill skill in allSkills)
        {
            if (!buttonMap.TryGetValue(skill.GetKey(), out var btn))
                continue;
            RefreshButtonState(skill, btn);
        }
    }

    //돈 UI 갱신
    private void RefreshMoney()
    {
        if (moneyText != null)
            moneyText.text = $"{Money.Value:N0}";
    }
}
