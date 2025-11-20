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
    public ILock lockModule;
    private Vector3 defaultRotation;
    /// <summary>
    /// 
    /// </summary>
    /// <param name="tr">대상 문</param>
    /// <param name="type">도어락 타입</param>
    /// <param name="doorValue">키카드 == 카드 인덱스,락핏 == 문을 따는 최소 밸류</param>

    private bool isOpen;
    private bool unlockInstalled = false;
    private bool isBattle { get { return GameManager.GetInstance.CurrentPhase == GamePhase.Battle; } }
    private string registedName;

    public void Init(Vector3Int[] tile, Transform tr,int doorValue,DoorLockType lockType)
    {
        this.tile = tile[0];
        this.tileTwo = tile[1];
        this.tr = tr;
        lockModule = ILock.Factory(lockType, doorValue, "금고",tile[0],0);
        defaultRotation = tr.rotation.eulerAngles;

        isOpen = false;
        //배틀페이즈 전환 시 기존 등록 
        RegistInteraction(OnInteraction);
        SecurityData.OnBattlePhase += OnPhaseChanged;
    }
    private void OnPhaseChanged()
    {
        if (lockModule.IsLock() || unlockInstalled)
        {
            return;
        }
        else
        {
            ReleaseInteraction(OnInteraction);
            lockModule = ILock.Factory(DoorLockType.bomb, 2, "금고",tile,2);//폭탄방식으로 모듈 변경
        }
    }
    public void OnInteraction(EntityStats stat)
    {
        if(stat.characterType != CharacterType.None) NodePlayerManager.GetInstance.GetCurrentPlayer().animationController.InteractionState(tr.transform.position);
        else//NPC일때
        {
            DoorOpen();
            ReleaseInteraction(OnInteraction);
            RegistInteraction(UnInteraction);
        }
        bool lockCheck = lockModule.TryUnLock(stat);
        if (lockCheck && !isOpen)
        {
            //이동 가능 불가 여부 추후 추가 필요
            if(unlockInstalled) GameManager.GetInstance.NoneBattleTurn.BuffCount -= DoorOpen;
            DoorOpen();
        }
        else if (!unlockInstalled && !lockCheck)
        {
            unlockInstalled = true;
            GameManager.GetInstance.NoneBattleTurn.BuffCount += DoorOpen;
        }

    }

    public void UnInteraction(EntityStats stat)
    {
        NodePlayerManager.GetInstance.GetCurrentPlayer().animationController.InteractionState(tr.transform.position);
        DoorClose();
    }
    public void DoorOpen()
    {
        if (lockModule.IsLock())
        {
            unlockInstalled = false;
            Vector3 targetRot = defaultRotation + (Vector3.up * 90);
            tr.DORotate(targetRot, 0.7f);
            GameManager.GetInstance.Nodes[tile].isWalkable = true;
            GameManager.GetInstance.Nodes[tileTwo].isWalkable = true;
            isOpen = true;
            ReleaseInteraction(OnInteraction);
            RegistInteraction(UnInteraction);
        }
    }
    public void DoorClose()
    {
        tr.transform.DORotate(defaultRotation, 0.7f);
        GameManager.GetInstance.Nodes[tile].isWalkable = false;
        GameManager.GetInstance.Nodes[tileTwo].isWalkable = false;
        isOpen = false;
        ReleaseInteraction(UnInteraction);
        RegistInteraction(OnInteraction);
    }
    public void RegistInteraction(Interaction interaction)
    {
        List<Vector3Int> vecs = GameManager.GetInstance.GetNearNodes(tile);
        if (isBattle)
        {
            if (lockModule.IsLock())
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
            if (lockModule.IsLock())
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