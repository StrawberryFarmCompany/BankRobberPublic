using NodeDefines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EscapeCar : IInteractable
{
    public Vector3Int tile { get; set; }

    public void Init(Vector3Int tile)
    {
        this.tile = tile;
        RegistInteraction(OnInteraction);
    }

    public void OnInteraction(EntityStats stat)
    {
        stat.OnReset?.Invoke();
        stat.OnReset = null;
    }
    public void UnInteraction(EntityStats stat)
    {

    }
    public void RegistInteraction(Interaction interaction)
    {
        GameManager.GetInstance.Nodes[tile].AddInteraction(OnInteraction, InteractionType.EscapeCar.ToString());
    }
    public void ReleaseInteraction(Interaction interaction)
    {
        GameManager.GetInstance.Nodes[tile].RemoveInteraction(OnInteraction, InteractionType.EscapeCar.ToString());
    }
}
