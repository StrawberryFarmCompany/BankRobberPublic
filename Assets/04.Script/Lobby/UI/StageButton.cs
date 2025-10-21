using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class StageButton : MonoBehaviour
{
    public SceneType sceneType;
    public string firstText;
    public string secondText;
    public string thirdText;

    [HideInInspector]
    public StageUI stageUI;
    [HideInInspector]
    public TextMeshProUGUI firstTextObject;
    [HideInInspector]
    public TextMeshProUGUI secondTextObject;
    [HideInInspector]
    public TextMeshProUGUI thirdTextObject;

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
        thirdTextObject.text = thirdText;
        stageUI.startButton.interactable = LoadSceneManager.GetInstance.CheckEntryCondition(sceneType);
    }
}
