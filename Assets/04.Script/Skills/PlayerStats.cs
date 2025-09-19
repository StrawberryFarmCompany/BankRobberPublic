using System.Collections.Generic;

public class PlayerStats
{
    public string characterName;
    public int actionPoint;
    public int curActionPoint;
    public int movementSpeed;
    public int movement;
    public int maxHp;
    public int curHp;
    public int evasionRate;
    public int accuracyModifier;
    public float attackRange;
    public float detectingDistance;
    public int sabotage;
    public int aggroControl;
    public int maxRerollCount;
    public int curRerollCount;

    private PassiveSkill equippedPassive;

    public PlayerStats(EntityData baseStats)
    {
        characterName = baseStats.displayName;
        actionPoint = baseStats.maxActionPoint;
        curActionPoint = baseStats.curActionPoint;
        movementSpeed = baseStats.movementSpeed;
        movement = baseStats.movementPoint;
        maxHp = baseStats.maxHp;
        curHp = baseStats.curHp;
        evasionRate = baseStats.evasionRate;
        accuracyModifier = baseStats.accuracyModifier;
        attackRange = baseStats.attackRange;
        detectingDistance = baseStats.detectingDistance;
        sabotage = baseStats.sabotage;
        aggroControl = baseStats.aggroControl;
        maxRerollCount = baseStats.maxRerollCount;
        curRerollCount = baseStats.curRerollCount;
    }

    public void EquipPassive(PassiveSkill skill)
    {
        // 이미 다른 패시브가 장착돼 있다면 먼저 해제
        if (equippedPassive != null)
        {
            equippedPassive.Remove(this);
        }

        equippedPassive = skill;
        equippedPassive.Apply(this);
    }

    public void UnequipPassive(PassiveSkill skill)
    {
        if (equippedPassive != null)
        {
            equippedPassive.Remove(this);
            equippedPassive = null;
        }
    }

    public bool IsHit(float diceSum, float weaponRangeBonus, PlayerStats target)
    {
        return GetShotRate(diceSum, weaponRangeBonus, target) >= 0;
    }
    public float GetShotRate(float diceSum,float weaponRangeBonus,PlayerStats target)
    {
        return diceSum + accuracyModifier - weaponRangeBonus - target.evasionRate;
    }
}
