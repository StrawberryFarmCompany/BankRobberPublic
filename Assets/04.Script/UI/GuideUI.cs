using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;

public class GuideUI : MonoBehaviour
{

    [SerializeField] GuideUIData guideData;         //나중에 LoadSceneManager의 최근 씬 확인으로 데이터 받아오는 방법도 고려
    [SerializeField] TextMeshProUGUI goalText;
    private int curGuideIndex;

    private void Awake()
    {
        UIManager.GetInstance.guideUI = this;
        Init();
    }

    public void Init()
    {
        if(guideData == null)
        {
            //LoadSceneManager의 최근 씬 확인으로 데이터 받아오는 방법
        }

        goalText.text = guideData.Elements[0].description;
        curGuideIndex = 0;
    }

    public void UpdateGuide()
    {
        //if()
        if (guideData.Elements[curGuideIndex].IsMissionComplete())
        {
            curGuideIndex++;

        }
    }
}
