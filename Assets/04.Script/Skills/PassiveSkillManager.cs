using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PassiveSkillManager : MonoBehaviour
{
    public static readonly List<PassiveSkill> AllPassives = new()
    {
        //전투
        new PassiveSkill(
            "Combat.Passive.1",
            "체력 +2",
            increaseActionPoint: 0,
            increaseMoveSpeed: 0,
            increaseMovement: 0,
            increaseMaxHp: 2,
            increaseEvasionRate: 0,
            increaseAccuracyModifier: 0,
            increaseSabotage: 0,
            increaseAggroControl: 0
        ),
        new PassiveSkill(
            "Combat.Passive.2",
            "체력 & 이동력 증가 +1",
            increaseActionPoint: 0,
            increaseMoveSpeed: 0,
            increaseMovement: 1,
            increaseMaxHp: 1,
            increaseEvasionRate: 0,
            increaseAccuracyModifier: 0,
            increaseSabotage: 0,
            increaseAggroControl: 0
        ),

        //잠입
        new PassiveSkill(
            "Stealth.Passive.1",
            "이동력 +2",
            increaseActionPoint: 0,
            increaseMoveSpeed: 0,
            increaseMovement: 2,
            increaseMaxHp: 0,
            increaseEvasionRate: 0,
            increaseAccuracyModifier: 0,
            increaseSabotage: 0,
            increaseAggroControl: 0
        ),
        new PassiveSkill(
            "Stealth.Passive.2",
            "명중 보정치 +2",
            increaseActionPoint: 0,
            increaseMoveSpeed: 0,
            increaseMovement: 0,
            increaseMaxHp: 0,
            increaseEvasionRate: 0,
            increaseAccuracyModifier: 2,
            increaseSabotage: 0,
            increaseAggroControl: 0
        ),

        //도움
        new PassiveSkill(
            "Support.Passive.1",
            "행동력 +1",
            increaseActionPoint: 1,
            increaseMoveSpeed: 0,
            increaseMovement: 0,
            increaseMaxHp: 0,
            increaseEvasionRate: 0,
            increaseAccuracyModifier: 0,
            increaseSabotage: 0,
            increaseAggroControl: 0
        ),
        new PassiveSkill(
            "Support.Passive.2",
            "사보타지 +2",
            increaseActionPoint: 0,
            increaseMoveSpeed: 0,
            increaseMovement: 0,
            increaseMaxHp: 0,
            increaseEvasionRate: 0,
            increaseAccuracyModifier: 0,
            increaseSabotage: 2,
            increaseAggroControl: 0
        ),
    };

    //스킬 키 검색
    public static PassiveSkill GetPassive(string key)
    {
        return AllPassives.FirstOrDefault(p => p.skillName == key);
    }
}
