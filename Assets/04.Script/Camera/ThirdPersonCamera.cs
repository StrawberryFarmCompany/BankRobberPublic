using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ThirdPersonCamera : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;          //플레이어
    [SerializeField] private float pivotHeight = 1.7f;  //어깨쯤
    [SerializeField] private float distance = 3.8f;     //고정 거리
    [SerializeField] private float startPitch = 22f;    //기본 각도

    [Header("Mouse Look")]
    [SerializeField] private float sensX = 0.15f;       //좌우
    [SerializeField] private float sensY = 0.15f;       //상하
    [SerializeField] private float minPitch = -15f;     //위로 제한
    [SerializeField] private float maxPitch = 70f;      //아래로 제한

    private float yaw, pitch;

    void Awake()
    {
        if (!target)
        {
            enabled = false;
            return;
        }

        if (maxPitch <= minPitch) maxPitch = minPitch + 1f;     //뒤집힘 방지
        if (distance < 0.2f) distance = 0.2f;                   //카메라 가까워지는 경우 방지
        if (pivotHeight < 0f) pivotHeight = 0f;                 //땅 아래로 내려가는 문제 방지

        yaw = target.eulerAngles.y;
        pitch = Mathf.Clamp(startPitch, minPitch, maxPitch);
    }

    void Update()
    {
        if (Mouse.current == null) return;

        Vector2 d = Mouse.current.delta.ReadValue();                    //마우스 이동량
        yaw += d.x * sensX;                                             //좌우 이동 
        pitch = Mathf.Clamp(pitch - d.y * sensY, minPitch, maxPitch);   //상하 이동
    }

    void LateUpdate()
    {
        Quaternion rot = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 pivot = target.position + Vector3.up * pivotHeight;
        Vector3 pos = pivot + rot * new Vector3(0f, 0f, -distance);

        transform.SetPositionAndRotation(pos, rot);
    }
}
