using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;
using System.Collections;

public class FreePOV : MonoBehaviour
{
    public CinemachineVirtualCamera vcam;
    public Transform followTarget;
    [SerializeField] private BoxCollider bound;

    [Header("Move Settings")]
    public float moveSpeed = 10f;

    [Header("Default Offset")]
    private Vector3 defaultOffset = new Vector3(0, 16.5f, 0);
    public float resetDuration = 0.4f;

    private CinemachineTransposer transposer;
    private Vector2 moveInput;

    private Vector2 resetInput;
    private Vector3 player1;
    private Vector3 player2;
    private Vector3 player3;


    
    void Start()
    {
        if (vcam == null) vcam = GetComponent<CinemachineVirtualCamera>();
        transposer = vcam.GetCinemachineComponent<CinemachineTransposer>();
        if (transposer != null)
            transposer.m_FollowOffset = defaultOffset;
        Application.targetFrameRate = 60;
        defaultOffset.y = bound.size.y / 2;
    }

    void FixedUpdate()
    {
        Movement(moveInput);
        Debug.Log("Update됨");

    }


    private void Movement(Vector2 direction)
    {
        Debug.Log("작동됨"+direction);
        Vector3 move = new Vector3(direction.x, 0, direction.y) * moveSpeed * Time.deltaTime;
        if (followTarget != null)
        {
            Debug.Log("followMove: " + direction);
            followTarget.position += move;
        }
        else
        {
            Debug.Log("transMove: " + direction);
            transform.position += move;
        }


    }
    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
        Debug.Log("Input: " + moveInput);
    }

    public void OnReset(InputAction.CallbackContext context)
    {
        //if (context.performed && transposer != null)
        //{
        //    StopAllCoroutines();
        //    StartCoroutine(ResetOffsetSmooth());
        //}

        if (context.started)
        {
            transform.position = CalculatePos();
        }
    }

    private Vector3 CalculatePos()
    {
        player1 = CameraManager.GetInstance.player1Cam.Follow.position;
        player2 = CameraManager.GetInstance.player2Cam.Follow.position;
        player3 = CameraManager.GetInstance.player3Cam.Follow.position;
        Vector3 centerPos = (player1 + player2 + player3) / 3;
        centerPos.y = defaultOffset.y;
        return centerPos;
    }
    private IEnumerator ResetOffsetSmooth()
    {
        Vector3 start = transposer.m_FollowOffset;
        float t = 0f;

        while (t < resetDuration)
        {
            t += Time.deltaTime;
            float k = Mathf.SmoothStep(0f, 1f, t / resetDuration);
            transposer.m_FollowOffset = Vector3.Lerp(start, defaultOffset, k);
            yield return null;
        }

        transposer.m_FollowOffset = defaultOffset;
    }
}
