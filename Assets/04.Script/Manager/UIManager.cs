using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using NodeDefines;
using System.Resources;

public class UIManager : MonoBehaviour
{
    public static UIManager GetInstance;
    [SerializeField] private GameObject actionPanel;
    [SerializeField] private GameObject cancelPanel;

    public AutoPipBar pip;
    [Header("인터랙션 캔버스")]
    [SerializeField] private GameObject interactionCanvasPrefab;
    [SerializeField] private Transform canvasRoot;
    private GameObject interactionCanvasInstance;
    private BuffPannel buffPannel;

    public bool SelectionLocked { get; private set; }

    public LeftInteractionPanel leftInteractionPanel;

    [Header("게임 엔드 패널")]
    public GameEnd gameEndUI;
    //[HideInInspector]
    //public //개별 캐릭터 결과창에 들어갈 UI (상태 세팅)

    public BuffPannel BuffPannel
    {
        get 
        {
            if (buffPannel == null)
            {
                GameObject obj = GameObject.Instantiate((GameObject)ResourceManager.GetInstance.GetPreLoad["BuffPannel"], actionPanel.transform.parent.parent.parent.parent);
                buffPannel = obj.GetComponent<BuffPannel>();
                buffPannel.Init();
            }
            return buffPannel; 
        }
    }
    

    private void Awake()
    {
        GetInstance = this;
    }


    public void ShowActionPanel(bool show)
    {
        actionPanel.SetActive(show);
        cancelPanel.SetActive(!show);
    }

    public void SetSelectionLocked(bool locked) => SelectionLocked = locked;

    /// <summary>
    /// 게임 엔드 패널에 캐릭터 상태를 나타내는 프리팹을 생성하는 함수
    /// </summary>
    /// <param name="stats"></param>
    public void SetCharacterResultUI(List<NodePlayerController> players)
    {
        foreach (NodePlayerController player in players)
        {
            gameEndUI.CreateCharacterUI(player.playerStats);
        }
    }
}
