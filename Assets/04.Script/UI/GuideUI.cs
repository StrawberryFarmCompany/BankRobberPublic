using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Xml.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class GuideUI : MonoBehaviour
{

    [SerializeField] GuideUIData guideData;         //나중에 LoadSceneManager의 최근 씬 확인으로 데이터 받아오는 방법도 고려
    [SerializeField] TextMeshProUGUI goalText;
    [SerializeField] Image panel;
    [SerializeField] TextMeshProUGUI bishopConditionText;
    [SerializeField] TextMeshProUGUI rookConditionText;
    [SerializeField] TextMeshProUGUI knightConditionText;
    bool isLastSet;
    private List<NodePlayerController> players = new List<NodePlayerController>();

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
        isLastSet = false;
        bishopConditionText.text = "";
        rookConditionText.text =  "";
        knightConditionText.text = "";
        bishopConditionText.gameObject.SetActive(false);
        rookConditionText.gameObject.SetActive(false);
        knightConditionText.gameObject.SetActive(false);
        Vector2 panelScale = new Vector2(500, 120);
        panel.rectTransform.sizeDelta = panelScale;
    }

    private void Update()
    {
        //테스트용 나중에 지우셈
        UpdateGuide();
    }

    public void UpdateGuide()
    {
        if (curGuideIndex != guideData.Elements.Count - 1)
        {
            if (guideData.Elements[curGuideIndex].IsMissionComplete())
            {
                curGuideIndex++;
                goalText.text = guideData.Elements[curGuideIndex].description;

                if (guideData.Elements[curGuideIndex].missionType == GuideMissionType.OpenDoorWithButton)
                {
                    if(GameManager.GetInstance.buttonDocuments[guideData.Elements[curGuideIndex].doorIndex].isSaw)
                        goalText.text = $"{guideData.Elements[curGuideIndex].description} : {GameManager.GetInstance.GetButtonDoorNumber(guideData.Elements[curGuideIndex].doorIndex)}";
                }
                if(guideData.Elements[curGuideIndex].missionType == GuideMissionType.OpenDoorWithPassword)
                {
                    bool isSaw = true;
                    foreach (Document document in GameManager.GetInstance.passwordDocuments[guideData.Elements[curGuideIndex].doorIndex])
                        isSaw = isSaw && document.isSaw;
                    if (isSaw)
                        goalText.text = $"{guideData.Elements[curGuideIndex].description} : {GameManager.GetInstance.passwordDoorPair[guideData.Elements[curGuideIndex].doorIndex]}";
                }
                
                // 미리 후순위 목표를 달성 했을 수도 있으니 한 번 더 체크
                UpdateGuide();
            }
        }
        else //마지막 돈 탈취 후 탈출하기
        {
            if (!isLastSet)
            {
                isLastSet = true;
                Vector2 panelScale = new Vector2(500, 300);
                panel.rectTransform.sizeDelta = panelScale;
                bishopConditionText.gameObject.SetActive(true);
                rookConditionText.gameObject.SetActive(true);
                knightConditionText.gameObject.SetActive(true);
                foreach(var player in NodePlayerManager.GetInstance.GetAllPlayers())
                    players.Add(player);
            }

            foreach(NodePlayerController player in players)
            {
                switch (player.playerStats.characterType)
                {
                    case CharacterType.Bishop:
                        bishopConditionText.text = $"비숍{CurCharacterCondition(player, NodePlayerManager.GetInstance.isEscapeBishop)}";
                        break;
                    case CharacterType.Rook:
                        rookConditionText.text = $"룩{CurCharacterCondition(player, NodePlayerManager.GetInstance.isEscapeRook)}";
                        break;
                    case CharacterType.Knight:
                        knightConditionText.text = $"나이트{CurCharacterCondition(player, NodePlayerManager.GetInstance.isEscapeKnight)}";
                        break;
                    default:
                        break;
                }
            }

        }
    }

    public string CurCharacterCondition(NodePlayerController player, EscapeCondition condition)
    {
        //탈출성공, 탈취성공, 빈가방, 체포됨
        if(condition == EscapeCondition.Escape || condition == EscapeCondition.SuccessHeist)
        {
            return $" : 탈출 성공";
        }
        else if(condition == EscapeCondition.Arrest)
        {
            return $" : 체포됨";
        }
        else if(condition == EscapeCondition.Heisting)
        {
            if(player.fullBackPack != null)
            {
                return $" : 탈취 성공";
            }
            else
            {
                return $" : 빈 가방";
            }
        }
        else
        {
            return $" : 알 수 없음";
        }

    }
}
