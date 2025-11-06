using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialTrap : ITrap
{
    public Vector3Int tile;
    public Vector3Int targetPos;


    public TutorialTrap(Vector3Int tile, Vector3Int pos)
    {
        this.tile = tile;
        this.targetPos = pos;
    }

    public void Init()
    {
        var node = GameManager.GetInstance.Nodes[
            GameManager.GetInstance.GetNearNodes(tile)[0]
        ];

        node.AddEvent(OnStanding);
    }

    public void OnStanding(EntityStats stat)
    {
        if(stat.thisGameObject != null)
        {
            stat.thisGameObject.transform.position = targetPos;
            stat.NodeUpdates(targetPos);

        }
    }
}
