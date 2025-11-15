using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportTrap : ITrap
{
    public Vector3Int tile;
    public Vector3Int targetPos;


    public TeleportTrap(Vector3Int tile, Vector3Int pos)
    {
        this.tile = tile;
        this.targetPos = pos;
    }

    public void Init()
    {
        foreach(var nodePos in GameManager.GetInstance.GetNearNodes(tile))
        {
            var node = GameManager.GetInstance.Nodes[nodePos];
            if (node.GetCenter == tile)
            {
                node.AddEvent(OnStanding);
            }
        }
        
    }

    public void OnStanding(EntityStats stat)
    {
        if (stat.thisGameObject != null)
        {
            stat.thisGameObject.transform.position = targetPos;
            stat.NodeUpdates(targetPos);
            if (stat.characterType != CharacterType.None)
                stat.thisGameObject.GetComponent<NodePlayerController>().TurnOnHighlighter();

        }
    }
}
