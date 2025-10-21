using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StageUI : MonoBehaviour
{
    [SerializeField] private GameObject[] StageBtns;
    public Button startButton;
    [SerializeField] private TextMeshProUGUI fisrtDescriptionText;
    [SerializeField] private TextMeshProUGUI secondDescriptionText;
    [SerializeField] private TextMeshProUGUI conditionDescriptionText;
    [SerializeField] private TextMeshProUGUI checkConditionDescriptionText;

    public SceneType curSelectScene; // 최근 선택된 씬

    private void Start()
    {
        for (int i = 0; i < StageBtns.Length; i++)
        {
            StageButton stageButton = StageBtns[i].GetComponent<StageButton>();
            stageButton.firstTextObject = this.fisrtDescriptionText;
            stageButton.secondTextObject = this.secondDescriptionText;
            stageButton.conditionTextObject = this.conditionDescriptionText;
            stageButton.checkConditionTextObject = this.checkConditionDescriptionText;
            stageButton.Init(this);
        }
        startButton.onClick.AddListener(OnChangeScene);
    }
    
    public void OnChangeScene()
    {
        LoadSceneManager.GetInstance.StageSceneLoad(curSelectScene);
    }

}
