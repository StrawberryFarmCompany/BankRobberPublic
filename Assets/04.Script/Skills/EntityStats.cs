using NodeDefines;
using System;
using System.Collections.Generic;
using UnityEngine;

public class EntityStats
{
    public EntityTag entityTag;
    public string characterName;
    public int actionPoint;
    public int curActionPoint;
    public int movementSpeed;
    public int movement;
    public int maxHp;
    private int curHp;
    public int CurHp
    { 
        get 
        { 
            return curHp; 
        } 
        set 
        { 
            if(curHp > value)
            {
                OnDamaged.Invoke();
            }
            curHp = value; 
        } 
    }
    public int evasionRate;
    public int accuracyModifier;
    public float attackRange;
    public float detectingDistance;
    public int sabotage;
    public int aggroControl;
    public int maxRerollCount;
    public int curRerollCount;
    public int moveRange;
    public Sprite portrait;

    public Node currNode;

    private PassiveSkill equippedPassive;

    public Action OnDamaged;

    public EntityStats(EntityData baseStats)
    {
        entityTag = baseStats.Tag;
        characterName = baseStats.displayName;
        actionPoint = baseStats.maxActionPoint;
        curActionPoint = baseStats.curActionPoint;
        movementSpeed = baseStats.movementSpeed;
        movement = baseStats.movementPoint;
        maxHp = baseStats.maxHp;
        CurHp = baseStats.curHp;
        evasionRate = baseStats.evasionRate;
        accuracyModifier = baseStats.accuracyModifier;
        attackRange = baseStats.attackRange;
        detectingDistance = baseStats.detectingDistance;
        sabotage = baseStats.sabotage;
        aggroControl = baseStats.aggroControl;
        maxRerollCount = baseStats.maxRerollCount;
        curRerollCount = baseStats.curRerollCount;
        moveRange = baseStats.moveRange;
        portrait = baseStats.portrait;
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

    public bool IsHit(float diceSum, float weaponRangeBonus, EntityStats target)
    {
        return GetShotRate(diceSum, weaponRangeBonus, target) >= 0;
    }
    public float GetShotRate(float diceSum,float weaponRangeBonus,EntityStats target)
    {
        return diceSum + accuracyModifier - weaponRangeBonus - target.evasionRate;
    }

    

    public bool ConsumeActionPoint(int amount)
    {
        if (curActionPoint >= amount)
        {
            curActionPoint -= amount;
            return true; // 행동 성공
        }
        return false; // 행동 실패, 행동력이 부족함
    }

    public bool ConsumeMovement(int amount)
    {
        if (movement >= amount)
        {
            movement -= amount;
            return true; // 이동 성공
        }
        return false; // 이동 실패, 이동력이 부족함
    }

    public void ActiveRun()
    {
        Debug.Log($"prevoius: {movement}");
        if (ConsumeActionPoint(1)) movement += movementSpeed; // 달리기 활성화 시 이동력 증가
        Debug.Log($"Run Activated: {movement}");
    }

    public void Damaged(int damage)
    {
        CurHp -= damage;
        if (CurHp <= 0)
        {
            CurHp = 0;
            Dead();
        }
    }

    private void Dead()
    {
        //GameManager.GetInstance.사망으로 인해 발생할 게임내 상황을 정의
        //Destroy(this.gameObject);
    }

    public void ResetForNewTurn()
    {
        curActionPoint = actionPoint;
        movement = movementSpeed;
    }


}
