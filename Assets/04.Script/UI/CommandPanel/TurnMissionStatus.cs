using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class TurnMissionStatus : MonoBehaviour
{
    [SerializeField] TMP_Text titleLabel;
    [SerializeField] TMP_Text stealthRoundLabel;

    [SerializeField] Button endTurnButton;

    [SerializeField] bool lockButtonOnEnemy = false;

    [SerializeField] string allyText = "내 턴";
    [SerializeField] string enemyText = "상대 턴";
    [SerializeField] string neutralText = "중립 턴";
    [SerializeField] string stealthPrefix = "잠입 라운드 ";

    int stealthRound = 1;       //라운드 카운트
    bool sawFirstAlly = false;  //시작 후 첫 아군 턴 증가 방지
    TurnBehaviour onAllyStart, onEnemyStart, onNeutralStart;

    NoneBattleTurnStateMachine SM => GameManager.GetInstance.NoneBattleTurn;

    void Awake()
    {
        onAllyStart = OnAllyStart;
        onEnemyStart = OnEnemyStart;
        onNeutralStart = OnNeutralStart;

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

    void OnAllyStart()
    {
        if (titleLabel) titleLabel.SetText(allyText);

        //첫 아군 턴 증가 방지
        if (sawFirstAlly) stealthRound++;
        else sawFirstAlly = true;

        UpdateStealthLabel();
        SetButtonInteractable(true);
    }

    void OnEnemyStart()
    {
        if (titleLabel) titleLabel.SetText(enemyText);
        SetButtonInteractable(false);
    }

    void OnNeutralStart()
    {
        if (titleLabel) titleLabel.SetText(neutralText);
        SetButtonInteractable(false);
    }

    void UpdateStealthLabel()
    {
        if (stealthRoundLabel)
            stealthRoundLabel.SetText($"{stealthPrefix}{stealthRound/2+1}");
    }

    void SetButtonInteractable(bool myTurn)
    {
        if (!endTurnButton) return;
        endTurnButton.interactable = myTurn;
    }
}
