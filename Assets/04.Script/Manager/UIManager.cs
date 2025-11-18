using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using NodeDefines;
using System.Resources;
using System.Net.Http.Headers;
using System.Collections;
using DG.Tweening;

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
    public DocumentUI documentUI;
    public PasswordUI passwordUI;
    public GuideUI guideUI;
    public ActionTooltip actionTooltip;
    [SerializeField] public ActionTooltipTrigger specialSkillTooltip;
    public Transform CanvasRoot { get { return canvasRoot; } }

    private Defines.WarningMessage warningMessege;

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

    private IEnumerator Start()
    {
        if(ResourceManager.GetInstance.GetPreLoad.Count <= 0 && ResourceManager.GetInstance.GetPreLoad == null)
            yield return new WaitUntil(() => ResourceManager.GetInstance.GetPreLoad.Count > 0 && ResourceManager.GetInstance.GetPreLoad != null);
        warningMessege = new Defines.WarningMessage(actionPanel.transform);
        RefreshSpecialSkillTooltip();
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

    /// <summary>
    /// 문서 인터랙션을 했을 때 나타나는 UI
    /// </summary>
    public void SetDocumentUI(int index, DocumentType type, bool isFirstTwoDigit)
    {
        documentUI.index = index;
        documentUI.type = type;
        if(type == DocumentType.password)
            documentUI.isFirstTwoDigit = isFirstTwoDigit;
        documentUI.gameObject.SetActive(true);
    }

    /// <summary>
    /// 패스워드 문을 인터랙션 했을 때 나타나는 UI
    /// </summary>
    /// <param name="index"></param>
    public void SetPasswordUI(int index, Transform doorPos)
    {
        passwordUI.index = index;
        passwordUI.doorPos = doorPos;
        passwordUI.Clear();
        passwordUI.gameObject.SetActive(true);
    }

    public void SetErrorMessege(string str)
    {
        warningMessege.SetErrorMessege(str);
    }

    private void FindSpecialSkillTooltip()
    {
        if (specialSkillTooltip != null) return;

        var btn = GameObject.Find("SpecialActionButton");
        if (btn != null)
            specialSkillTooltip = btn.GetComponent<ActionTooltipTrigger>();
    }

    public void RefreshSpecialSkillTooltip()
    {
        FindSpecialSkillTooltip();

        var player = NodePlayerManager.GetInstance?.GetCurrentPlayer();
        if (player == null || specialSkillTooltip == null)
        {
            return;
        }

        Skill so = SkillSOMapper.Get(player.playerStats.playerSkill);
        Debug.Log($"Skill: {player.playerStats.playerSkill}, SO: {so}, effect: {(so != null ? so.effect : "NULL")}");

        if (so == null) return;

        specialSkillTooltip.description = so.effect;
    }

    private void OnEnable()
    {
        EquippedSkills.OnChanged += RefreshSpecialSkillTooltip;
    }

    private void OnDisable()
    {
        EquippedSkills.OnChanged -= RefreshSpecialSkillTooltip;
    }
}
