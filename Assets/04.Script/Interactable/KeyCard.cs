using NodeDefines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyCard : IInteractable
{
    public Vector3Int tile { get; set; }
    private GameObject target;
    private int keyValue;

    public void Init(Vector3Int tile, Transform tr,int keyValue)
    {
        this.tile = tile;
        target = tr.gameObject;
        this.keyValue = keyValue;
        RegistInteraction(OnInteraction);
    }

    public void OnInteraction(EntityStats stat)
    {
        target.SetActive(false);
        GameManager.GetInstance.isPlayerGetKeyCard[keyValue] = true;
        ReleaseInteraction(OnInteraction);
    }
    public void UnInteraction(EntityStats stat)
    {

    }
    public void RegistInteraction(Interaction interaction)
    {
        List<Vector3Int> vecs = GameManager.GetInstance.GetNearNodes(tile);
        for (int i = 0; i < vecs.Count; i++)
        {
            GameManager.GetInstance.Nodes[vecs[i]].AddInteraction(OnInteraction, InteractionType.KeyCard.ToString());
        }
    }
    public void ReleaseInteraction(Interaction interaction)
    {
        List<Vector3Int> vecs = GameManager.GetInstance.GetNearNodes(tile);
        for (int i = 0; i < vecs.Count; i++)
        {
            GameManager.GetInstance.Nodes[vecs[i]].AddInteraction(OnInteraction, InteractionType.KeyCard.ToString());
        }
    }
}