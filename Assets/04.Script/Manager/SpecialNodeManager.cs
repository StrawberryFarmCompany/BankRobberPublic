using System.Collections.Generic;
using UnityEngine;

public class SpecialNodeManager : MonoSingleTon<SpecialNodeManager>
{

    private Dictionary<Vector3Int, SpecialNodeType> specialNodes = new();


    public void RegisterNode(Vector3Int pos, SpecialNodeType type)
    {
        specialNodes[pos] = type;
    }

    public bool TryGetSpecialNodeType(Vector3Int pos, out SpecialNodeType type)
    {
        return specialNodes.TryGetValue(pos, out type);
    }
}
