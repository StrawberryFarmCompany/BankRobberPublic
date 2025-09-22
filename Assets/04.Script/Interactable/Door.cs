using DG.Tweening;
using NodeDefines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : IInteractable
{
    public Vector3Int tile { get; set; }
    public Transform tr;
    public ILock lockModule;
    /// <summary>
    /// 
    /// </summary>
    /// <param name="tr">대상 문</param>
    /// <param name="type">도어락 타입</param>
    /// <param name="doorValue">키카드 == 카드 인덱스,락핏 == 문을 따는 최소 밸류</param>
    public void Init(Vector3Int tile, Transform tr, DoorLockType type,int doorValue)
    {
        this.tile = tile;
        this.tr = tr;
        lockModule = ILock.Factory(type, doorValue);
        RegistInteraction(OnInteraction);
    }
    public void OnInteraction(PlayerStats stat)
    {
        if (lockModule.IsLock(stat))
        {
            //이동 가능 불가 여부 추후 추가 필요
            tr.transform.DORotate(new Vector3(0f, 90f, 0f),0.7f);
            GameManager.GetInstance.Nodes[tile].isWalkable = true;
            ReleaseInteraction(OnInteraction);
            RegistInteraction(UnInteraction);
        }
    }
    public void UnInteraction(PlayerStats stat)
    {
        //이동 가능 불가 여부 추후 추가 필요
        tr.transform.DORotate(new Vector3(0f, 0f, 0f), 0.7f);
        GameManager.GetInstance.Nodes[tile].isWalkable = false;

    }
    public void RegistInteraction(Interaction interaction)
    {
        List<Vector3Int> vecs = GameManager.GetInstance.GetNearNodes(tile);
        for (int i = 0; i < vecs.Count; i++)
        {
            GameManager.GetInstance.Nodes[vecs[i]].AddInteraction(OnInteraction, lockModule.ToString() + InteractionType.Door.ToString());
        }
    }
    public void ReleaseInteraction(Interaction interaction)
    {
        List<Vector3Int> vecs = GameManager.GetInstance.GetNearNodes(tile);
        for (int i = 0; i < vecs.Count; i++)
        {
            GameManager.GetInstance.Nodes[vecs[i]].AddInteraction(OnInteraction, lockModule.ToString() + InteractionType.Door.ToString());
        }
    }
}
public enum DoorLockType{none,lockPick,keyCard}