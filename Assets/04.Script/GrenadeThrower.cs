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
    private float throwForce = 8f;

    public GameObject grenadePrefab { get {return (GameObject)ResourceManager.GetInstance.GetPreLoad["Grenade"]; } }
    public Queue<Grenade> grenadePool = new Queue<Grenade>();

    [SerializeField]GameObject throwUIPannel;

    PlayerInput[] inputs;
    LineRenderer line;
    IEnumerator Start()
    {
        if (ResourceManager.GetInstance.GetBuffData.Count <= 0) yield return new WaitUntil(() => ResourceManager.GetInstance.GetBuffData.Count > 0);
        mainCam = Camera.main;
        bridgeCam = new GameObject("BridgeCam").AddComponent<Camera>();
        bridgeCam.enabled = false;
        bridgeCam.GetUniversalAdditionalCameraData().renderPostProcessing = true;
        bridgeCam.clearFlags = CameraClearFlags.SolidColor;
        bridgeCam.backgroundColor = Color.black;
        line = bridgeCam.gameObject.AddComponent<LineRenderer>();
        line.material = (Material)ResourceManager.GetInstance.GetPreLoad["PathPreviewerMat"];
        line.textureMode = LineTextureMode.Tile;
        line.startWidth = 0.1f;
        line.endWidth = 0.1f;
        line.textureScale = new Vector2(15f, 1);
        input.enabled = false;

        List<PlayerInput> inputList = new List<PlayerInput>();

        var players = NodePlayerManager.GetInstance.GetAllPlayers();
        for (int i = 0; i < players.Count; i++)
        {
            inputList.Add(players[i].playerInput);
        }
        inputList.Add(NodePlayerManager.GetInstance.GetComponent<PlayerInput>());
        inputList.Add(CameraManager.GetInstance.transform.Find("Target").GetComponent<PlayerInput>());
        inputs = inputList.ToArray();
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
                    for (int i = 0; i < inputs.Length; i++)
                    {
                        inputs[i].enabled = false;
                    }

                    
                }
                
            }
        );
    }
    public void SetThrowingSequence(NodePlayerController player)
    {
        if (inputs == null || NodePlayerManager.GetInstance.GetCurrentPlayer().playerStats.grenadeCount <= 0 || NodePlayerManager.GetInstance.GetCurrentPlayer().playerStats.curActionPoint <= 0) return;
        CamSwitchMove(mainCam, bridgeCam, player.transform.rotation,player.transform.position + (Vector3.up*1.5f),true);
        FloorCullingManager.GetInstance?.EnableAllCollisionsAndRenderers();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        UIManager.GetInstance.SetThrowingUIOnOff(false);
        NodePlayerManager.GetInstance.GetCurrentPlayer().GetComponent<Collider>().enabled = false;
        throwUIPannel.SetActive(true);
        line.enabled = true;
    }

    public void SetStrategySequence()
    {
        CamSwitchMove(bridgeCam, mainCam, mainCam.transform.rotation , mainCam.transform.position,false);
    }

    public void OnThrow(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            Grenade grenade = GrenadeDequeue();
            grenade.transform.position = bridgeCam.transform.position;
            grenade.OnThrow(bridgeCam.transform.forward, throwForce);
            SetStrategySequence();
            NodePlayerManager.GetInstance.GetCurrentPlayer().playerStats.ConsumeActionPoint(1);
            NodePlayerManager.GetInstance.GetCurrentPlayer().playerStats.grenadeCount -= 1;
            throwUIPannel.SetActive(false);
            line.enabled = false;
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
        SetLine();
    }
    public void OnExit(InputAction.CallbackContext ctx)
    {
        if (ctx.started) 
        {
            RecoverControll();
            throwUIPannel.SetActive(false);
            line.enabled = false;
        }
    }


    private void GrenadeEnqueue(Grenade grenade)
    {
        grenade.gameObject.SetActive(false);
        grenadePool.Enqueue(grenade);
    }


    private void RecoverControll()
    {
        NodePlayerManager.GetInstance.RefreshPlayer(false);
        NodePlayerManager.GetInstance.GetCurrentPlayer().GetComponent<Collider>().enabled = true;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        UIManager.GetInstance.SetThrowingUIOnOff(true);

        input.enabled = false;
        mainCam.enabled = true;
        bridgeCam.enabled = false;

        if (NodePlayerManager.GetInstance == null) return;

        for (int i = 0; i < inputs.Length; i++)
        {
            if (inputs[i] == null) continue;
            inputs[i].enabled = true;
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
    private void SetLine()
    {
        Vector3 startPos = bridgeCam.transform.position;
        Vector3 startVelocity = bridgeCam.transform.forward * throwForce;

        float deltaTime = Time.fixedDeltaTime;  // 0.02 (물리와 동기화)
        int segmentCount = 32;

        List<Vector3> positions = new List<Vector3>();
        Vector3 currentPos = startPos;
        Vector3 currentVel = startVelocity;

        for (int i = 0; i < segmentCount; i++)
        {
            positions.Add(currentPos);

            // 물리 엔진과 동일한 업데이트 순서
            currentVel += Physics.gravity * deltaTime;              // 1. 속도 먼저 중력 적용
            currentPos += currentVel * deltaTime;                   // 2. 새 속도로 위치 이동

        }

        line.useWorldSpace = true;
        line.positionCount = positions.Count;
        line.SetPositions(positions.ToArray());
    }

}
