using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using UnityEngine;

public enum QuestStatus
{
    NotStarted,
    InProgress,
    CanComplete,
    Completed,
    Failed
}

[Serializable]
public class NpcDialogueChange
{
    public ENPC npc;
    public int newBranch;
    public int newIndex;
}

[CreateAssetMenu(fileName = "NewQuest", menuName = "New Quest")]
public class QuestData : ScriptableObject
{
    [Header("기본 정보")]
    public string questID;
    public string questName;
    [TextArea] public string description;

    [Header("목표")]
    public int requiredAmount;
    public string targetID;
    public int currentAmount;

    [Header("목표 NPC 대화")]
    public ENPC targetNpc; // 퀘스트 목표 NPC
    public int branch; // 퀘스트 목표 대화 분기
    public int index; // 퀘스트 목표 대화 인덱스
    public string targetDialogue; // 퀘스트 목표 대화 내용

    [Header("상태")]
    public QuestStatus status = QuestStatus.NotStarted;

    [Header("결과에 따른 NPC 대화 변화")]
    public List<NpcDialogueChange> successChanges;
    public List<NpcDialogueChange> failChanges;
}
