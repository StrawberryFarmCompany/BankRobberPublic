using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FollowPOV : MonoBehaviour
{
    [Header("팔로우 카메라")]
    public Cinemachine.CinemachineFreeLook playerFollowCam;

    [Header("카메라 설정")]
    public float moveXSpeed = 10f;
    public float moveYSpeed = 0.1f;
    public float speedMultiplier = 2f;
    public bool xflip = true;
    public bool yflip = true;

    private float currSpeedMultiplier = 1f;
    private bool canFollowMove = false;
    private Vector2 moveInput;
    void Update()
    {
        if (!canFollowMove) return;
        MoveCamera(moveInput);
    }

    // ==============================플레이어 추적 카메라 관련 부분================================================================================================
    public void OnCanFollowMove(InputAction.CallbackContext context)
    {
        if (context.started) canFollowMove = true;
        if (context.canceled) canFollowMove = false;
    }
    public void OnFollowMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    private void MoveCamera(Vector2 delta)
    {
        float moveX = delta.x * moveXSpeed * currSpeedMultiplier * Time.deltaTime * (xflip? -1 : 1);
        float moveY = delta.y * moveYSpeed * currSpeedMultiplier * Time.deltaTime * (yflip? -1 : 1);

        playerFollowCam.m_XAxis.Value += moveX;
        playerFollowCam.m_YAxis.Value = Mathf.Clamp01(playerFollowCam.m_YAxis.Value + moveY);
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
