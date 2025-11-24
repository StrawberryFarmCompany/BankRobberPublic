using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemySitePreviewer
{
    public MeshRenderer meshRenderer;
    public MeshFilter mesh;
    static Transform sitePreviewerParent;
    public EnemySitePreviewer(GameObject enemyOBj,string name)
    {
        if (sitePreviewerParent == null)
        {
            sitePreviewerParent = new GameObject("EnemySitePreviewers").transform;
        }
        GameObject obj = new GameObject(name);
        obj.transform.parent = sitePreviewerParent;
        meshRenderer = obj.AddComponent<MeshRenderer>();
        mesh = obj.AddComponent<MeshFilter>();
        meshRenderer.material = (Material)ResourceManager.GetInstance.GetPreLoad["EnemySiteMat"];
    }
    public bool CheckObstacle(Vector3 start,Vector3 target,Vector3 dirrection, float distance)
    {
        start += Vector3.up * 1.5f;
        target += Vector3.up * 1.5f;
        if (Physics.Raycast(start, dirrection, out RaycastHit hit, distance))
        {
            // 맞은 오브젝트에 NodePlayerController가 붙어 있다면 플레이어임
            if (hit.collider.TryGetComponent<NodePlayerController>(out NodePlayerController player)) return true;
            else return false;
        }
        return true;
    }
    public Vector3Int[] SearchSite(Vector3Int start,float halfAngle,float yRot)
    {
        Queue<Vector3Int> priorityQueue = new Queue<Vector3Int>();
        Dictionary<Vector3Int, VisitData> visited = new Dictionary<Vector3Int,VisitData>();//방문 여부, 노드 존재여부
        Vector3Int[] nearPoses = GameManager.GetInstance.nearNode;
        visited.Add(start,new VisitData(true,true));
        priorityQueue.Enqueue(start);

        /*while (priorityQueue.Count> 0)
        {
            for (int i = 0; i < nearPoses; i++)
            {

            }
        }*/
        return visited.Where(x=>x.Value.nodeExist).Select(x=>x.Key).ToArray();
    }
}
public struct VisitData
{
    public bool visited;
    public bool nodeExist;
    public VisitData(bool visited,bool nodeExist)
    {
        this.visited = visited;
        this.nodeExist = nodeExist;
    }
}
