using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;
    public float jumpPower;
    private Vector2 curMovementInput;
    public LayerMask groundLayerMask;

    [Header("Look")]
    public Transform cameraContainer;
    public float minXLook;
    public float maxXLook;
    private float camCurXRot;
    public float lookSensitivity;

    public void SetLookSensitivity(float v)
    {
        lookSensitivity = Mathf.Clamp(v, 0.01f, 0.5f);
    }

    private Rigidbody _rigidbody;
    public bool canLook = true;

    public Action quest;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void FixedUpdate()
    {
        if (PauseManager.isPaused) return;
        Move();
    }

    // 카메라 움직임 값 구현 및 Player Input에 전달
    public void CameraLook(InputAction.CallbackContext context)
    {
        if (PauseManager.isPaused) return;
        if (!canLook) return;
        Vector2 temp = context.ReadValue<Vector2>();

        camCurXRot += temp.y * lookSensitivity;
        camCurXRot = Mathf.Clamp(camCurXRot, minXLook, maxXLook);
        cameraContainer.localEulerAngles = new Vector3(-camCurXRot, 0, 0);

        transform.eulerAngles += new Vector3(0, temp.x * lookSensitivity, 0);
    }

    // 움직임 값 구현
    void Move()
    {
        if(curMovementInput == Vector2.zero)
        {
            _rigidbody.velocity = new Vector3(0, _rigidbody.velocity.y , 0);
            return;
        }
        Vector3 dir = transform.forward * curMovementInput.y + transform.right * curMovementInput.x; // W->(0,1), S->(0,-1), D->(1,0), A->(-1,0) 를 통해 네 방향값 정해 주기
        dir *= moveSpeed;
        dir.y = _rigidbody.velocity.y;  // y에 velocity.y값 넣은 이유는 점프 할 때만 위아래로 움직여야 해서 

        _rigidbody.velocity = dir;
    }

    // 움직임을 Player Input에 전달
    public void OnMove(InputAction.CallbackContext context)
    {
        if (PauseManager.isPaused) return;
        // 움직일 때 (키보드 누르고 있을 때)
        if (context.phase == InputActionPhase.Performed)
        {
            curMovementInput = context.ReadValue<Vector2>();
        }

        // 가만히 있을 때 (키보드 안누를 때)
        else if(context.phase == InputActionPhase.Canceled)
        {
            curMovementInput = Vector2.zero;
        }
    }

    // 점프 Player Input에 전달
    public void OnJump(InputAction.CallbackContext context)
    {
        if (PauseManager.isPaused) return;
        if (context.phase == InputActionPhase.Started && IsGrounded())
        {
            _rigidbody.AddForce(Vector2.up * jumpPower, ForceMode.Impulse);
        }
    }

    bool IsGrounded()
    {
        Ray[] rays = new Ray[4]
        {
            new Ray(transform.position + (transform.forward * 0.2f), Vector3.down),
            new Ray(transform.position + (-transform.forward * 0.2f), Vector3.down),
            new Ray(transform.position + (transform.right * 0.2f), Vector3.down),
            new Ray(transform.position + (-transform.right * 0.2f), Vector3.down)
        };

        for(int i = 0; i < rays.Length; i++)
        {
            if (Physics.Raycast(rays[i], 0.1f, groundLayerMask))
            {
                return true;
            }
        }

        return false;
    }

    public void ToggleCursor()
    {
        bool toggle = Cursor.lockState == CursorLockMode.Locked;
        Cursor.lockState = toggle ? CursorLockMode.None : CursorLockMode.Locked;
        canLook = !toggle;
    }
}