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

    [SerializeField] string allyText = "My Turn";
    [SerializeField] string enemyText = "Enemy Turn";
    [SerializeField] string neutralText = "Neutral";
    [SerializeField] string stealthPrefix = "Stealth Round ";

    int stealthRound = 1;       //라운드 카운트
    bool sawFirstAlly = false;  //시작 후 첫 아군 턴 증가 방지
    TurnBehaviour onAllyStart, onEnemyStart, onNeutralStart;

    NoneBattleTurnStateMachine SM => GameManager.GetInstance.NoneBattleTurn;

    void Awake()
    {
        onAllyStart = OnAllyStart;
        onEnemyStart = OnEnemyStart;
        onNeutralStart = OnNeutralStart;

        SM.AddStartPointer(TurnTypes.allay, onAllyStart);
        SM.AddStartPointer(TurnTypes.enemy, onEnemyStart);
        SM.AddStartPointer(TurnTypes.neutral, onNeutralStart);
    }

    void OnDestroy()
    {
        SM.RemoveStartPointer(TurnTypes.allay, onAllyStart);
        SM.RemoveStartPointer(TurnTypes.enemy, onEnemyStart);
        SM.RemoveStartPointer(TurnTypes.neutral, onNeutralStart);
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
            stealthRoundLabel.SetText($"{stealthPrefix}{stealthRound}");
    }

    void SetButtonInteractable(bool myTurn)
    {
        if (!endTurnButton) return;
        endTurnButton.interactable = lockButtonOnEnemy ? myTurn : true;
    }
}
