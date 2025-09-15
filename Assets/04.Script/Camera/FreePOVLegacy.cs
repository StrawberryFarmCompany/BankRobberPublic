using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;
using System.Collections;
using System.ComponentModel;

public class FreePOVLegacy : MonoBehaviour
{
    public CinemachineVirtualCamera vcam;
    [SerializeField] private BoxCollider bound;

    [Header("Move Settings")]
    public float moveSpeed = 10f;
    public float UpDownSpeed = 1f;
    public float rotateSpeed = 0.2f;

    [Header("Default Offset")]
    private Vector3 defaultOffset = new Vector3(0, 16.5f, 0);
    public float resetDuration = 0.4f;

    [Header("X축 좌우 반전")]
    public bool xflip = true;
    [Header("Y축 좌우 반전")]
    public bool yflip = true;
    
    private Vector2 moveInput;

    private Vector2 resetInput;
    private Vector3 player1;
    private Vector3 player2;
    private Vector3 player3;

    private bool isRotationMode;

    private float verticalInput; // Y축 입력

    public float speedMultiplier = 2f;
    private float currSpeedMultiplier = 1f;

    void Start()
    {
        if (vcam == null) vcam = GetComponent<CinemachineVirtualCamera>();
        Application.targetFrameRate = 60;
        defaultOffset.y = bound.size.y / 2;
    }

    void FixedUpdate()
    {
        Movement(moveInput);

    }


    private void Movement(Vector2 direction)
    {
        if (direction == Vector2.zero && Mathf.Approximately(verticalInput, 0f)) return;

        // 회전 기준 벡터를 평면에 투영해서 Y값 제거
        Vector3 forward = Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;
        Vector3 right = Vector3.ProjectOnPlane(transform.right, Vector3.up).normalized;

        // XZ 이동
        Vector3 move = (right * direction.x + forward * direction.y) * moveSpeed * Time.deltaTime * currSpeedMultiplier;

        // Y 이동 (마우스 휠)
        move += Vector3.up * verticalInput * UpDownSpeed * Time.deltaTime * currSpeedMultiplier;

        Transform target = transform;
        Vector3 newPos = target.position + move;

        if (bound != null)
        {
            Bounds b = bound.bounds;
            newPos.x = Mathf.Clamp(newPos.x, b.min.x, b.max.x);
            newPos.y = Mathf.Clamp(newPos.y, b.min.y, b.max.y);
            newPos.z = Mathf.Clamp(newPos.z, b.min.z, b.max.z);
        }

        target.position = newPos;
    }
    public void OnMove(InputAction.CallbackContext context)
    {
        if (CameraManager.GetInstance.isFreeView == false) return;
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnReset(InputAction.CallbackContext context)
    {
        if (context.started && CameraManager.GetInstance.isFreeView)
        {
            transform.position = CalculatePos();
        }
    }

    public void OnRotationMode(InputAction.CallbackContext context)
    {
        if (context.performed && CameraManager.GetInstance.isFreeView)
            isRotationMode = true;

        if (context.canceled)
            isRotationMode = false;
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        if (!CameraManager.GetInstance.isFreeView || !isRotationMode) return;

        Vector2 lookDelta = context.ReadValue<Vector2>();
        float yaw = lookDelta.x * rotateSpeed * (xflip ? 1 : -1) * currSpeedMultiplier;
        float pitch = -lookDelta.y * rotateSpeed * (yflip ? 1 : -1) * currSpeedMultiplier;

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

    public void OnUpDown(InputAction.CallbackContext context)
    {
        if (!CameraManager.GetInstance.isFreeView) return;

        // 마우스 휠 입력 읽기 (양수: 올리기, 음수: 내리기)
        verticalInput = -context.ReadValue<Vector2>().y; // Y축 반전
    }
    private Vector3 CalculatePos()
    {
        if (CameraManager.GetInstance.player1Cam.Follow == null)
            Debug.Log("없음");
        player1 = CameraManager.GetInstance.player1Cam.Follow.position;
        player2 = CameraManager.GetInstance.player2Cam.Follow.position;
        player3 = CameraManager.GetInstance.player3Cam.Follow.position;
        Vector3 centerPos = (player1 + player2 + player3) / 3;
        centerPos.y = defaultOffset.y;
        return centerPos;
    }

    public void OnSpeedUp(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            currSpeedMultiplier = speedMultiplier;
        }
        if (context.canceled)
        {
            currSpeedMultiplier = 1;
        }
    }
}