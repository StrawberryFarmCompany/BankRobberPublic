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
    [SerializeField] private CinemachineVirtualCamera freeViewCam;

    [Header("플레이어 캐릭터")]
    public GameObject player1;
    public GameObject player2;
    public GameObject player3;

    [Header("플레이어 위치로 이동 시 조정값")]
    public Vector3 offset = new Vector3(0, 6, -5);

    public bool isCompleteTransition = true;

    private bool isRotationMode = false;
    private bool canFollowMove = false;
    private bool IsReadyTransition;
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
        if (context.started && isCompleteTransition && IsReadyTransition)
        {
            StartCoroutine(FreeViewTransitionCoroutine(NodePlayerManager.GetInstance.GetCurrentPlayer().gameObject));
        }

        if (context.started && isCompleteTransition)
        {
            IsReadyTransition = true;
            Invoke("IsReadyTransitionOff", 1f);
        }
    }

    public void IsReadyTransitionOff()
    {
        IsReadyTransition = false;
    }


    private IEnumerator FreeViewTransitionCoroutine(GameObject player)
    {
        IsReadyTransition = false;
        float duration = brain.m_DefaultBlend.m_Time;
        float elapsed = 0f;

        Vector3 startPos = freeViewCam.transform.position;
        Vector3 targetPos = player.transform.position + offset;
        Quaternion startRot = freeViewCam.transform.rotation;

        isCompleteTransition = false;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            // 위치 보간
            freeViewCam.transform.position = Vector3.Lerp(startPos, targetPos, t);

            // 회전 보간 (x축은 고정, y축만 보간)
            Vector3 lookDir = player.transform.position - freeViewCam.transform.position;
            if (lookDir.sqrMagnitude > 0.001f)
            {
                Quaternion targetRot = Quaternion.LookRotation(lookDir, Vector3.up);

                // x축 고정: targetRot의 EulerAngles에서 x를 현재 rotation.x로 유지
                Vector3 targetEuler = targetRot.eulerAngles;
                targetEuler.x = freeViewCam.transform.rotation.eulerAngles.x;

                Quaternion fixedTargetRot = Quaternion.Euler(targetEuler);

                freeViewCam.transform.rotation = Quaternion.Slerp(startRot, fixedTargetRot, t);
            }

            yield return null;
        }

        // 보정 (마지막에 정확히 위치 고정)
        freeViewCam.transform.position = targetPos;
        isCompleteTransition = true;
    }

}
