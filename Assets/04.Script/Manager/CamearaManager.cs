using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
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
    [SerializeField] private CinemachineVirtualCamera freeViewCam;
    public CinemachineFreeLook player1Cam;
    public CinemachineFreeLook player2Cam;
    public CinemachineFreeLook player3Cam;

    [Header("플레이어 추적 카메라")]
    public float rotateSpeed = 0.2f;
    [Header("X축 좌우 반전")]
    public bool xflip = true;
    [Header("Y축 좌우 반전")]
    public bool yflip = true;

    public int disablePriority = 5;
    public int activeProiority = 20;

    public float transitionDuration;

    private bool isCompleteTransition = true;
    public bool isFreeView = true;

    private bool isRotationMode = false;
    private bool canFollowMove = false;
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

    private void Start()
    {
        InitializePriority();
        freeViewCam.Priority = activeProiority;

        transitionDuration = brain.m_DefaultBlend.m_Time;
    }

    public void OnFreeView(InputAction.CallbackContext context)
    {
        if (context.started && isCompleteTransition)
        {
            InitializePriority();
            freeViewCam.Priority = activeProiority;
            isFreeView = true;
        }
    }

    public void OnPlayer1View(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            StartCoroutine(transPlayerViewCoroutine(player1Cam));
        }
    }

    public void OnPlayer2View(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            StartCoroutine(transPlayerViewCoroutine(player2Cam));
        }
    }

    public void OnPlayer3View(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            StartCoroutine(transPlayerViewCoroutine(player3Cam));
        }
    }

    private IEnumerator transPlayerViewCoroutine(CinemachineFreeLook transTarget)
    {
        isFreeView = false;
        InitializePriority();
        transTarget.Priority = activeProiority;
        isCompleteTransition = false;
        yield return new WaitForSeconds(transitionDuration);
        freeViewCam.transform.position = transTarget.transform.position;
        freeViewCam.transform.rotation = transTarget.transform.rotation;
        isCompleteTransition = true;
    }

    private void InitializePriority()
    {
        freeViewCam.Priority = disablePriority;
        player1Cam.Priority = disablePriority;
        player2Cam.Priority = disablePriority;
        player3Cam.Priority = disablePriority;
    }
}
