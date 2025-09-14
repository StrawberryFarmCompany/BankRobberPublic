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
    public CinemachineVirtualCamera player1Cam;
    public CinemachineVirtualCamera player2Cam;
    public CinemachineVirtualCamera player3Cam;

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

    public void OnRotationMode(InputAction.CallbackContext context)
    {
        if (context.performed && !isFreeView)
            isRotationMode = true;

        if (context.canceled)
            isRotationMode = false;
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        if (isFreeView || !isRotationMode) return;

        Vector2 lookDelta = context.ReadValue<Vector2>();
        float yaw = lookDelta.x * rotateSpeed * (xflip ? 1 : -1);
        float pitch = -lookDelta.y * rotateSpeed * (yflip ? 1 : -1);

        // 현재 회전각
        Vector3 currentEuler = transform.rotation.eulerAngles;

        // 오일러 각도는 0~360이니까 0~180과 180~360 사이 구분 필요
        float currentPitch = currentEuler.x;
        if (currentPitch > 180f) currentPitch -= 360f;

        // pitch 제한 적용
        float newPitch = Mathf.Clamp(currentPitch + pitch, 0f, 89.9f);

        // 회전 적용 (Y는 누적, X는 제한)
        float newYaw = currentEuler.y + yaw;
        transform.rotation = Quaternion.Euler(newPitch, newYaw, 0f);
    }

    private IEnumerator transPlayerViewCoroutine(CinemachineVirtualCamera transTarget)
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
