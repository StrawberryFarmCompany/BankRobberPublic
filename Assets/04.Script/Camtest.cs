using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Rendering.Universal;
public class Camtest : MonoBehaviour
{
    // Start is called before the first frame update
    private Camera mainCam;
    private Camera bridgeCam;
    private bool ThrowingSequence
    {
        get { return mainCam.enabled == false; }
    }
    public Transform cameraPos;
    void Start()
    {
        mainCam = Camera.main;
        bridgeCam = new GameObject("BridgeCam").AddComponent<Camera>();
        bridgeCam.enabled = false;
        bridgeCam.GetUniversalAdditionalCameraData().renderPostProcessing = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            CamSwitchMove(mainCam, bridgeCam, cameraPos);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        { 
            CamSwitchMove(bridgeCam,mainCam, mainCam.transform);
        }
    }
    private void CamSwitchMove(Camera last, Camera next,Transform target)
    {
        bridgeCam.DOKill(false);
        last.enabled = false;
        bridgeCam.transform.position = last.transform.position;
        bridgeCam.transform.rotation = last.transform.rotation;
        bridgeCam.enabled = true;
        
        bridgeCam.transform.DORotateQuaternion(target.rotation,0.1f);

        bridgeCam.transform.DOMove(target.position, 0.3f).OnComplete
        (
            () =>
            {
                last.enabled = false;
                next.enabled = true;
            }
        );
    }
}
