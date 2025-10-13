using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using static Cinemachine.CinemachineTargetGroup;

public class CameraManager : MonoBehaviour
{
    private static CameraManager instance;

    public static CameraManager GetInstance
    {
        get
        {
            return instance;
        }
    }
    private CinemachineBrain brain;
    [SerializeField] private GameObject cam;
    [SerializeField] private CinemachineFreeLook fcam;

    [Header("팔로우 타겟")]
    [SerializeField] private Transform followTarget; 

    [Header("플레이어 위치로 이동 시 조정값")]
    public Vector3 offset = new Vector3(0, 6, -5);


    private bool IsReadyTransition;
    private float lastTapTime = 0f;
    [SerializeField] private float doubleTapThreshold = 0.3f; // 두 번 누를 최대 간격

    public bool isFreeView;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }

        brain = cam.GetComponent<CinemachineBrain>();
    }

    public void OnPlayerView(InputAction.CallbackContext context)
    {
        if (!context.started) return;

        float currentTime = Time.time;

        if (currentTime - lastTapTime <= doubleTapThreshold && IsReadyTransition)
        {
            fcam.Follow = NodePlayerManager.GetInstance.GetCurrentPlayer().gameObject.transform;
            fcam.LookAt = NodePlayerManager.GetInstance.GetCurrentPlayer().gameObject.transform;
            isFreeView = false;
            IsReadyTransition = false; // 리셋
        }
        else
        {
            IsReadyTransition = true;
            lastTapTime = currentTime;
        }
    }

    public void SwitchToPlayerCamera(GameObject player)
    {
        if (player == null) return;
        //isCompleteTransition = false;
        isFreeView = false;
        fcam.Follow = player.transform;
        fcam.LookAt = player.transform;
    }

}
