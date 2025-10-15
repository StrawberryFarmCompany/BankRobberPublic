using NodeDefines;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GoldBar : IInteractable
{
    public Vector3Int tile { get; set; }
    public GameObject consumeItems;
    public Transform tr;


    public void Init(Vector3Int tile, Transform tr, GameObject consumeItems)
    {
        this.tile = tile;
        this.tr = tr;
        this.consumeItems = consumeItems;
        RegistInteraction(OnInteraction);
    }

    public void OnInteraction(EntityStats stat)
    {
        NodePlayerManager.GetInstance.GetCurrentPlayer().animationController.InteractionState(tr.transform.position);
        if (NodePlayerManager.GetInstance.GetCurrentPlayer().fullBackPack != null) return;
        NodePlayerManager.GetInstance.GetCurrentPlayer().GetGold();

        GameObject.Destroy(consumeItems);
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
            GameManager.GetInstance.Nodes[vecs[i]].AddInteraction(OnInteraction,InteractionType.GoldBar.ToString());
        }
    }
    public void ReleaseInteraction(Interaction interaction)
    {
        List<Vector3Int> vecs = GameManager.GetInstance.GetNearNodes(tile);
        for (int i = 0; i < vecs.Count; i++)
        {
            GameManager.GetInstance.Nodes[vecs[i]].RemoveInteraction(OnInteraction, InteractionType.GoldBar.ToString());
        }
    }

    public void OnBehaviour()
    {
        
    }

}
