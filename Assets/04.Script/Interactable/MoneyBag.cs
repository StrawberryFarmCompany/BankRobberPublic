using NodeDefines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyBag : IInteractable
{
    public Vector3Int tile { get; set; }

    public void OnInteraction(EntityStats stat)
    {
        
    }

    public void UnInteraction(EntityStats stat)
    {
        
    }


    public void RegistInteraction(Interaction interaction)
    {
        List<Vector3Int> vecs = GameManager.GetInstance.GetNearNodes(tile);
        for (int i = 0; i < vecs.Count; i++)
        {
            GameManager.GetInstance.Nodes[vecs[i]].AddInteraction(OnInteraction, InteractionType.MoneyBag.ToString());
        }
    }
    public void ReleaseInteraction(Interaction interaction)
    {
        List<Vector3Int> vecs = GameManager.GetInstance.GetNearNodes(tile);
        for (int i = 0; i < vecs.Count; i++)
        {
            GameManager.GetInstance.Nodes[vecs[i]].AddInteraction(OnInteraction, InteractionType.MoneyBag.ToString());
        }
    }
}
