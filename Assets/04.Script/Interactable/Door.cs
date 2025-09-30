using DG.Tweening;
using NodeDefines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.AI.Navigation;

public class Door : IInteractable
{
    public Vector3Int tile { get; set; }
    public Transform tr;
    public ILock lockModule;
    private Vector3 defaultRotation;
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
        defaultRotation = tr.rotation.eulerAngles;
        RegistInteraction(OnInteraction);
    }
    public void OnInteraction(EntityStats stat)
    {
        if (lockModule.IsLock(stat))
        {
            //이동 가능 불가 여부 추후 추가 필요
            Vector3 targetRot;

            targetRot = defaultRotation + (Vector3.up * 90);
            tr.transform.DORotate(targetRot,0.7f).OnComplete(RebuildAllNavMeshes);
            GameManager.GetInstance.Nodes[tile].isWalkable = true;
            ReleaseInteraction(OnInteraction);
            RegistInteraction(UnInteraction);
        }
    }
    public void UnInteraction(EntityStats stat)
    {
        //이동 가능 불가 여부 추후 추가 필요
        tr.transform.DORotate(defaultRotation, 0.7f).OnComplete(RebuildAllNavMeshes);
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
            GameManager.GetInstance.Nodes[vecs[i]].RemoveInteraction(OnInteraction, lockModule.ToString() + InteractionType.Door.ToString());
        }
    }

    private static NavMeshSurface[] _cached;
    private static void RebuildAllNavMeshes()
    {
        if (_cached == null || _cached.Length == 0)
            _cached = Object.FindObjectsOfType<NavMeshSurface>(); // 씬 내 Surface 전부 수집

        for (int i = 0; i < _cached.Length; i++)
        {
            if (_cached[i] != null)
            {
                _cached[i].BuildNavMesh();
            }
        }

        //foreach (var s in _cached)
        //{
        //    if (s != null) s.BuildNavMesh();
        //}
    }
}
public enum DoorLockType{none,lockPick,keyCard}