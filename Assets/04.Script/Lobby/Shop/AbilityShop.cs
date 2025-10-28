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
    [SerializeField] private GameObject upgradeButtonPrefab;
    [SerializeField] private TextMeshProUGUI moneyText;     // 돈 표시 텍스트

    [Header("스킬 데이터")]
    [SerializeField] private List<Skill> allSkills;

    [Header("스킬 영역 매핑")]
    [SerializeField] private List<SkillAreaSet> skillAreaSets;

    [SerializeField] private SkillInfo skillInfo;

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
        EquippedSkills.OnChanged += RefreshAllButtons;
        onMoneyChanged = (int value) => RefreshMoney();
        Money.GetInstance.OnChanged += onMoneyChanged;
    }

    void OnDestroy()
    {
        AbilityPurchases.OnChanged -= RefreshAllButtons;
        EquippedSkills.OnChanged -= RefreshAllButtons;
        if (onMoneyChanged != null) Money.GetInstance.OnChanged -= onMoneyChanged;
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
            GameObject prefabToUse = (skill.kind == Kind.Upgrade) ? upgradeButtonPrefab : skillButtonPrefab;

            GameObject btnObj = Instantiate(prefabToUse, parent);
            Button btn = btnObj.GetComponent<Button>();
            TMP_Text label = btnObj.GetComponentInChildren<TMP_Text>();

            string key = skill.GetKey();
            int price = AbilityPurchases.GetPrice(skill);

            label.text = $"{skill.title}";
            buttonMap[key] = btn;

            btn.onClick.AddListener(() => OnClickPurchase(skill, btn));

            SkillInfoTrigger trigger = btnObj.AddComponent<SkillInfoTrigger>();
            trigger.Init(skill, skillInfo);
        }
    }

    //구매 버튼 클릭 처리
    private void OnClickPurchase(Skill skill, Button btn)
    {
        string key = skill.GetKey();
        int price = AbilityPurchases.GetPrice(skill);

        bool purchased = AbilityPurchases.IsPurchased(key);
        bool equipped = EquippedSkills.IsEquipped(key);

        if (!purchased)
        {
            if (!AbilityPurchases.TryPurchase(key, price))
            {
                Debug.Log($"[AbilityShop] 돈 없엉: {skill.title}");
                return;
            }
        }
        else //장착/해제
        {
            if (!equipped)
            {
                EquippedSkills.Equip(skill);
            }
            else
            {
                EquippedSkills.Unequip(skill);
            }
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
        bool equipped = EquippedSkills.IsEquipped(key);

        if (!purchased)
        {
            label.text = $"{skill.title}";
            bgImage.color = Color.white;
            btn.interactable = true;
        }
        else if (equipped)
        {
            label.text = $"{skill.title}\n장착중";
            bgImage.color = new Color(0.75f, 0.75f, 0.75f);
            btn.interactable = true;
        }
        else
        {
            label.text = $"{skill.title}\n장착하기";
            bgImage.color = new Color(0.85f, 0.95f, 1f);
            btn.interactable = true;
        }
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
            moneyText.text = $"{Money.GetInstance.MoneyValue:N0}";
    }
}

[Serializable]
public class SkillAreaSet
{
    public Group group;
    public Kind kind;
    public Transform area;
}
