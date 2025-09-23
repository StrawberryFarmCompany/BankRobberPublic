using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class AutoPipBar : MonoBehaviour
{
    [SerializeField] GameObject expanded;
    [SerializeField] GameObject compact;
    [SerializeField] TextMeshProUGUI movementText;
    [SerializeField] TextMeshProUGUI healthText;
    [SerializeField] Image portraitImage; // 캐릭터 초상화 이미지

    [SerializeField] private Image[] pipMovementImages; // 파이프 이미지 배열
    [SerializeField] private Image[] pipActionPointImages; // 파이프 이미지 배열




    void Update()  //=====================================================================================요거  바꿔야함 호출식으로
    {
        RefreshMovement();
        RefreshActionPoint();
        UpdatePortrait();
        RefreshHealth();

    }

    void RefreshMovement()
    {
        Color fillMovementColor = new Color(0, 54, 255, 255);

        if (NodePlayerManager.GetInstance.GetCurrentPlayer().playerCondition.playerStats.movement > 5)
        {
            expanded.SetActive(false);
            compact.SetActive(true);
            movementText.text = $"x {NodePlayerManager.GetInstance.GetCurrentPlayer().playerCondition.playerStats.movement}";
        }
        else
        {
            expanded.SetActive(true);
            compact.SetActive(false);
            for (int i = 0; i < pipMovementImages.Length; i++)
            {
                if (i < NodePlayerManager.GetInstance.GetCurrentPlayer().playerCondition.playerStats.movement)
                {
                    pipMovementImages[i].color = fillMovementColor;
                }
                else
                {
                    pipMovementImages[i].color = Color.black;
                }
            }
        }
    }

    void RefreshActionPoint()
    {
        Color fillActionPointColor = new Color(255, 196, 0, 255);
        Color emptyActionPointColor = new Color(88, 88, 88, 255);
        for (int i = 0; i < pipActionPointImages.Length; i++)
        {
            if (i < NodePlayerManager.GetInstance.GetCurrentPlayer().playerCondition.playerStats.curActionPoint)
            {
                pipActionPointImages[i].color = fillActionPointColor;
            }
            else
            {
                pipActionPointImages[i].color = emptyActionPointColor;
            }
        }
    }

    void UpdatePortrait()
    {
         portraitImage.sprite =  NodePlayerManager.GetInstance.GetCurrentPlayer().playerCondition.portrait;
    }

    void RefreshHealth()
    {
        healthText.text = $"{NodePlayerManager.GetInstance.GetCurrentPlayer().playerCondition.playerStats.CurHp} / {NodePlayerManager.GetInstance.GetCurrentPlayer().playerCondition.playerStats.maxHp}";   
    }






}
