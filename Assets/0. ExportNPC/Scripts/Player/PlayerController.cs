//using System;
//using UnityEngine;
//using UnityEngine.InputSystem;
//using Cinemachine;
//using System.Collections;

//public class PlayerController : MonoBehaviour
//{
//    [Header("Movement")]
//    public float moveSpeed;
//    private Vector2 curMovementInput;
//    public float jumpPower;
//    public float detectDistance;
//    public LayerMask groundLayerMask;

//    [Header("Look")]
//    public Transform cameraContainer;
//    public float minXLook;
//    public float maxXLook;
//    private float camCurXRot;
//    public float lookSensitivity;

//    [Header("Camera")]
//    public Transform cameraTransform; // 카메라 Transform을 인스펙터에서 할당

//    private Vector2 mouseDelta;

//    public bool canLook = true;
//    public Action quest;

//    private Rigidbody rigidbody;
//    private bool canDoubleJump = false;



//    private void Awake()
//    {
//        rigidbody = GetComponent<Rigidbody>();
//    }

//    void Start()
//    {
//        Cursor.lockState = CursorLockMode.Locked;
//    }
 

//    private void FixedUpdate()
//    {
//        Move();
//        //RotateToCamera(); // 추가
//    }

//    private void LateUpdate()
//    {
//        if (canLook)
//        {
//            CameraLook();
//        }
//    }

//    public void OnLookInput(InputAction.CallbackContext context)
//    {
//        mouseDelta = context.ReadValue<Vector2>();
//    }

//    public void OnMoveInput(InputAction.CallbackContext context)
//    {
//        if (context.phase == InputActionPhase.Performed)
//        {
//            curMovementInput = context.ReadValue<Vector2>();
//        }
//        else if (context.phase == InputActionPhase.Canceled)
//        {
//            curMovementInput = Vector2.zero;
//        }
//    }

//    public void OnJumpInput(InputAction.CallbackContext context)
//    {
//        if (context.phase == InputActionPhase.Started && canDoubleJump)
//        {
//            canDoubleJump = false; // 더블 점프 사용 후 불가능 상태로 설정
//            rigidbody.AddForce(Vector2.up * jumpPower, ForceMode.Impulse);
//        }
//        if (context.phase == InputActionPhase.Started && IsGrounded())
//        {
//            rigidbody.AddForce(Vector2.up * jumpPower, ForceMode.Impulse);
//            canDoubleJump = true; // 더블 점프 가능 상태로 설정
//        }

//    }


//    private void Move()
//    {
//        Vector3 dir = transform.forward * curMovementInput.y + transform.right * curMovementInput.x;
//        dir *= moveSpeed;
//        dir.y = rigidbody.velocity.y;

//        rigidbody.velocity = dir;
//    }

//    void CameraLook()
//    {
//        camCurXRot += mouseDelta.y * lookSensitivity;
//        camCurXRot = Mathf.Clamp(camCurXRot, minXLook, maxXLook);
//        cameraContainer.localEulerAngles = new Vector3(-camCurXRot, 0, 0);
//        transform.eulerAngles += new Vector3(0, mouseDelta.x * lookSensitivity, 0);
//    }

//    bool IsGrounded()
//    {
//        Ray[] rays = new Ray[4]
//        {
//            new Ray(transform.position + (transform.forward * 0.2f) + (transform.up * 0.01f), Vector3.down),
//            new Ray(transform.position + (-transform.forward * 0.2f) + (transform.up * 0.01f), Vector3.down),
//            new Ray(transform.position + (transform.right * 0.2f) + (transform.up * 0.01f), Vector3.down),
//            new Ray(transform.position + (-transform.right * 0.2f) +(transform.up * 0.01f), Vector3.down)
//        };

//        for (int i = 0; i < rays.Length; i++)
//        {
//            if (Physics.Raycast(rays[i], detectDistance, groundLayerMask))
//            {
//                return true;
//            }
//        }

//        return false;
//    }

//    public void ToggleCursor()
//    {
//        bool toggle = Cursor.lockState == CursorLockMode.Locked;
//        Cursor.lockState = toggle ? CursorLockMode.None : CursorLockMode.Locked;
//        canLook = !toggle;
//    }

//    public void OnQuesttButton(InputAction.CallbackContext callbackContext)
//    {
//        if (callbackContext.phase == InputActionPhase.Started)
//        {
//            quest?.Invoke();
//            ToggleCursor();
//        }
//    }
//}