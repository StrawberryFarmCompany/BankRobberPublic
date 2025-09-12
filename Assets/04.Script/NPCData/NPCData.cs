using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NPCType
{
    banker,         //은행원
    bouncer,        //경비원
    police,         //경찰
    citizen,        //시민
    bankPresident,  //은행장
    bankManager     //은행매니저
}

[CreateAssetMenu(fileName = "NPC",menuName = "New NPC")]
public class NPCData : ScriptableObject
{
    [Header("Info")]
    public string displayName;    //이름
    public string description;    //설명
    public NPCType type;          //NPC타입
    public int maxHp;             //전체 체력
    public int curHp;             //현재 체력
    public float movementSpeed;   //이동속도
    public bool isCombat;         //NPC 공격 가능 여부
    public float accuracy;        //명중률
    public float sensingDistance; //감지 거리
}
