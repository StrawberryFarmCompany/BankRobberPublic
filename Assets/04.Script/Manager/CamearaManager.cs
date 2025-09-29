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

    public bool isCompleteTransition = true;

    //private bool isRotationMode = false;
    //private bool canFollowMove = false;

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
        if (!context.started || !isCompleteTransition) return;

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

    //private IEnumerator FreeViewTransitionCoroutine(GameObject player)
    //{
    //    IsReadyTransition = false;
    //    float duration = brain.m_DefaultBlend.m_Time;
    //    float elapsed = 0f;

    //    Vector3 startPos = fcam.transform.position;
    //    Vector3 targetPos = player.transform.position + offset;
    //    Quaternion startRot = fcam.transform.rotation;

    //    isCompleteTransition = false;

    //    while (elapsed < duration)
    //    {
    //        elapsed += Time.deltaTime;
    //        float t = Mathf.Clamp01(elapsed / duration);

    //        // 위치 보간
    //        fcam.transform.position = Vector3.Lerp(startPos, targetPos, t);

    //        // 회전 보간 (x축은 고정, y축만 보간)
    //        Vector3 lookDir = player.transform.position - fcam.transform.position;
    //        if (lookDir.sqrMagnitude > 0.001f)
    //        {
    //            Quaternion targetRot = Quaternion.LookRotation(lookDir, Vector3.up);

    //            // x축 고정: targetRot의 EulerAngles에서 x를 현재 rotation.x로 유지
    //            Vector3 targetEuler = targetRot.eulerAngles;
    //            targetEuler.x = fcam.transform.rotation.eulerAngles.x;

    //            Quaternion fixedTargetRot = Quaternion.Euler(targetEuler);

    //            fcam.transform.rotation = Quaternion.Slerp(startRot, fixedTargetRot, t);
    //        }

    //        yield return null;
    //    }

    //    // 보정 (마지막에 정확히 위치 고정)
    //    fcam.transform.position = targetPos;
    //    isCompleteTransition = true;
    //}

}
