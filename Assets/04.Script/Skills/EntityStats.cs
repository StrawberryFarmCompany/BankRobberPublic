using NodeDefines;
using System;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using BuffDefine;
using FOW;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class EntityStats
{
    public EntityTag entityTag;
    public string characterName;
    public int actionPoint;
    public int curActionPoint;
    public int movementSpeed;
    public int movement;
    public int maxHp;
    private float curHp;
    public float CurHp
    { 
        get 
        { 
            return curHp; 
        } 
        set 
        { 
            if(curHp > value)
            {
                OnDamaged?.Invoke();
            }
            curHp = value; 
        } 
    }
    public int evasionRate;
    public int baseEvasionRate;
    public int accuracyModifier;
    public float attackRange;
    public float detectingDistance;
    public int sabotage;
    public int aggroControl;
    public int maxRerollCount;
    public int curRerollCount;
    public Sprite portrait;
    public CharacterType characterType;
    public SkillGroupType skillGroup;
    public Node currNode;

    public PlayerSkill playerSkill; //플레이어 스킬
    public bool hasPermanentAttackBuff = false;

    public SecurityData secData;
    private PassiveSkill equippedPassive;
    private List<IBuff> buffs;
    public List<IBuff> Buffs { get { return buffs; } }
    public Action OnDamaged;
    public Action OnDead;
    public Action<Vector3Int> ForceMove;

    public GameObject thisGameObject;
    public bool isFullBag;
    /// <summary>
    /// 탈출 시 초기화하는 함수 monobehaviour 단에서 구현하여 해당 OnReset Action에 추가
    /// </summary>
    public Action OnReset;

    private HPBar hpbar;

    public EntityStats(EntityData baseStats, GameObject gameObject)
    {
        entityTag = baseStats.Tag;
        characterName = baseStats.displayName;
        actionPoint = baseStats.maxActionPoint;
        curActionPoint = baseStats.curActionPoint;
        movementSpeed = baseStats.movementSpeed;
        movement = baseStats.movementPoint;
        maxHp = baseStats.maxHp;
        CurHp = baseStats.curHp;
        baseEvasionRate = baseStats.evasionRate;
        evasionRate = baseStats.evasionRate;
        accuracyModifier = baseStats.accuracyModifier;
        attackRange = baseStats.attackRange;
        detectingDistance = baseStats.detectingDistance;
        sabotage = baseStats.sabotage;
        aggroControl = baseStats.aggroControl;
        maxRerollCount = baseStats.maxRerollCount;
        curRerollCount = baseStats.curRerollCount;
        playerSkill = baseStats.playerSkill;
        portrait = baseStats.portrait;
        skillGroup = baseStats.skillGroup;
        buffs = new List<IBuff>();



        if (baseStats.characterType != CharacterType.None) 
        {
            this.characterType = baseStats.characterType;
            isFullBag = false;
        }

        if (gameObject != null)
        {
            thisGameObject = gameObject;
        }
    }
    public void CreateHpBar()
    {
        if (characterType == CharacterType.None)
        {
            hpbar = new HPBar();
            hpbar.Init(maxHp, CurHp);
            hpbar.RegistHideOBJ(thisGameObject.AddComponent<HiderDisableObjects>());
        }
    }
    public void RegistBuff(BuffData data)
    {
        IBuff buff = IBuff.Factory(data, this, data.StatusType);
        buffs.Add(buff);
        buff.RegistBuff();
    }
    public void RemoveBuff(IBuff data)
    {
        buffs.Remove(data);
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
        if (ConsumeActionPoint(1)) movement += movementSpeed; // 달리기 활성화 시 이동력 증가
    }

    public void Damaged(float damage)
    {
        CurHp -= damage;
        if (hpbar != null)
        {
            hpbar.SetCurrHP(CurHp);
        }
        if (CurHp <= 0)
        {
            CurHp = 0;
            Dead();
        }
        if(UIManager.GetInstance.pip != null)
        {
            UIManager.GetInstance.pip.RefreshHealth();
        }
    }

    private void Dead()
    {
        GameManager.GetInstance.GatherCostAndScore();
        RemoveHiderObj();
        //thisGameObject.SetActive(false);

        if (characterType != CharacterType.None)
        {
            NodePlayerManager.GetInstance.SetEscapeCondition(this, EscapeCondition.Arrest);
            UIManager.GetInstance.gameEndUI.SetDeadCharacter(this);
        }
        OnDead?.Invoke();
        DestroyEntity();
        //GameManager.GetInstance.사망으로 인해 발생할 게임내 상황을 정의
        if (NodePlayerManager.GetInstance.GetAllPlayers().Count <= 0)
        {
            NodePlayerManager.GetInstance.LateGameEndCall();
        }
    }

    public void HealHealthPoint(float amount)
    {
        CurHp += amount;
        if (CurHp > maxHp)
        {
            CurHp = maxHp;
        }

        if (hpbar != null)
        {
            hpbar.SetCurrHP(CurHp);
        }
    }

    public void HealActionPoint(int amount)
    {
        curActionPoint += amount;
        if (curActionPoint > actionPoint)
        {
            curActionPoint = actionPoint;
        }
    }

    public void HealMovement(int amount)
    {
        movement += amount;
    }

    public void ResetForNewTurn()
    {
        curActionPoint = actionPoint;
        movement = movementSpeed;
        evasionRate = baseEvasionRate;                                  // 매 턴마다 회피율을 기본값으로 리셋 임시적으로 넣은거라서 나중에 바꿔야함
    }
    public void GetTileInteraction(Vector3 pos)
    {
        if (this == NodePlayerManager.GetInstance.GetCurrentPlayer().playerStats)
        {
            UIManager.GetInstance.leftInteractionPanel.OnInteractionRefresh();
        }
    }

    public void NodeUpdates(Vector3 pos,bool forceUpdate = false)
    {
        Vector3Int tempPos = GameManager.GetInstance.GetNode(GameManager.GetInstance.GetVecInt(pos)).GetCenter;

        if (currNode == null || currNode.GetCenter != tempPos || forceUpdate)
        {
            if(currNode != null)currNode.RemoveCharacter(this);
            currNode = GameManager.GetInstance.GetNode(tempPos);
            currNode.AddCharacter(this);
            Debug.Log($"{pos}로 이동");
            if(hpbar != null)hpbar.SetPosition(currNode.GetCenter + Vector3.up * 2);
        }
        FloorCullingManager.GetInstance.UpdateCullingByCurrentPlayer();
    }

    public Vector3Int GetPosition()
    {
        if (currNode == null) return Vector3Int.zero;
        return currNode.GetCenter;
    }
    private void RemoveHiderObj()
    {
        if (thisGameObject.TryGetComponent<FogOfWarHider>(out FogOfWarHider hider))
        {
            HiderBehavior[] hiderObjects = thisGameObject.GetComponents<HiderBehavior>();
            hider.DeregisterHider();
            FogOfWarWorld.Destroy(hider);
            for (int i = 0; i < hiderObjects.Length; i++)
            {
                FogOfWarWorld.Destroy(hiderObjects[i]);
            }
            if (hpbar != null)
            {
                HiderDisableObjects hiderObj = thisGameObject.GetComponent<HiderDisableObjects>();
                hpbar.Destroy();
            }
        }
    }
    public void DestroyEntity()
    {

        OnDamaged = null;
        OnDead = null;
        OnReset = null;
        ForceMove = null;
        UnequipPassive(equippedPassive);
        equippedPassive = null;
        buffs.Clear();
        currNode?.RemoveCharacter(this);
        currNode = null;
        if (characterType != CharacterType.None)
            isFullBag = false;
        GameManager.GetInstance.UnregisterEntity(this);
        //NodePlayerManager.GetInstance.UnregisterPlayer(thisGameObject.GetComponent<NodePlayerController>());
        //GameManager.GetInstance.BattleTurn.RemoveUnit();
    }

}