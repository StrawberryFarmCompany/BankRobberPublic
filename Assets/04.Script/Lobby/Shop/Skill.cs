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

    [Header("숫자 ID")]
    public int idNum = 1;   //Combat/Active의 1 -> 체력 회복

    [Header("표시 정보")]
    public string title = "체력 회복";
    [TextArea] public string effect;    //설명

    [Header("가격")]
    public int price = -1;  //-1이면 기본가(Active 10000) 사용

    public string GetKey()
    {
        //"Combat.Active.1"
        return $"{group}.{kind}.{idNum}";
    }
}