using NodeDefines;
using System.Collections.Generic;
using UnityEngine;

public class MoveRangeHighlighter : MonoBehaviour
{
    [Header("하이라이트 표시")]
    [SerializeField] private GameObject normalHighlighter;
    [SerializeField] private GameObject securityAreaHighlighter;
    [SerializeField] private GameObject interactableHighlighter;

    private List<GameObject> activeHighlights = new();

    public void ShowMoveRange(Vector3Int start, int range)
    {
        ClearHighlights();
        HashSet<Vector3Int> map = new HashSet<Vector3Int>();
        GetPath(start, start, map, range);

        foreach (Vector3Int item in map)
        {
            HighlightNode(item);
        }
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
    private void GetPath(Vector3Int startPos,Vector3Int currPos,HashSet<Vector3Int> map,int maxRange)
    {
        if (Mathf.Abs(startPos.x - currPos.x) > maxRange|| Mathf.Abs(startPos.z - currPos.z) > maxRange) return;

        if (!GameManager.GetInstance.Nodes.ContainsKey(currPos)) return;
        else if (!GameManager.GetInstance.Nodes[currPos].isWalkable) return;

        if (map.Contains(currPos)) return;

        map.Add(currPos);
        for (int i = 0; i < GameManager.GetInstance.nearNode.Length; i++)
        {
            GetPath(startPos, GameManager.GetInstance.nearNode[i] + currPos, map, maxRange);
        }
    }

    private void HighlightNode(Vector3Int pos)
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

        GameObject obj = Instantiate(prefab, pos, Quaternion.identity);
        activeHighlights.Add(obj);
    }

    public void ClearHighlights()
    {
        foreach (var obj in activeHighlights)
            Destroy(obj);
        activeHighlights.Clear();
    }
}
