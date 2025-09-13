using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraManager : MonoSingleTon<CameraManager>
{
    [SerializeField] private CinemachineVirtualCamera freeViewCam;
    public CinemachineVirtualCamera player1Cam;
    public CinemachineVirtualCamera player2Cam;
    public CinemachineVirtualCamera player3Cam;



    public int disablePriority = 5;
    public int activeProiority = 20;

    private void Awake()
    {
        freeViewCam.Priority = activeProiority;
        player1Cam.Priority = disablePriority;
        player2Cam.Priority = disablePriority;
        player3Cam.Priority = disablePriority;
    }

    public void OnFreeView(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            freeViewCam.Priority = activeProiority;
            player1Cam.Priority = disablePriority;
            player2Cam.Priority = disablePriority;
            player3Cam.Priority = disablePriority;
        }
    }

    public void OnPlayer1View(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            freeViewCam.Priority = disablePriority;
            player1Cam.Priority = activeProiority;
            player2Cam.Priority = disablePriority;
            player3Cam.Priority = disablePriority;
        }
    }

    public void OnPlayer2View(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            freeViewCam.Priority = disablePriority;
            player1Cam.Priority = disablePriority;
            player2Cam.Priority = activeProiority;
            player3Cam.Priority = disablePriority;
        }
    }

    public void OnPlayer3View(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            freeViewCam.Priority = disablePriority;
            player1Cam.Priority = disablePriority;
            player2Cam.Priority = disablePriority;
            player3Cam.Priority = activeProiority;
        }
    }
}
