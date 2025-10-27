using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

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

[Serializable]
public class CharacterWithGun
{
    public CharacterType type;
    public GunData gun;
}

[CreateAssetMenu(fileName = "NewQuest", menuName = "New Quest")]
public class QuestData : ScriptableObject
{
    [Header("기본 정보")]
    public string questID;
    public string questName;
    [TextArea] public string description;

    [Header("목표 스테이지 및 점수")]
    public SceneType scene;
    public string sceneName;

    public int requiredScore;
    public int currentScore;
    [Tooltip("안 쓸 거면 체크")]
    public bool isScoreCompleted;

    public int requiredSuccesses;
    public int currentSuccesses;
    [Tooltip("안 쓸 거면 체크")]
    public bool isSuccessesCompleted;

    public int requiredPerfects;
    public int currentPerfects;
    [Tooltip("안 쓸 거면 체크")]
    public bool isPerfectsCompleted;

    public int requiredFails;
    public int currentFails;
    [Tooltip("안 쓸 거면 체크")]
    public bool isFailedCompleted;

    public CharacterWithGun[] characterWithGuns; 

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
