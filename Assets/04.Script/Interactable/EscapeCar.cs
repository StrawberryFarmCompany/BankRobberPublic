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
        if (stat.isFullBag)
        {
            GameManager.GetInstance.GatherGoldAndScore();
            NodePlayerManager.GetInstance.SetEscapeCondition(stat, EscapeCondition.SuccessHeist);
        }
        else
        {
            NodePlayerManager.GetInstance.SetEscapeCondition(stat, EscapeCondition.Escape);
        }
        UIManager.GetInstance.gameEndUI.SetEscapeCharacter(stat);
        stat.OnReset?.Invoke();
        stat.DestroyEntity();
        stat.thisGameObject.SetActive(false);
        if(NodePlayerManager.GetInstance.GetAllPlayers().Count <= 0)
        {
            NodePlayerManager.GetInstance.LateGameEndCall();
        }
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
