using NodeDefines;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GoldBar : IInteractable
{
    public Vector3Int tile { get; set; }
    public GameObject[] consumeItems;
    

    public void Init(Vector3Int tile, GameObject[] consumeItems)
    {
        this.tile = tile;
        this.consumeItems = consumeItems;
        RegistInteraction(OnInteraction);
    }

    public void OnInteraction(EntityStats stat)
    {
        if (NodePlayerManager.GetInstance.GetCurrentPlayer().fullBackPack != null) return;
        NodePlayerManager.GetInstance.GetCurrentPlayer().GetGold();
        //GameManager.GetInstance.Nodes[tile].isWalkable = true;

        foreach (GameObject obj in consumeItems)
        {
            if (obj != null)
                GameObject.Destroy(obj);
        }
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
