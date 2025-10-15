using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;

public class FreePOV : MonoBehaviour
{
    public CinemachineFreeLook fcam;
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

    [SerializeField] private float scrollSpeed = 0.1f;

    [SerializeField, Range(0f, 1f)] private float scrollMin = 0f;
    [SerializeField, Range(0f, 1f)] private float scrollMax = 1f;

    [SerializeField] private float minX;
    [SerializeField] private float maxX;
    [SerializeField] private float minY;
    [SerializeField] private float maxY;
    [SerializeField] private float minZ;
    [SerializeField] private float maxZ;


    void Start()
    {
        if (fcam == null) fcam = GetComponent<CinemachineFreeLook>();

        //감도 초기 적용
        ApplySensitivity();

        //감도 변경 시 자동 갱신
        if (CameraSensitivityManager.GetInstance != null)
            CameraSensitivityManager.GetInstance.OnSensitivityChanged += ApplySensitivity;

        // 초기 카메라 방향 설정
        ApplyRotation();
    }

    private void OnDestroy()
    {
        //씬 전환 등으로 FreePOV가 사라질 때 정리
        if (CameraSensitivityManager.GetInstance != null)
            CameraSensitivityManager.GetInstance.OnSensitivityChanged -= ApplySensitivity;
    }

    private void ApplySensitivity()
    {
        CameraSensitivityManager csmg = CameraSensitivityManager.GetInstance;
        if (csmg == null) return;

        moveSpeed = csmg.MoveSpeed;
        wasdMoveSpeed = csmg.WasdSpeed;
        rotateSpeed = csmg.RotateSpeed;
        scrollSpeed = csmg.ScrollSpeed;
    }

    void Update()
    {
        if (wasdMoveInput != Vector2.zero)
        {
            WasdMovement(wasdMoveInput);
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

    private void WasdMovement(Vector2 direction)
    {
        if (direction == Vector2.zero) return;

        Vector3 forward = Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;
        Vector3 right = Vector3.ProjectOnPlane(transform.right, Vector3.up).normalized;

        Vector3 move = (right * direction.x + forward * direction.y) * wasdMoveSpeed * Time.deltaTime * currSpeedMultiplier;

        Transform target = transform;
        Vector3 newPos = target.position + move;

        // min/max xyz로 이동 제한
        newPos.x = Mathf.Clamp(newPos.x, minX, maxX);
        newPos.y = Mathf.Clamp(newPos.y, minY, maxY);
        newPos.z = Mathf.Clamp(newPos.z, minZ, maxZ);

        target.position = newPos;
    }

    public void OnWASDMove(InputAction.CallbackContext context)
    {
        if (!CameraManager.GetInstance.isFreeView)
        {
            followTarget.position = NodePlayerManager.GetInstance.GetCurrentPlayer().gameObject.transform.position;
            CameraManager.GetInstance.isFreeView = true;
        }

        if(fcam.Follow != followTarget) fcam.Follow = followTarget;
        if (fcam.LookAt != followTarget) fcam.LookAt = followTarget;
        wasdMoveInput = context.ReadValue<Vector2>();
    }

    private void MoveCamera(Vector2 delta)
    {
        Vector3 right = transform.right;
        Vector3 forward = Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;

        Vector3 move = (right * delta.x + Vector3.up * delta.y) * moveSpeed * currSpeedMultiplier;
        Vector3 newPos = transform.position + move;

        // min/max xyz로 이동 제한
        newPos.x = Mathf.Clamp(newPos.x, minX, maxX);
        newPos.y = Mathf.Clamp(newPos.y, minY, maxY);
        newPos.z = Mathf.Clamp(newPos.z, minZ, maxZ);

        transform.position = newPos;
    }

    private void ApplyRotation()
    {
        // 입력값으로 yawDelta 계산
        float yawDelta = rotateSpeed * yawDirection * Time.deltaTime * currSpeedMultiplier;

        // FreeLook 카메라의 수평축(XAxis) 값 회전
        fcam.m_XAxis.Value += yawDelta;

        if (followTarget != null)
        {
            // followTarget의 Y축 회전과 동기화
            followTarget.Rotate(Vector3.up, yawDelta, Space.World);

            // 카메라 자체 위치를 따라가도록 align
            Vector3 euler = followTarget.rotation.eulerAngles;
            transform.rotation = Quaternion.Euler(0f, euler.y, 0f);
        }
    }

    // 회전 입력
    public void OnLeftRotate(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            yawDirection = 1;
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
            yawDirection = -1;
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

    public void OnScroll(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        float scrollValue = context.ReadValue<Vector2>().y;
        fcam.m_YAxis.Value += scrollValue * scrollSpeed * Time.deltaTime;

        // 값 범위 제한
        fcam.m_YAxis.Value = Mathf.Clamp(fcam.m_YAxis.Value, scrollMin, scrollMax);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        // min/max 좌표를 기반으로 박스 표시
        Vector3 center = new Vector3(
            (minX + maxX) / 2f,
            (minY + maxY) / 2f,
            (minZ + maxZ) / 2f
        );
        Vector3 size = new Vector3(
            Mathf.Abs(maxX - minX),
            Mathf.Abs(maxY - minY),
            Mathf.Abs(maxZ - minZ)
        );

        Gizmos.DrawWireCube(center, size);
    }
}
