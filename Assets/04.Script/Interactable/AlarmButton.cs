using DG.Tweening;
using NodeDefines;
using System.Collections.Generic;
using UnityEngine;

public class AlarmButton : IInteractable
{
    public Vector3Int tile { get; set; }
    public Transform tr;
    public int index;
    public ILock lockModule;

    private bool isOpen;
    private Door door;

    public void Init(Vector3Int tile, Transform tr, int index)
    {
        this.tile = tile;
        this.tr = tr;
        this.index = index;
        lockModule = ILock.Factory(DoorLockType.button, index);

        isOpen = false;
        RegistInteraction(OnInteraction);
    }

    public void OnInteraction(EntityStats stat)
    {
        NodePlayerManager.GetInstance.GetCurrentPlayer().animationController.InteractionState(tr.transform.position);
        door = GameManager.GetInstance.GetMatchButtonDoor(index);

        if (!isOpen)
        {
            if (door != null)
            {
                //문 열림
                isOpen = true;
                door.block.transform.DOScale(0, 0.5f);
                GameManager.GetInstance.Nodes[door.tile].isWalkable = true;
                NodePlayerManager.GetInstance.GetCurrentPlayer().TurnOnHighlighter();
            }
            else
            {
                //소음 발생
                NoiseManager.AddNoise(tile, NoiseType.Disarm);
            }
            ReleaseInteraction(OnInteraction);
        }
    }
    public void UnInteraction(EntityStats stat)
    {

    }
    public void RegistInteraction(Interaction interaction)
    {
        List<Vector3Int> vecs = GameManager.GetInstance.GetNearNodes(tile);
        for (int i = 0; i < vecs.Count; i++)
        {
            GameManager.GetInstance.Nodes[vecs[i]].AddInteraction(OnInteraction, $"{InteractionType.AlarmBTN.ToString()} {index}");
        }
    }
    public void ReleaseInteraction(Interaction interaction)
    {
        List<Vector3Int> vecs = GameManager.GetInstance.GetNearNodes(tile);
        for (int i = 0; i < vecs.Count; i++)
        {
            GameManager.GetInstance.Nodes[vecs[i]].RemoveInteraction(OnInteraction, $"{InteractionType.AlarmBTN.ToString()} {index}");
        }
    }
}

