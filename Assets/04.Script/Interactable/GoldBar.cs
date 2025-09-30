using NodeDefines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldBar : IInteractable
{
    public Vector3Int tile { get; set; }
    

    public void Init(Vector3Int tile)
    {
        this.tile = tile;
        RegistInteraction(OnInteraction);
    }

    public void OnInteraction(EntityStats stat)
    {
        NodePlayerManager.GetInstance.GetCurrentPlayer().GetGold();
        GameManager.GetInstance.Nodes[tile].isWalkable = true;
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
            GameManager.GetInstance.Nodes[vecs[i]].AddInteraction(OnInteraction, InteractionType.GoldBar.ToString());
        }
    }

    public void OnBehaviour()
    {
        
    }

}
