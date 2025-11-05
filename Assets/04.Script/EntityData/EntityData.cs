using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EntityTag
{
    BankManager,    //은행매니저
    Enemy,          //적 기물
    Citizen,        //시민
    Ally,           //플레이어 기물
    CCTV            //씨씨티비
}

public enum PlayerSkill
{
    None,

    SneakAttack,    //은신 공격
    SneakAttack_A,  //성공 시 이동력 회복
    SneakAttack_B,  //성공 확률 증가

    Silence,        //소음 제거
    Silence_A,      //턴 증가
    Silence_B,      //모든 아군 소음 제거

    Heal,       //치유
    Heal_A,     //체력 회복 강화
    Heal_B,     //모든 아군 체력 회복

    DoubleAttack,       //이중 타격
    DoubleAttack_A,     //3 타격
    DoubleAttack_B,     //무기 공격력 보정치 추가

    Ready,          //행동력 회복
    Ready_A,        //회복 강화
    Ready_B,        //모든 아군 행동력 회복

    Evasion,        //회피율 증가
    Evasion_A,      //회피율 추가 증가
    Evasion_B       //모은 아군 회피율 증가
}

public enum CharacterType
{
    None,
    Bishop,
    Rook,
    Knight
}

public enum SkillGroupType
{
    Combat,
    Stealth,
    Support
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
    public CharacterType characterType;
    public PlayerSkill playerSkill; //플레이어 스킬
    public int sabotage;           //손재주(함정해제)
    public int aggroControl;       //장악력
    public int maxRerollCount;     //최대 리롤 횟수
    public int curRerollCount;     //현재 리롤 횟수

    public Sprite portrait;

    [Header("Skill Group")]
    public SkillGroupType skillGroup;
}
