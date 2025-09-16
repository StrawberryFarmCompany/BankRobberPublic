using System.Collections.Generic;

public class PlayerStats
{
    public string characterName;
    public int actionPoint;
    public int movementSpeed;
    public int movement;
    public int maxHp;
    public int evasionRate;
    public int accuracyModifier;
    public int sabotage;
    public int aggroControl;

    private PassiveSkill equippedPassive;

    public PlayerStats(EntityData baseStats)
    {
        characterName = baseStats.displayName;
        actionPoint = baseStats.maxActionPoint;
        movementSpeed = baseStats.movementSpeed;
        movement = baseStats.movementPoint;
        maxHp = baseStats.maxHp;
        evasionRate = baseStats.evasionRate;
        accuracyModifier = baseStats.accuracyModifier;
        sabotage = baseStats.sabotage;
        aggroControl = baseStats.aggroControl;
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
}
