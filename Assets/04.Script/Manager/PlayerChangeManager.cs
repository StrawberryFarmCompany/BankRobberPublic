using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

public class PlayerChangeManager : MonoBehaviour
{
    public GameObject player1;
    public GameObject player2;
    public GameObject player3;

    private Camera mainCam;
    
    [Header("Cinemachine")]
    public CinemachineVirtualCamera transitCam;
    public float zoomOutFov = 70f;
    public float zoomInFov = 45f;
    public float zoomOutTime = 0.3f;
    public float moveTime = 0.5f;
    public float zoomInTime = 0.2f;
    private bool isSwitching;   //교체 중

    private void Start()
    {
        mainCam = Camera.main;

        // 시작은 player1 시점
        OnStart();
        SetCameraToPlayer(player1);

        var brain = mainCam.GetComponent<CinemachineBrain>();
        if (!brain) brain = mainCam.gameObject.AddComponent<CinemachineBrain>();
        brain.enabled = false;
    }

    // 카메라 시점 변환
    private void SetCameraToPlayer(GameObject player)
    {
        if (!player || !mainCam) return;

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
            if (transitCam != null && !isSwitching)
            {
                StartCoroutine(SwitchRoutine(player1));
                return;
            }

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
            if (transitCam != null && !isSwitching)
            {
                StartCoroutine(SwitchRoutine(player2));
                return;
            }

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
            if (transitCam != null && !isSwitching)
            {
                StartCoroutine(SwitchRoutine(player3));
                return;
            }

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

    //카메라 전환 애니메이션
    private IEnumerator SwitchRoutine(GameObject targetPlayer)
    {
        isSwitching = true;
        ToggleInputs(null);

        //메인카메라 분리 + Brain ON
        mainCam.transform.SetParent(null, true);
        var brain = mainCam.GetComponent<CinemachineBrain>();
        brain.enabled = true;

        //transitCam 현재 화면 포즈로 맞춤
        transitCam.transform.SetPositionAndRotation(mainCam.transform.position, mainCam.transform.rotation);
        transitCam.m_Lens.FieldOfView = mainCam.fieldOfView;
        transitCam.Priority = 100;
        yield return null; //블렌드 적용

        //줌아웃
        yield return AnimateFov(zoomOutTime, transitCam.m_Lens.FieldOfView, zoomOutFov);

        //CameraContainer로 이동
        Transform dest = targetPlayer.transform.Find("CameraContainer");
        Vector3 sp = transitCam.transform.position, dp = dest.position;
        Quaternion sr = transitCam.transform.rotation, dr = dest.rotation;
        for (float t = 0f; t < 1f; t += Time.deltaTime / Mathf.Max(0.0001f, moveTime))
        {
            float k = Mathf.SmoothStep(0f, 1f, t);
            transitCam.transform.position = Vector3.LerpUnclamped(sp, dp, k);
            transitCam.transform.rotation = Quaternion.SlerpUnclamped(sr, dr, k);
            yield return null;
        }

        //줌인
        yield return AnimateFov(zoomInTime, zoomOutFov, zoomInFov);

        //Brain끄고 메인카메라를 새 플레이어 밑으로 이동
        transitCam.Priority = 0;
        brain.enabled = false;
        mainCam.fieldOfView = zoomInFov;

        mainCam.transform.SetParent(dest);
        mainCam.transform.localPosition = Vector3.zero;
        mainCam.transform.localRotation = Quaternion.identity;

        ToggleInputs(targetPlayer);
        isSwitching = false;
    }

    private IEnumerator AnimateFov(float dur, float a, float b)
    {
        for (float t = 0f; t < 1f; t += Time.deltaTime / Mathf.Max(0.0001f, dur))
        {
            transitCam.m_Lens.FieldOfView = Mathf.LerpUnclamped(a, b, Mathf.SmoothStep(0f, 1f, t));
            yield return null;
        }

        transitCam.m_Lens.FieldOfView = b;
    }

    private void ToggleInputs(GameObject who)
    {
        player1.GetComponent<PlayerInput>().enabled = (who == player1);
        player2.GetComponent<PlayerInput>().enabled = (who == player2);
        player3.GetComponent<PlayerInput>().enabled = (who == player3);
    }
}
