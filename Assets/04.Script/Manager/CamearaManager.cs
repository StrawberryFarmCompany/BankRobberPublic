//using System;
//using System.Collections;
//using System.Collections.Generic;
//using Cinemachine;
//using Unity.VisualScripting;
//using UnityEngine;

//public class CameraManager : Singleton<CameraManager>
//{
//    [SerializeField] private CinemachineVirtualCamera freeViewCam;
//    [SerializeField] private CinemachineVirtualCamera player1Cam;
//    [SerializeField] private CinemachineVirtualCamera player2Cam;
//    [SerializeField] private CinemachineVirtualCamera player3Cam;



//    public int currentPriority = 5;
//    public int activeProiority = 20;
//    public bool isFollow = true;

//    private void Awake()
//    {
//        vcam = GetComponentInChildren<CinemachineVirtualCamera>();
//    }

//    private void OnTriggerEnter2D(Collider2D collision)
//    {
//        if (collision.gameObject.GetComponent<PlayerController>())
//        {
//            vcam.Priority = activeProiority;
//            if (isFollow) vcam.Follow = collision.gameObject.transform;
//            PlayerManager.Instance.player.playerController.vcam = vcam;

//        }
//    }

//    private void OnTriggerExit2D(Collider2D collision)
//    {
//        if (collision.gameObject.GetComponent<PlayerController>())
//        {
//            vcam.Priority = currentPriority;
//            vcam.Follow = null;
//        }
//    }


//}
