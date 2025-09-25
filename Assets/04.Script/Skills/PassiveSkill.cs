public class PassiveSkill
{
    public string skillName;
    public string description;
    public int increaseActionPoint;
    public int increaseMovementSpeed;
    public int increaseMovement;
    public int increaseMaxHp;
    public int increaseEvasionRate;
    public int increaseAccuracyModifier;
    public int increaseSabotage;
    public int increaseAggroControl;

    public PassiveSkill(string skillName, string description, int increaseActionPoint, int increaseMoveSpeed, int increaseMovement, int increaseMaxHp, int increaseEvasionRate, int increaseAccuracyModifier, int increaseSabotage, int increaseAggroControl)
    {
        this.skillName = skillName;
        this.description = description;
        this.increaseActionPoint = increaseActionPoint;
        this.increaseMovementSpeed = increaseMoveSpeed;
        this.increaseMovement = increaseMovement;
        this.increaseMaxHp = increaseMaxHp;
        this.increaseEvasionRate = increaseEvasionRate;
        this.increaseAccuracyModifier = increaseAccuracyModifier;
        this.increaseSabotage = increaseSabotage;
        this.increaseAggroControl = increaseAggroControl;
    }

    public void Apply(EntityStats target)
    {
        target.actionPoint += increaseActionPoint;
        target.movementSpeed += increaseMovementSpeed;
        target.movement += increaseMovement;
        target.maxHp += increaseMaxHp;
        target.evasionRate += increaseEvasionRate;
        target.accuracyModifier += increaseAccuracyModifier;
        target.sabotage += increaseSabotage;
        target.aggroControl += increaseAggroControl;
    }

    public void Remove(EntityStats target)
    {
        target.actionPoint -= increaseActionPoint;
        target.movementSpeed -= increaseMovementSpeed;
        target.movement -= increaseMovement;
        target.maxHp -= increaseMaxHp;
        target.evasionRate -= increaseEvasionRate;
        target.accuracyModifier -= increaseAccuracyModifier;
        target.sabotage -= increaseSabotage;
        target.aggroControl -= increaseAggroControl;
    }
}
