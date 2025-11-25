using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using FOW;

public class EnemySitePreviewer
{
    private MeshRenderer meshRenderer;
    private MeshFilter mesh;
    private static Transform sitePreviewerParent;
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

        if (enemyOBj.TryGetComponent<HiderDisableObjects>(out HiderDisableObjects hider))
        {
            hider.ModifyHiddenObjects(new GameObject[]{ obj});
        }
        else
        {
            Debug.LogError($"{enemyOBj.name}에 hiderObject가 없음 (FOW)");
        }
    }
    private bool CheckObstacle(Vector3 start,Vector3 target)
    {
        start += Vector3.up * 1.5f;
        target += Vector3.up * 1.5f;
        Vector3 dirrection = new Vector3(target.x,0f, target.z) - new Vector3(start.x, 0f, start.z);
        
        if (Physics.Raycast(start, dirrection, out RaycastHit hit, Vector3.Distance(start, target)))
        {
            // 맞은 오브젝트에 NodePlayerController가 붙어 있다면 플레이어임
            if (hit.collider.TryGetComponent<NodePlayerController>(out NodePlayerController player)) return true;
            else return false;
        }
        return true;
    }
    public void SetMesh(Vector3Int start, float halfAngle, float yRot,float dist)
    {
        mesh.mesh = null;
        HashSet<Vector3Int> vec = new HashSet<Vector3Int>();
        NodePreviewer.SetBoundMesh(SearchSite(start, halfAngle, yRot, dist), mesh, vec);
        vec.Clear();
    }
    private Vector3Int[] SearchSite(Vector3Int start,float halfAngle,float yRot,float dist)
    {
        Queue<Vector3Int> priorityQueue = new Queue<Vector3Int>();
        Dictionary<Vector3Int, bool> visited = new Dictionary<Vector3Int,bool>();//방문 여부 == 키 존재여부, 노드 존재여부 = bool
        dist *= dist;
        Vector3Int[] nearPoses = GameManager.GetInstance.nearNode;
        visited.Add(start,true);
        priorityQueue.Enqueue(start);
        bool overRotation = yRot - halfAngle <0f;
        yRot += 720f;
        float angleMax = Mathf.Max((yRot + halfAngle) % 360f, (yRot - halfAngle) % 360f);
        float angleMin = Mathf.Min((yRot + halfAngle) % 360f, (yRot - halfAngle) % 360f);


        while (priorityQueue.Count > 0)
        {
            //2중첩 시 진입금지
            Vector3Int currPos = priorityQueue.Dequeue();
            bool currData = visited[currPos];
            for (int i = 0; i < nearPoses.Length; i++)
            {
                Vector3Int nextPos = nearPoses[i] + currPos;
                bool nextNodeExist = GameManager.GetInstance.GetNode(nextPos) != null;


                if (!currData && !nextNodeExist) continue;//2스텍시 루프 끊음
                if (visited.ContainsKey(nextPos)) continue;

                Vector3 relDist = nextPos - start;

                float nodeDist = Mathf.Pow(relDist.x,2f) + Mathf.Pow(relDist.z, 2f);

                if (nodeDist > dist) continue;
                float targetAngle = Mathf.Atan2(relDist.x, relDist.z);
                targetAngle = Mathf.Rad2Deg * targetAngle;
                targetAngle += 360f;
                targetAngle %= 360f;

                if (!overRotation)
                {
                    if (angleMax >= targetAngle && angleMin <= targetAngle)
                    {
                        if (CheckObstacle(start, nextPos))
                        {
                            visited.Add(nextPos, nextNodeExist);
                            priorityQueue.Enqueue(nextPos);
                        }
                    }
                    else continue;
                }
                else
                {
                    if (angleMax <= targetAngle || angleMin >= targetAngle)
                    {
                        if (CheckObstacle(start, nextPos))
                        {
                            visited.Add(nextPos, nextNodeExist);
                            priorityQueue.Enqueue(nextPos);
                        }
                    }
                    else continue;
                }
            }
        }
        return visited.Where(x=>x.Value).Select(x=>x.Key).ToArray();
    }
}
