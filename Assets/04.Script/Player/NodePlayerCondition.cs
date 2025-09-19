using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class NodePlayerCondition : MonoBehaviour
{
    public NodePlayerController nodePlayerController;

    [SerializeField] private EntityData playerData; // 플레이어 데이터 (인스펙터에 할당)

    [Header("플레이어 상태")]
    public string displayName;     //이름
    public string description;     //설명
    public int maxActionPoint;     //최대 행동력
    public int curActionPoint;     //현재 행동력
    public int maxMovement;      //이동속도(최대 이동력)
    public int curMovement;      //이동력
    public int maxHp;              //전체 체력
    public int curHp;              //현재 체력
    public int evasionRate;        //회피율(기본 회피율 7)
    public int accuracyModifier;   //명중 보정치(처음엔 다 0 추후 업그레이드 느낌)
    public float attackRange;      //공격 범위(초기값 5.25)
    public int sabotage;           //손재주(함정해제)
    public int aggroControl;       //장악력
    public int maxRerollCount;     //최대 리롤 횟수
    public int curRerollCount;     //현재 리롤 횟수
    
    public int moveRange;        //이동 범위

    private void Awake()
    {
        displayName = playerData.displayName;                 //이름
        description = playerData.description;                 //설명

        maxActionPoint = playerData.maxActionPoint;           //최대 행동력
        curActionPoint = maxActionPoint;                      //현재 행동력

        maxMovement = playerData.movementSpeed;               //이동속도(최대 이동력)
        curMovement = maxMovement;                            //이동력

        maxHp = playerData.maxHp;                             //전체 체력
        curHp = maxHp;                                        //현재 체력

        evasionRate = playerData.evasionRate;                 //회피율(기본 회피율 7)

        accuracyModifier = playerData.accuracyModifier;       //명중 보정치(처음엔 다 0 추후 업그레이드 느낌)

        attackRange = playerData.attackRange;                 //공격 범위(초기값 5.25)

        sabotage = playerData.sabotage;                       //손재주(함정해제)
        aggroControl = playerData.aggroControl;               //장악력
        maxRerollCount = playerData.maxRerollCount;           //최대 리롤 횟수
        curRerollCount = maxRerollCount;                      //현재 리롤 횟수

        moveRange = 2;                 //이동 범위


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
        if (curMovement >= amount)
        {
            curMovement -= amount;
            return true; // 이동 성공
        }
        return false; // 이동 실패, 이동력이 부족함
    }

    public void ActiveRun()
    {
        if(ConsumeActionPoint(1)) curMovement += maxMovement; // 달리기 활성화 시 이동력 증가
    }

    public void Damaged(int damage)
    {
        curHp -= damage;
        if (curHp <= 0)
        {
            curHp = 0;
            Dead();
        }
    }

    private void Dead()
    {
        //GameManager.GetInstance.사망으로 인해 발생할 게임내 상황을 정의
        Destroy(this.gameObject);
    }


}
