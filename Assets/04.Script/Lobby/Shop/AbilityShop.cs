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

    [Header("스킬 데이터")]
    [SerializeField] private List<Skill> allSkills;

    [Header("스킬 영역 매핑")]
    [SerializeField] private List<SkillAreaSet> skillAreaSets;

    private readonly Dictionary<(Group, Kind), Transform> skillAreas = new();
    private readonly Dictionary<string, Button> buttonMap = new();
    private Action<int> onMoneyChanged;

    void Awake()
    {
        foreach (var set in skillAreaSets)
        {
            if (set.area == null)
            {
                continue;
            }

            skillAreas[(set.group, set.kind)] = set.area;
        }
    }

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
            if (!skillAreas.TryGetValue((skill.group, skill.kind), out Transform parent))
            {
                continue;
            }

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

[Serializable]
public class SkillAreaSet
{
    public Group group;
    public Kind kind;
    public Transform area;
}
