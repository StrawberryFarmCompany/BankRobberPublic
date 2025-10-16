using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TurnActionInput : MonoBehaviour
{
    // 내 턴일 때만 행동 가능
    bool canAct = false;

    [SerializeField] CanvasGroup actionPanelGroup;

    [SerializeField] Button turnButton;

    TurnBehaviour onAllyStart, onEnemyStart, onNeutralStart;
    
    NodePlayerController playerController => NodePlayerManager.GetInstance.GetCurrentPlayer();

    NoneBattleTurnStateMachine SM => GameManager.GetInstance.NoneBattleTurn;

    void Awake()
    {
        onAllyStart = () => SetCanAct(true);   // 내 턴 시작 → 입력 허용
        onEnemyStart = () => SetCanAct(false);  // 적 턴 시작 → 입력 차단
        onNeutralStart = () => SetCanAct(false);  // 중립 턴 시작 → 입력 차단

        SM.AddStartPointer(TurnTypes.ally, onAllyStart);
        SM.AddEndPointer(TurnTypes.ally, onEnemyStart);
        SM.AddEndPointer(TurnTypes.enemy, onNeutralStart);

    }

    void OnDestroy()
    {
        SM.RemoveStartPointer(TurnTypes.ally, onAllyStart);
        SM.RemoveEndPointer(TurnTypes.ally, onEnemyStart);
        SM.RemoveEndPointer(TurnTypes.enemy, onNeutralStart);
        onAllyStart = null;
        onEnemyStart = null;
        onNeutralStart = null;
    }

    void SetCanAct(bool allow)
    {
        canAct = allow;
        //패널 자체 클릭 막기
        if (actionPanelGroup)
        {
            turnButton.interactable = allow;
            actionPanelGroup.interactable = allow;
            actionPanelGroup.blocksRaycasts = true;
        }
    }

    public void OnRunPressed()
    {
        if(playerController.isMoveMode && playerController.IsMyTurn())
        {
            UIManager.GetInstance.ShowActionPanel(false);
            playerController.StartMode(ref playerController.isRunMode);
        }
    }
    public void OnThrowPressed()
    {
        if ((playerController.IsMyTurn() && playerController.isMoveMode))
        {
            UIManager.GetInstance.ShowActionPanel(false);
            playerController.StartMode(ref playerController.isHideMode);
            playerController.TurnOnHighlighter(6);
        }
    }

    public void OnAimingPressed()
    {
        if (playerController.isMoveMode && playerController.IsMyTurn())
        {
            UIManager.GetInstance.ShowActionPanel(false);
            playerController.StartMode(ref playerController.isAimingMode);
        }
    }

    

    public void OnHidePressed()
    {
        if (playerController.IsMyTurn() && !playerController.isHide && playerController.isMoveMode)
        {
            UIManager.GetInstance.ShowActionPanel(false);
            playerController.StartMode(ref playerController.isHideMode);
        }
        
        if (playerController.IsMyTurn() && playerController.isHide && playerController.isMoveMode)
        {
            UIManager.GetInstance.ShowActionPanel(false);
            playerController.StartMode(ref playerController.isSneakAttackMode);
        }
    }

    public void OnRangedAttackPressed()
    {
        if (playerController.IsMyTurn() && playerController.isMoveMode)
        {
            UIManager.GetInstance.ShowActionPanel(false);
            playerController.StartMode(ref playerController.isRangeAttackMode);
            playerController.TurnOnHighlighter(0);
        }
    }

    public void OnPerkActionPressed()
    {
        if (playerController.IsMyTurn() && playerController.isMoveMode)
        {
            UIManager.GetInstance.ShowActionPanel(false);
            playerController.StartMode(ref playerController.isPerkActionMode);
        }
    }

    public void OnCancelPressed()
    {
        if (playerController.IsMyTurn() && !playerController.isMoveMode)
        {
            UIManager.GetInstance.ShowActionPanel(true);
            playerController.StartMode(ref playerController.isMoveMode);
            playerController.TurnOnHighlighter(playerController.playerStats.movement);
        }
    }

    public void OnEndPressed()
    {
        //if (playerController.IsMyTurn() && playerController.isMoveMode)
        {
            UIManager.GetInstance.ShowActionPanel(true);
            NodePlayerManager.GetInstance.NotifyPlayerEndTurn(playerController);
            //나중에 플레이어 턴 끝나면 패널 어떻게 처리할건지 논의
        }
    }
}
