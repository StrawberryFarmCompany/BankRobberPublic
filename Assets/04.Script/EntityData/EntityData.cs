using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EntityTag
{
    bankManager,    //은행매니저
    enemy,          //적 기물
    citizen,        //시민
    ally,           //플레이어 기물
    CCTV            //씨씨티비
}

public enum PlayerSkill
{
    SneakAttack,    //은신 공격
    Heal,       //치유
    Ready,          //준비

}

[CreateAssetMenu(fileName = "Entity",menuName = "New Entity")]
public class EntityData : ScriptableObject
{
    [Header("CommonInfo")]
    public string displayName;     //이름
    public string description;     //설명
    public EntityTag Tag;          //엔티티태그
    public int maxActionPoint;     //최대 행동력
    public int curActionPoint;     //현재 행동력
    public int movementSpeed;      //이동속도(최대 이동력)
    public int movementPoint;      //이동력
    public int maxHp;              //전체 체력
    public int curHp;              //현재 체력
    public int evasionRate;        //회피율(기본 회피율 7)
    public int accuracyModifier;   //명중 보정치(처음엔 다 0 추후 업그레이드 느낌)
    public float attackRange;      //공격 범위(초기값 5.25)

    [Header("NPCOnly")]
    public float detectingDistance;//감지 거리
    public bool isCombat;          //NPC 공격 가능 여부

    [Header("PlayerOnly")]
    public PlayerSkill playerSkill; //플레이어 스킬
    public int sabotage;           //손재주(함정해제)
    public int aggroControl;       //장악력
    public int maxRerollCount;     //최대 리롤 횟수
    public int curRerollCount;     //현재 리롤 횟수

    public Sprite portrait;
}
