using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
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
        activeQuests = QuestManager.GetInstance.activeQuests;

        QuestWindow.SetActive(false);
    }

    public void CloseUI()
    {
        QuestWindow.SetActive(false);
        CharacterManager.Instance.player.controller.crosshair.SetActive(true);
        CharacterManager.Instance.player.controller.LockCursor();
        CharacterManager.Instance.player.controller.EnableActions();
        if (CharacterManager.Instance.player.curUIPanel == QuestWindow)
        {
            CharacterManager.Instance.player.curUIPanel = null;
        }
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void OnCloseUI(InputAction.CallbackContext context)
    {
        if (QuestWindow.activeInHierarchy && context.started)
        {
            CloseUI();
        }
    }

    void Toggle()
    {
        if (IsOpen())
        {
            QuestWindow.SetActive(false);
            CharacterManager.Instance.player.controller.crosshair.SetActive(true);
            CharacterManager.Instance.player.controller.EnableActions();
            CharacterManager.Instance.player.controller.LockCursor();
            CharacterManager.Instance.player.curUIPanel = null;
            UpdateQuestList();
        }
        else
        {
            QuestWindow.SetActive(true);
            CharacterManager.Instance.player.controller.crosshair.SetActive(false);
            CharacterManager.Instance.player.controller.DisableActions();
            CharacterManager.Instance.player.controller.UnlockCursor();
            CharacterManager.Instance.player.curUIPanel = QuestWindow;
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

        if (quest.sceneName == null)
        {
            questProgressText.text = $"{quest.targetDialogue}";
            return;
        }
        if (quest.targetDialogue == null || quest.targetDialogue == "")
        {
            questProgressText.text = $"{quest.sceneName} : {quest.currentScore} / {quest.requiredScore}";
            return;
        }
        else
        {
            questProgressText.text = $"{quest.sceneName.ToString()} : {quest.currentScore} / {quest.requiredScore}\n{quest.targetDialogue}";
            return;
        }

    }


}
