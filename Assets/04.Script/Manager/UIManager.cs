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

    private BuffPannel buffPannel;
    public BuffPannel BuffPannel
    {
        get
        {
            if (buffPannel == null)
            {
                GameObject obj = GameObject.Instantiate((GameObject)ResourceManager.GetInstance.GetPreLoad["BuffPannel"], interactionCanvasInstance.transform);
                buffPannel = obj.GetComponent<BuffPannel>();
                buffPannel.Init();
            }
            return buffPannel;
        }
    }
    [Header("인터랙션 캔버스")]
    [SerializeField] private GameObject interactionCanvasPrefab;
    [SerializeField] private Transform canvasRoot;
    private GameObject interactionCanvasInstance;
    private BuffPannel buffPannel;
    public BuffPannel BuffPannel
    {
        get 
        {
            if (buffPannel == null)
            {
                GameObject obj = GameObject.Instantiate((GameObject)ResourceManager.GetInstance.GetPreLoad["BuffPannel"], interactionCanvasInstance.transform);
                buffPannel = obj.GetComponent<BuffPannel>();
                buffPannel.Init();
            }
            return buffPannel; 
        }
    }
    public bool SelectionLocked { get; private set; }

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
}
