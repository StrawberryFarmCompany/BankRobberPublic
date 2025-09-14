using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerChangeManager : MonoBehaviour
{
    public GameObject player1;
    public GameObject player2;
    public GameObject player3;

    private Camera mainCam;

    private void Start()
    {
        mainCam = Camera.main;

        // 시작은 player1 시점
        OnStart();
        SetCameraToPlayer(player1);
    }

    // 카메라 시점 변환
    private void SetCameraToPlayer(GameObject player)
    {
        Transform camHolder = player.transform.Find("CameraContainer");
        if (camHolder != null && mainCam != null)
        {
            mainCam.transform.SetParent(camHolder);
            mainCam.transform.localPosition = Vector3.zero;
            mainCam.transform.localRotation = Quaternion.identity;
        }
    }

    // 플레이어1로 변경
    public void OnPlayer1(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            player1.GetComponent<PlayerInput>().enabled = true;
            player2.GetComponent<PlayerInput>().enabled = false;
            player3.GetComponent<PlayerInput>().enabled = false;

            SetCameraToPlayer(player1);
        }
    }

    //플레이어2로 변경
    public void OnPlayer2(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            player1.GetComponent<PlayerInput>().enabled = false;
            player2.GetComponent<PlayerInput>().enabled = true;
            player3.GetComponent<PlayerInput>().enabled = false;

            SetCameraToPlayer(player2);
        }
    }

    //플레이어3로 변경
    public void OnPlayer3(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            player1.GetComponent<PlayerInput>().enabled = false;
            player2.GetComponent<PlayerInput>().enabled = false;
            player3.GetComponent<PlayerInput>().enabled = true;

            SetCameraToPlayer(player3);
        }
    }

    private void OnStart()
    {
        player2.GetComponent<PlayerInput>().enabled = false;
        player3.GetComponent<PlayerInput>().enabled = false;
    }
}
