using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionObject
{
    // Start is called before the first frame update
    ushort boundary;
    float damage;
    Vector3Int center;
    Dictionary<Vector3Int, float> damageMap;
    public ExplosionObject(ushort boundary, float damage, Vector3Int center)
    {
        this.boundary = (ushort)(boundary * 10);
        this.damage = damage;
        this.center = center;
        damageMap = GetBound();
    }

    public void Explosion()
    {
        foreach (var item in damageMap)
        {
            NodeDefines.Node node = GameManager.GetInstance.GetNode(item.Key + center);
            if(node != null)
            {
                if(node.Standing != null)
                {
                    for (int i = 0; i < node.Standing.Count; i++)
                    {
                        node.Standing[i].Damaged(damage * item.Value);
                    }
                }
            }
        }
    }
    // bfs로 구현
    private Dictionary<Vector3Int, float> GetBound()
    {
        Dictionary<Vector3Int, int> costMap = new Dictionary<Vector3Int, int>();
        costMap.Add(Vector3Int.zero, boundary);
        Queue<Vector3Int> posQueue = new Queue<Vector3Int>();

        posQueue.Enqueue(Vector3Int.zero);

        Vector3Int[] nearNode = new Vector3Int[26] { /*동일층*/Vector3Int.forward, Vector3Int.right, Vector3Int.back, Vector3Int.left, new Vector3Int(-1, 0, -1), new Vector3Int(1, 0, 1), new Vector3Int(-1, 0, 1), new Vector3Int(1, 0, -1),
        /*-1층*/new Vector3Int(0,-1,1), new Vector3Int(1,-1,0), new Vector3Int(0,-1,-1), new Vector3Int(-1,-1,0), new Vector3Int(-1, -1, -1), new Vector3Int(1, -1, 1), new Vector3Int(-1, -1, 1), new Vector3Int(1, -1, -1),
        new Vector3Int(0,1,1), new Vector3Int(1,1,0), new Vector3Int(0,1,-1), new Vector3Int(-1,1,0), new Vector3Int(-1, 1, -1), new Vector3Int(1, 1, 1), new Vector3Int(-1, 1, 1), new Vector3Int(1, 1, -1),
        new Vector3Int(0, 1, 0),new Vector3Int(0, -1, 0)};

        int[] cost = nearNode.Select((p) => 
        {
            int currNum = Mathf.Abs(p.x) + Mathf.Abs(p.y) + Mathf.Abs(p.z);
            if (currNum <= 1) return 10;
            else if (currNum <= 2) return 14;
            else if (currNum <= 3) return 17;

            return 17;
            }).ToArray();

        while (posQueue.Count > 0)
        {
            Vector3Int currPos = posQueue.Dequeue();

            for (int i = 0; i < nearNode.Length; i++)
            {
                Vector3Int nextPos = nearNode[i] + currPos;
                int nextCost = costMap[currPos] - cost[i];
                if (nextCost >= 0)
                {
                    if (costMap.TryGetValue(nextPos,out int val))
                    {
                        if (nextCost > val) costMap[nextPos] = nextCost;
                        else continue;
                    }
                    else
                    {
                        costMap.Add(nextPos, nextCost);
                    }
                    if(nextCost > 0) posQueue.Enqueue(nextPos);
                    else costMap[nextPos] = 1;
                }
            }
        }
        float denom = boundary-1;
        Dictionary<Vector3Int, float> result = new Dictionary<Vector3Int, float>();

        foreach (var item in costMap)
        {
            result.Add(item.Key, item.Value / denom);
        }
        return result;
    }
}
