using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class StageButton : MonoBehaviour
{
    public SceneType sceneType;
    public string firstText;
    public string secondText;
    public string conditionText;

    [HideInInspector]
    public StageUI stageUI;
    [HideInInspector]
    public TextMeshProUGUI firstTextObject;
    [HideInInspector]
    public TextMeshProUGUI secondTextObject;
    [HideInInspector]
    public TextMeshProUGUI conditionTextObject;
    [HideInInspector]
    public TextMeshProUGUI checkConditionTextObject;

    [HideInInspector]
    public Button button;

    public void Init(StageUI stage)
    {
        stageUI = stage;
        button = GetComponent<Button>();
        button.onClick.AddListener(SetStage);
    }

    public void SetStage()
    {
        stageUI.curSelectScene = sceneType;
        firstTextObject.text = firstText;
        secondTextObject.text = secondText;
        conditionTextObject.text = conditionText;

        bool condition = LoadSceneManager.GetInstance.CheckEntryCondition(sceneType);

        Color color = new Color();

        if (condition)
        {
            color = Color.blue;
        }
        else
        {
            color = Color.yellow;
        }

        checkConditionTextObject.text = $"{ScoreManager.GetInstance.GetScore(sceneType - 1)} / {LoadSceneManager.GetInstance.GetRequireScore(sceneType)}";
        if (ScoreManager.GetInstance.IsStageCleared(sceneType) || LoadSceneManager.GetInstance.GetRequireScore(sceneType)==0)
        {
            checkConditionTextObject.text = "완료";
        }
        checkConditionTextObject.color = color;

        stageUI.startButton.interactable = condition;
    }
}
