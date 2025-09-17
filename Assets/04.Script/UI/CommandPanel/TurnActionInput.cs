using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnActionInput : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z)) OnRunPressed();            //Run
        if (Input.GetKeyDown(KeyCode.X)) OnMeleeAttackPressed();    //Melee Attack
        if (Input.GetKeyDown(KeyCode.C)) OnHidePressed();           //Hiding
        if (Input.GetKeyDown(KeyCode.V)) OnRangedAttackPressed();   //Ranged Attack
        if (Input.GetKeyDown(KeyCode.R)) OnSpecialActionPressed();  //Special Action
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
