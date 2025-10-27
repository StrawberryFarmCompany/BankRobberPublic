using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Group { Combat, Stealth, Support }  //전투,잠입,도움
public enum Kind { Active, Passive, Upgrade }   //액티브,패시브,강화

[CreateAssetMenu(fileName = "Skill", menuName = "Shop/Skill")]
public class Skill : ScriptableObject
{
    [Header("카테고리")]
    public Group group;     //전투,잠입,도움
    public Kind kind;       //액티브,패시브,강화

    [Header("표시 정보")]
    public string title = "체력 회복";           //스킬 이름
    [TextArea(2, 4)] public string effect;      //설명
    [TextArea(1, 2)] public string coolTime;    //쿨타임
    [TextArea(1, 2)] public string cost;        //소모 자원

    [Header("가격 자동 설정")]
    [SerializeField, HideInInspector] private int price;

    public int idNum = 1;

    public int Price
    {
        get
        {
            switch (kind)
            {
                case Kind.Active: return 10000;
                case Kind.Passive: return 40000;
                case Kind.Upgrade: return 100000;
                default: return 0;
            }
        }
    }

    public string GetKey()
    {
        //"Combat.Active.1"
        return $"{group}.{kind}.{idNum}";
    }
}