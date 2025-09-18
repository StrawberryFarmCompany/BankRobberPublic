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

    TurnBehaviour onAllyStart, onEnemyStart, onNeutralStart;

    NoneBattleTurnStateMachine SM => GameManager.GetInstance.NoneBattleTurn;

    void Awake()
    {
        onAllyStart = () => SetCanAct(true);   // 내 턴 시작 → 입력 허용
        onEnemyStart = () => SetCanAct(false);  // 적 턴 시작 → 입력 차단
        onNeutralStart = () => SetCanAct(false);  // 중립 턴 시작 → 입력 차단

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

    void Update()
    {
        if (!canAct) return;

        if (Input.GetKeyDown(KeyCode.Z)) OnRunPressed();            //Run
        if (Input.GetKeyDown(KeyCode.X)) OnMeleeAttackPressed();    //Melee Attack
        if (Input.GetKeyDown(KeyCode.C)) OnHidePressed();           //Hiding
        if (Input.GetKeyDown(KeyCode.V)) OnRangedAttackPressed();   //Ranged Attack
        if (Input.GetKeyDown(KeyCode.R)) OnSpecialActionPressed();  //Special Action
    }

    void SetCanAct(bool allow)
    {
        canAct = allow;

        //패널 자체 클릭 막기
        if (actionPanelGroup)
        {
            actionPanelGroup.interactable = allow;
            actionPanelGroup.blocksRaycasts = true;
        }
    }

    public void OnRunPressed()
    {
        Debug.Log("[Action] Run");
    }

    public void OnMeleeAttackPressed()
    {
        Debug.Log("[Action] Melee Attack");
    }

    public void OnHidePressed()
    {
        Debug.Log("[Action] Hide");
    }

    public void OnRangedAttackPressed()
    {
        Debug.Log("[Action] Ranged Attack");
    }

    public void OnSpecialActionPressed()
    {
        Debug.Log("[Action] Special Action");
    }
}
