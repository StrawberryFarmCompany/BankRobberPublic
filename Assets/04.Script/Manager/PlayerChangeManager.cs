using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerChangeManager : MonoBehaviour
{
    public GameObject player1;
    public GameObject player2;
    public GameObject player3;

    private void Start()
    {
        OnStart();
    }

    public void OnPlayer1(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            player1.GetComponent<PlayerInput>().enabled = true;
            player2.GetComponent<PlayerInput>().enabled = false;
            player3.GetComponent<PlayerInput>().enabled = false;
        }
    }

    public void OnPlayer2(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            player1.GetComponent<PlayerInput>().enabled = false;
            player2.GetComponent<PlayerInput>().enabled = true;
            player3.GetComponent<PlayerInput>().enabled = false;
        }
    }

    public void OnPlayer3(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            player1.GetComponent<PlayerInput>().enabled = false;
            player2.GetComponent<PlayerInput>().enabled = false;
            player3.GetComponent<PlayerInput>().enabled = true;
        }
    }

    public void OnStart()
    {
        player2.GetComponent<PlayerInput>().enabled = false;
        player3.GetComponent<PlayerInput>().enabled = false;
    }
}
