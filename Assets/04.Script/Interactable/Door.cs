using DG.Tweening;
using NodeDefines;
using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.UIElements;

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

    private bool isOpen;
    private string keyOpen;
    private string keyClose;

    private string doorId;

    public void Init(Vector3Int tile, Transform tr, DoorLockType type,int doorValue)
    {
        this.tile = tile;
        this.tr = tr;
        lockModule = ILock.Factory(type, doorValue);

        defaultRotation = tr.rotation.eulerAngles;


        //문 인스턴스 식별자(좌표 기반)
        doorId = $"{tile.x},{tile.z}";

        //유니크 키(같은 타입 문 여러 개여도 각각 분리됨)
        keyOpen = $"{lockModule}:{InteractionType.Door}:{doorId}:Open";
        keyClose = $"{lockModule}:{InteractionType.Door}:{doorId}:Close";

        isOpen = false;

        RegistInteraction(OnInteraction);
    }
    public void OnInteraction(EntityStats stat)
    {
        if (lockModule.IsLock(stat) && !isOpen)
        {
            //이동 가능 불가 여부 추후 추가 필요
            Vector3 targetRot;

            targetRot = defaultRotation + (Vector3.up * 90);
            tr.transform.DORotate(targetRot, 0.7f);/*.OnComplete(RebuildAllNavMeshes);*/

            isOpen = true;

            NodePlayerManager.GetInstance.GetCurrentPlayer().animationController.InteractionState(tr.transform.position);
            GameManager.GetInstance.Nodes[tile].isWalkable = true;

            ReleaseInteraction(OnInteraction);
            RegistInteraction(UnInteraction);
        }
    }
    public void UnInteraction(EntityStats stat)
    {
        NodePlayerManager.GetInstance.GetCurrentPlayer().animationController.InteractionState(tr.transform.position);
        if (!isOpen) return;
        //이동 가능 불가 여부 추후 추가 필요
        tr.transform.DORotate(defaultRotation, 0.7f)/*.OnComplete(RebuildAllNavMeshes)*/;
        GameManager.GetInstance.Nodes[tile].isWalkable = false;

        isOpen = false;

        ReleaseInteraction(UnInteraction);
        RegistInteraction(OnInteraction);
    }
    public void RegistInteraction(Interaction interaction)
    {
        List<Vector3Int> vecs = GameManager.GetInstance.GetNearNodes(tile);
        string key = (interaction == OnInteraction) ? keyOpen : keyClose;
        for (int i = 0; i < vecs.Count; i++)
        {
            GameManager.GetInstance.Nodes[vecs[i]].AddInteraction(interaction, key/*OnInteraction, lockModule.ToString() + InteractionType.Door.ToString()*/);
        }
    }
    public void ReleaseInteraction(Interaction interaction)
    {
        List<Vector3Int> vecs = GameManager.GetInstance.GetNearNodes(tile);
        string key = (interaction == OnInteraction) ? keyOpen : keyClose;
        for (int i = 0; i < vecs.Count; i++)
        {
            GameManager.GetInstance.Nodes[vecs[i]].RemoveInteraction(interaction, key/*OnInteraction, lockModule.ToString() + InteractionType.Door.ToString()*/);
        }
    }

/*    private static NavMeshSurface[] _cached;
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
    }*/
}
public enum DoorLockType{none,lockPick,keyCard}