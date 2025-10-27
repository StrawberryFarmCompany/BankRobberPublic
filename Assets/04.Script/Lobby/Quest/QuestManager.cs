using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager GetInstance;

    public List<QuestData> activeQuests = new List<QuestData>();

    private void Awake()
    {
        if (GetInstance == null)
        {
            GetInstance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        LoadInitialQuests();
    }

    public void AcceptQuest(QuestData quest)
    {
        if (!activeQuests.Contains(quest))
        {
            quest.status = QuestStatus.InProgress;
            quest.currentScore = 0;
            activeQuests.Add(quest);
            Debug.Log($"퀘스트 수락: {quest.questName}");
        }
    }

    public void CompleteQuest(QuestData quest)
    {
        if (quest.status == QuestStatus.CanComplete)
        {
            quest.status = QuestStatus.Completed;
            activeQuests.Remove(quest);
            Debug.Log($"퀘스트 완료: {quest.questName}");

            ApplyDialogueChanges(quest.successChanges);
        }
    }

    public void FailQuest(QuestData quest)
    {
        if (quest.status != QuestStatus.Completed)
        {
            quest.status = QuestStatus.Failed;
            activeQuests.Remove(quest);
            Debug.Log($"퀘스트 실패: {quest.questName}");

            ApplyDialogueChanges(quest.failChanges);
        }
    }

    public bool IsQuestCompleted(string questID)
    {
        return activeQuests.Exists(q => q.questID == questID && q.status == QuestStatus.Completed);
    }

    /// <summary>
    /// 진행중인 퀘스트의 상태를 반환합니다.
    /// </summary>
    /// <param name="questID"></param>
    /// <returns></returns>
    public QuestStatus GetQuestStatus(string questID)
    {
        var quest = activeQuests.Find(q => q.questID == questID);
        return quest != null ? quest.status : QuestStatus.NotStarted;
    }

    public void ApplyDialogueChanges(List<NpcDialogueChange> changes)
    {
        foreach (var change in changes)
        {
            if (NPCDialogueDataManager.Instance.npcDialogueStates.ContainsKey(change.npc))
            {
                NPCDialogueDataManager.Instance.npcDialogueStates[change.npc] = (change.newBranch, change.newIndex);
            }
            else
            {
                NPCDialogueDataManager.Instance.npcDialogueStates.Add(change.npc, (change.newBranch, change.newIndex));
            }

            Debug.Log($"{change.npc} 대화 상태 변경 → Branch:{change.newBranch}, Index:{change.newIndex}");
        }

        NPCDialogueDataManager.Instance.SaveNpcStates(NPCDialogueDataManager.Instance.npcDialogueStates); // JSON 저장
    }

    //==================================================퀘스트 조건 체크. 근데 복합 퀘스트는 미구현=======================================================================
    public void UpdateQuestProgressScore(SceneType scene, GameResult result = GameResult.Failed, int scoreAmount = 0) // 특정 씬에서 퀘스트 진행 업데이트
    {
        foreach (var quest in activeQuests)
        {
            if (quest.scene == scene && quest.status == QuestStatus.InProgress)
            {
                if(!quest.isScoreCompleted)
                {
                    quest.currentScore += scoreAmount;
                    if (quest.currentScore >= quest.requiredScore) 
                        quest.isScoreCompleted = true;
                }

                if (!quest.isFailedCompleted && result == GameResult.Failed)
                {
                    quest.currentFails += 1;
                    if (quest.currentFails >= quest.requiredFails)
                        quest.isFailedCompleted = true;
                }

                if (!quest.isSuccessesCompleted && result == GameResult.Succeeded)
                {
                    quest.currentSuccesses += 1;
                    if(quest.currentSuccesses >= quest.requiredSuccesses)
                        quest.isSuccessesCompleted = true;
                }

                if(!quest.isPerfectsCompleted && result == GameResult.Perfect)
                {
                    quest.currentPerfects += 1;
                    if(quest.currentPerfects >= quest.requiredPerfects)
                        quest.isPerfectsCompleted = true;
                }

                if (quest.isScoreCompleted && quest.isFailedCompleted && quest.isSuccessesCompleted && quest.isPerfectsCompleted)
                {
                    quest.status = QuestStatus.CanComplete;
                    Debug.Log($"퀘스트 완료 가능: {quest.questName}");
                }
            }
        }
    }

    public void UpdateQuestProgressNPC(ENPC targetNpc, int branch, int index) // 특정 인물의 특정 대화를 들을 때 퀘스트 진행 업데이트
    {
        foreach (var quest in activeQuests)
        {
            if (quest.targetNpc == targetNpc && quest.branch == branch && quest.index == index && quest.status == QuestStatus.InProgress)
            {
                quest.currentScore++;
                if (quest.currentScore >= quest.requiredScore)
                {
                    quest.status = QuestStatus.CanComplete;
                    Debug.Log($"퀘스트 완료 가능: {quest.questName}");
                }
            }
        }
    }

    private void LoadInitialQuests()
    {
        int i = 0;
        while (true)
        {
            QuestData quest = Resources.Load<QuestData>("QuestData/" + "Quest" + i);
            if (quest == null)
            {
                Debug.Log("초기 퀘스트 로드 완료");
                break;
            }
            if (quest.status == QuestStatus.InProgress || quest.status == QuestStatus.CanComplete)
            {
                activeQuests.Add(quest);
            }
            i++;
        }
        Debug.Log("초기 퀘스트 로드 완료");
    }
}
