using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;

public class FreePOV : MonoBehaviour
{
    public CinemachineVirtualCamera vcam;
    public Transform followTarget;

    [Header("Move Settings")]
    public float wasdMoveSpeed = 20f; // WASD 이동 속도
    public float moveSpeed = 0.1f;    // 마우스 이동 속도
    public float rotateSpeed = 45f;   // 회전 속도 (deg per press)

    private float yaw = 180f;            // Y축 회전 각도
    private float yawDirection;
    private const float pitch = 45f;   // 고정된 X축 각도

    public BoxCollider bound;  // Inspector에서 연결

    private bool canMove = false;      // 마우스 휠 버튼 눌림 상태
    private Vector2 moveInput;         // 마우스 델타
    private Vector2 wasdMoveInput;   // WASD 입력

    public float speedMultiplier = 1.5f;
    private float currSpeedMultiplier = 1f;
    void Start()
    {
        if (vcam == null) vcam = GetComponent<CinemachineVirtualCamera>();
       // 초기 카메라 방향 설정
        ApplyRotation();
    }

    void Update()
    {
        if (wasdMoveInput != Vector2.zero)
        {
            Movement(wasdMoveInput);
        }
        if(moveInput != Vector2.zero)
        { 
            if (canMove)
            {
                MoveCamera(moveInput);

            }
        }
        

        ApplyRotation();
    }

    private void Movement(Vector2 direction)
    {
        if (direction == Vector2.zero) return;

        // 회전 기준 벡터를 평면에 투영해서 Y값 제거
        Vector3 forward = Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;
        Vector3 right = Vector3.ProjectOnPlane(transform.right, Vector3.up).normalized;

        // XZ 이동
        Vector3 move = (right * direction.x + forward * direction.y) * wasdMoveSpeed * Time.deltaTime * currSpeedMultiplier;

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
    public void OnWASDMove(InputAction.CallbackContext context)
    {
        if (CameraManager.GetInstance.isFreeView == false) return;
        wasdMoveInput = context.ReadValue<Vector2>();
    }

    private void MoveCamera(Vector2 delta)
    {
        // 현재 카메라의 좌우/앞뒤 벡터 (XZ 평면)
        Vector3 right = transform.right;
        Vector3 forward = Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;

        // 마우스 델타 기반 위치 이동 (Y축은 화면상 위/아래로 이동)
        Vector3 move = (right * delta.x + Vector3.up * delta.y) * moveSpeed * currSpeedMultiplier;
        Vector3 newPos = transform.position + move;

        // bound 안으로 제한
        if (bound != null)
        {
            Vector3 min = bound.bounds.min;
            Vector3 max = bound.bounds.max;

            newPos.x = Mathf.Clamp(newPos.x, min.x, max.x);
            newPos.y = Mathf.Clamp(newPos.y, min.y, max.y);
            newPos.z = Mathf.Clamp(newPos.z, min.z, max.z);
        }

        transform.position = newPos;
    }

    private void ApplyRotation()
    {
        yaw = Mathf.Repeat(yaw + rotateSpeed * yawDirection * Time.deltaTime * currSpeedMultiplier, 360f);
        Quaternion rot = Quaternion.Euler(pitch, yaw, 0);
        transform.rotation = rot;
    }

    // 회전 입력
    public void OnLeftRotate(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            yawDirection = -1;
        }

        if (context.canceled)
        {
            yawDirection = 0;
        }
    }

    public void OnRightRotate(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            yawDirection = 1;
        }

        if(context.canceled)
        {
            yawDirection = 0;
        }
    }

    // 마우스 휠 버튼 눌림
    public void OnCanMove(InputAction.CallbackContext context)
    {
        if (context.started) canMove = true;
        if (context.canceled) canMove = false;
    }

    // 마우스 이동 델타
    public void OnMove(InputAction.CallbackContext context)
    {
        if (!canMove) return;
        moveInput = context.ReadValue<Vector2>();
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
