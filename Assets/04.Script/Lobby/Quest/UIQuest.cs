using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIQuest : MonoBehaviour
{
    public PlayerController controller;

    private List<QuestData> activeQuests = new List<QuestData>();
    public GameObject QuestWindow;
    public GameObject questButton;
    public Transform questListParent;
    public TextMeshProUGUI questTitleText;
    public TextMeshProUGUI questDescriptionText;
    public TextMeshProUGUI questStateText;
    public TextMeshProUGUI questProgressText;


    void Start()
    {
        controller = CharacterManager.Instance.player.controller;
        controller.quest += Toggle;
        activeQuests = QuestManager.Instance.activeQuests;

        QuestWindow.SetActive(false);
    }

    void Toggle()
    {
        if (IsOpen())
        {
            QuestWindow.SetActive(false);
            UpdateQuestList();
        }
        else
        {
            QuestWindow.SetActive(true);
            UpdateQuestList();
        }
    }
    public bool IsOpen()
    {
        return QuestWindow.activeInHierarchy;
    }

    public void UpdateQuestList()
    {
        foreach (Transform child in questListParent)
        {
            Destroy(child.gameObject);
        }
        foreach (QuestData quest in activeQuests)
        {
            GameObject questItem = Instantiate(questButton, questListParent);
            questItem.GetComponentInChildren<TextMeshProUGUI>().text = quest.questName;
            Button button = questItem.GetComponent<Button>();
            button.onClick.AddListener(() => ShowQuestDetails(quest));
        }
    }

    public void ShowQuestDetails(QuestData quest)
    {
        questTitleText.text = quest.questName;
        questDescriptionText.text = quest.description;
        questStateText.text = quest.status.ToString();
        if (quest.status == QuestStatus.InProgress)
        {
            questStateText.color = Color.yellow;
        }
        else if (quest.status == QuestStatus.CanComplete)
        {
            questStateText.color = Color.green;
        }

        if (quest.targetID == null || quest.targetID == "")
        {
            questProgressText.text = $"{quest.targetDialogue}";
            return;
        }
        if (quest.targetDialogue == null || quest.targetDialogue == "")
        {
            questProgressText.text = $"{quest.targetID}óġ : {quest.currentAmount} / {quest.requiredAmount}";
            return;
        }
        else
        {
            questProgressText.text = $"{quest.targetID.ToString()} óġ: {quest.currentAmount} / {quest.requiredAmount}\n{quest.targetDialogue}";
            return;
        }

    }


}
