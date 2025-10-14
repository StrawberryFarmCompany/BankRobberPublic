using DG.Tweening;
using NodeDefines;
using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.UIElements;

public class VaultDoor : IInteractable
{
    //금고 칸 중 0,(1,2),3에 해당하는 위치
    public Vector3Int tile { get; set; }
    public Vector3Int tileTwo { get; set; }
    public Transform tr;
    public KeyCardLock lockModule;
    private Vector3 defaultRotation;
    /// <summary>
    /// 
    /// </summary>
    /// <param name="tr">대상 문</param>
    /// <param name="type">도어락 타입</param>
    /// <param name="doorValue">키카드 == 카드 인덱스,락핏 == 문을 따는 최소 밸류</param>

    private bool isOpen;
    private bool isDrillInstalled = false;
    private bool isBattle { get { return GameManager.GetInstance.CurrentPhase == GamePhase.Battle; } }
    private byte drillCount = 4;
    private string registedName;

    public void Init(Vector3Int[] tile, Transform tr,int doorValue)
    {
        this.tile = tile[0];
        this.tileTwo = tile[1];
        this.tr = tr;
        lockModule = new KeyCardLock(doorValue);
        defaultRotation = tr.rotation.eulerAngles;

        isOpen = false;
        //배틀페이즈 전환 시 기존 등록 
        RegistInteraction(OnInteraction);
        SecurityData.OnBattlePhase += OnPhaseChanged;
    }
    private void OnPhaseChanged()
    {
        if (lockModule.released)
        {
            return;
        }
        else
        {
            ReleaseInteraction(OnInteraction);
            RegistInteraction(OnInstallDrill);
        }
    }
    public void OnInteraction(EntityStats stat)
    {
        if (lockModule.IsLock(stat) && !isOpen)
        {
            //이동 가능 불가 여부 추후 추가 필요
            DoorOpen();

            ReleaseInteraction(OnInteraction);
            RegistInteraction(UnInteraction);
        }
    }

    public void UnInteraction(EntityStats stat)
    {
        if (!isOpen) return;
        DoorClose();

        ReleaseInteraction(UnInteraction);
        RegistInteraction(OnInteraction);
    }
    public void OnInstallDrill(EntityStats stat)
    {
        if (isDrillInstalled)
        {
            Debug.Log("이미 드릴이 설치되었습니다.");
            return;
        }
        isDrillInstalled = true;
        GameManager.GetInstance.BattleTurn.BuffCount += OnDrillCounting;
        ReleaseInteraction(OnInstallDrill);
    }
    public void OnDrillCounting()
    {
        --drillCount;
        Debug.Log($"금고가 열릴때까지 남은 턴 : {drillCount}");
        if (drillCount == 0)
        {
            lockModule.released = true;
            GameManager.GetInstance.BattleTurn.BuffCount -= OnDrillCounting;
            OnInteraction(null);
        }
    }
    public void DoorOpen()
    {
        Vector3 targetRot = defaultRotation + (Vector3.up * 90);
        tr.DORotate(targetRot, 0.7f);
        GameManager.GetInstance.Nodes[tile].isWalkable = true;
        GameManager.GetInstance.Nodes[tileTwo].isWalkable = true;
        isOpen = true;
    }
    public void DoorClose()
    {
        tr.transform.DORotate(defaultRotation, 0.7f);
        GameManager.GetInstance.Nodes[tile].isWalkable = false;
        GameManager.GetInstance.Nodes[tileTwo].isWalkable = false;
        isOpen = false;
    }
    public void RegistInteraction(Interaction interaction)
    {
        List<Vector3Int> vecs = GameManager.GetInstance.GetNearNodes(tile);
        if (isBattle)
        {
            if (lockModule.released)
            {
                registedName = isOpen? "Close Door" : "Open Door";
            }
            else
            {
                registedName = "Install Drill On VaultDoor";
                //특정 
            }
        }
        else
        {
            if (lockModule.released)
            {
                registedName = isOpen ? "Close Door" : "Open Door";
            }
            else
            {
                registedName = "Tag KeyCard On VaultDoor";
                //특정 
            }
        }
        for (int i = 0; i < vecs.Count; i++)
        {
            GameManager.GetInstance.Nodes[vecs[i]].AddInteraction(interaction, registedName);
        }
    }
    public void ReleaseInteraction(Interaction interaction)
    {
        
        List<Vector3Int> vecs = GameManager.GetInstance.GetNearNodes(tile);
        for (int i = 0; i < vecs.Count; i++)
        {
            GameManager.GetInstance.Nodes[vecs[i]].RemoveInteraction(interaction, registedName);
        }
        registedName = null;
    }
}