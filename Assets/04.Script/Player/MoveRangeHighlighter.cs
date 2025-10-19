using NodeDefines;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MoveRangeHighlighter : MonoBehaviour
{
    [Header("하이라이트 표시")]
    public static NodeBoundPreviewer normalHighlighter;
    [SerializeField] private GameObject securityAreaHighlighter;
    [SerializeField] private GameObject interactableHighlighter;

    [SerializeField] private GameObject ParentTransform;

    private List<GameObject> activeHighlights = new();
    private void Awake()
    {
        if (normalHighlighter == null)
            normalHighlighter = new NodeBoundPreviewer();
    }
    public void ShowMoveRange(Vector3Int start, int range)
    {
        ClearHighlights();
        HashSet<Vector3Int> map = new HashSet<Vector3Int>();
        //start위치까지 포함하여야 하고 음수처리 때문에 값 비교 array는 (range*2)+1
        GetPath(start, start, map,new int[(range*2)+1, (range * 2) + 1], range);
        normalHighlighter.SetMesh(map.ToArray());
        /*        for (int x = -range; x <= range; x++)
                {
                    for (int z = -range; z <= range; z++)
                    {
                        Vector3Int current = start + new Vector3Int(x, 0, z);

                        Node node = GameManager.GetInstance.GetNode(current);
                        if (node == null || !node.isWalkable)
                            continue;

                        HighlightNode(current);
                    }
                }*/
    }
    private void GetPath(Vector3Int startPos,Vector3Int currPos,HashSet<Vector3Int> map,int[,] costMap,int maxRange,int curr = 0)
    {
        int x = startPos.x - currPos.x;
        int z = startPos.z - currPos.z;
        if (Mathf.Abs(startPos.x - currPos.x) > maxRange|| Mathf.Abs(startPos.z - currPos.z) > maxRange) return;

        if (!GameManager.GetInstance.Nodes.ContainsKey(currPos)) return;
        else if (GameManager.GetInstance.Nodes[currPos] == null) return;
        else if (!GameManager.GetInstance.Nodes[currPos].isWalkable) return;
        else if (GameManager.GetInstance.Nodes[currPos].standing != null)
            if(startPos != currPos)
                if(GameManager.GetInstance.Nodes[currPos].standing.Count > 0) return;

        if (curr > maxRange) return;
        x = 0 >= x ? Mathf.Abs(x) : x + maxRange;
        z = 0 >= z ? Mathf.Abs(z) : z + maxRange;
        if (map.Contains(currPos))
        {
            if (costMap[x, z] <= curr)
            {
                return;
            }
        }

        costMap[x, z] = curr;

        map.Add(currPos);
        //추후 층계산 필요
        for (int i = 0; i < GameManager.GetInstance.nearNode.Length; i++)
        {
            GetPath(startPos, GameManager.GetInstance.nearNode[i] + currPos, map,costMap, maxRange,curr+1);
        }
    }

/*    private void HighlightNode(Vector3Int pos)
    {
        GameObject prefab = normalHighlighter;

        Vector3Int isSpecialNodePos = new Vector3Int(pos.x, pos.y-1, pos.z);

        if (SpecialNodeManager.GetInstance != null &&
            SpecialNodeManager.GetInstance.TryGetSpecialNodeType(isSpecialNodePos, out var type))
        {
            
            if (type == SpecialNodeType.Security)
            {
                prefab = securityAreaHighlighter;
            }
            else if (type == SpecialNodeType.Interactable)
                prefab = interactableHighlighter;
        }

        GameObject obj = Instantiate(prefab, pos, Quaternion.identity, ParentTransform.transform);
        activeHighlights.Add(obj);
    }*/

    public void ClearHighlights()
    {
        normalHighlighter.Enable(false);
    }
}
