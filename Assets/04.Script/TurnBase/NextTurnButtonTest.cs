using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class NextTurnButtonTest : MonoBehaviour
{
    // Start is called before the first frame update
    public void OnButtonClick()
    {
        TurnTypes type = GameManager.GetInstance.Turn.GetCurrState();
        int num = (int)type;
        num = (num+1)%Enum.GetValues(typeof(TurnTypes)).Length;
        GameManager.GetInstance.Turn.ChangeState(GameManager.GetInstance.Turn.FindState((TurnTypes)num));
    }
}
