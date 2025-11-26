using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;
public class GrenadeThrower : MonoBehaviour
{
    // Start is called before the first frame update
    private Camera mainCam;
    private Camera bridgeCam;
    [SerializeField] private PlayerInput input;


    [Header("Look")]
    [SerializeField] private float minXLook;
    [SerializeField] private float maxXLook;
    private float camCurXRot;
    private float camCurYRot;
    [SerializeField]private float lookSensitivity;
    public Transform cameraPos;
    public GameObject grenadePrefab { get {return (GameObject)ResourceManager.GetInstance.GetPreLoad["Grenade"]; } }
    public Queue<Grenade> grenadePool = new Queue<Grenade>();
    IEnumerator Start()
    {
        if (ResourceManager.GetInstance.GetBuffData.Count <= 0) yield return new WaitUntil(() => ResourceManager.GetInstance.GetBuffData.Count > 0);
        mainCam = Camera.main;
        bridgeCam = new GameObject("BridgeCam").AddComponent<Camera>();
        bridgeCam.enabled = false;
        bridgeCam.GetUniversalAdditionalCameraData().renderPostProcessing = true;
        input.enabled = false;
    }
    private void CamSwitchMove(Camera last, Camera next,Quaternion targetRot,Vector3 pos,bool throwSequence)
    {
        bridgeCam.transform.DOKill(false);
        last.enabled = false;
        bridgeCam.transform.position = last.transform.position;
        bridgeCam.transform.rotation = last.transform.rotation;
        bridgeCam.enabled = true;
        
        bridgeCam.transform.DORotateQuaternion(targetRot,0.3f);
        input.enabled = false;

        bridgeCam.transform.DOMove(pos, 0.5f).OnComplete
        (
            () =>
            {
                last.enabled = false;
                next.enabled = true;

                input.enabled = throwSequence;
                if (NodePlayerManager.GetInstance != null && throwSequence)
                {
                    var players = NodePlayerManager.GetInstance.GetAllPlayers();
                    for (int i = 0; i < players.Count; i++)
                    {
                        players[i].playerInput.enabled = false;
                    }

                    
                }
                
            }
        );
    }
    public void SetThrowingSequence(NodePlayerController player)
    {
        CamSwitchMove(mainCam, bridgeCam, player.transform.rotation,player.transform.position + (Vector3.up*1.8f),true);
        FloorCullingManager.GetInstance?.EnableAllCollisionsAndRenderers();
    }

    public void SetStrategySequence()
    {
        CamSwitchMove(bridgeCam, mainCam, mainCam.transform.rotation , mainCam.transform.position,false);
        NodePlayerManager.GetInstance.RefreshPlayer(true);
    }

    public void OnThrow(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            Grenade grenade = GrenadeDequeue();
            grenade.transform.position = bridgeCam.transform.position;
            grenade.OnThrow(bridgeCam.transform.forward, 600f);
            SetStrategySequence();
        }
    }

    public void OnCamRotate(InputAction.CallbackContext ctx)
    {
        if (PauseManager.isPaused) return;
        Vector2 temp = ctx.ReadValue<Vector2>();

        camCurXRot += temp.y * lookSensitivity;
        camCurXRot = Mathf.Clamp(camCurXRot, minXLook, maxXLook);
        camCurYRot += temp.x * lookSensitivity;
        bridgeCam.transform.localEulerAngles = new Vector3(-camCurXRot, camCurYRot, 0);
    }
    public void OnExit(InputAction.CallbackContext ctx)
    {
        if (ctx.started) RecoverControll();
    }


    private void GrenadeEnqueue(Grenade grenade)
    {
        grenade.gameObject.SetActive(false);
        grenadePool.Enqueue(grenade);
    }


    private void RecoverControll()
    {
        NodePlayerManager.GetInstance.RefreshPlayer(false);

        input.enabled = false;
        mainCam.enabled = true;
        bridgeCam.enabled = false;

        if (NodePlayerManager.GetInstance == null) return;

        var players = NodePlayerManager.GetInstance.GetAllPlayers();
        for (int i = 0; i < players.Count; i++)
        {
            players[i].playerInput.enabled = true;
        }
    }
    private Grenade GrenadeDequeue()
    {
        if (grenadePool.TryDequeue(out Grenade grenade))
        {
            grenade.gameObject.SetActive(true);
        }
        else
        {
            grenade = GameObject.Instantiate(grenadePrefab).GetComponent<Grenade>();
            grenade.enqueue += GrenadeEnqueue;
            grenade.explosionDone += RecoverControll;
        }
        return grenade;
    }
    
}
