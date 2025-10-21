using System.Collections;
using System.Linq;
using System;
using System.Collections.Generic;
using UnityEngine;

public class NodePreviewer
{
    #region 범위프리뷰어
    GameObject boundPreviewerGOBJ;
    [SerializeField]MeshFilter boundMeshFilter;
    public bool isBoundActivated { get { return boundPreviewerGOBJ.activeSelf; } }
    HashSet<Vector3Int> activatedBounds;

    #endregion

    #region 골 프리뷰어 변수
    Transform goalPreviewer;
    public bool isGoalActivated { get { return goalPreviewer.gameObject.activeSelf; } }
    Vector3Int lastNodePos;
    #endregion

    #region 경로미리보기 변수
    LineRenderer pathLine;
    #endregion

    //GC call 최적화를 위해 클래스 변수로 선언
    private Dictionary<Vector3, int> vertDict;
    Queue<int> triangleQueue;
    Queue<Vector2> uvQueue;
    readonly Vector3[] meshPoints = new Vector3[6]
    {
        new Vector3(0.5f , 0.03f , -0.5f),
        new Vector3(-0.5f , 0.03f, -0.5f),
        new Vector3(-0.5f , 0.03f, 0.5f),

        new Vector3(0.5f , 0.03f, 0.5f),
        new Vector3(0.5f , 0.03f, -0.5f),
        new Vector3(-0.5f , 0.03f, 0.5f)
    };

    public NodePreviewer()
    {
        boundPreviewerGOBJ = new GameObject("NodeBoundPreviewer");
        GameObject.DontDestroyOnLoad(boundPreviewerGOBJ);
        boundPreviewerGOBJ.transform.position = Vector3.zero;
        boundPreviewerGOBJ.transform.eulerAngles = Vector3.zero;
        boundPreviewerGOBJ.transform.localScale = Vector3.one;
        boundMeshFilter = boundPreviewerGOBJ.AddComponent<MeshFilter>();
        ResourceManager.GetInstance.LoadAsync<Material>("NodePreviewerMat", (mat) => { boundPreviewerGOBJ.AddComponent<MeshRenderer>().material = mat; });
        vertDict = new Dictionary<Vector3, int>();
        triangleQueue = new Queue<int>();
        uvQueue = new Queue<Vector2>();


        goalPreviewer = new GameObject("GoalPreviewer").transform;
        GameObject.DontDestroyOnLoad(goalPreviewer);
        goalPreviewer.transform.position = Vector3.zero;
        goalPreviewer.transform.eulerAngles = Vector3.zero;
        goalPreviewer.transform.localScale = Vector3.one;
        ResourceManager.GetInstance.LoadAsync<Material>("NodePreviewerGoalMat", (mat) => { goalPreviewer.gameObject.AddComponent<MeshRenderer>().material = mat; });
        Mesh goalPreviewerMesh = new Mesh();
        activatedBounds = new HashSet<Vector3Int>();

        Vector3[] GPVerts = GetPoints(Vector3.up*0.03f);
        goalPreviewerMesh.vertices = GPVerts;

        int[] triangle = Enumerable.Range(0, 6).ToArray();
        goalPreviewerMesh.triangles = triangle;

        List<Vector2> goalPreviewUV = new List<Vector2>();
        foreach (var item in meshPoints) goalPreviewUV.Add(new Vector2(item.x+0.5f,item.z + 0.5f));

        goalPreviewerMesh.uv = goalPreviewUV.ToArray();
        goalPreviewer.gameObject.AddComponent<MeshFilter>().mesh = goalPreviewerMesh;

        GameObject.DontDestroyOnLoad(goalPreviewer.gameObject);
        pathLine = new GameObject("PathLineRenderer").AddComponent<LineRenderer>();
        pathLine.startWidth = 0.4f;
        pathLine.endWidth = 0.4f;
        pathLine.textureMode = LineTextureMode.Tile;
        pathLine.textureScale = new Vector2(2.2f, 1f);
        ResourceManager.GetInstance.LoadAsync<Material>("PathPreviewerMat", (mat) => { pathLine.material = mat; });


    }
    public void Enable(bool onOff)
    {
        boundPreviewerGOBJ.SetActive(onOff);
        goalPreviewer.gameObject.SetActive(onOff);
    }
    public void SetBoundMesh(Vector3Int[] poses)
    {
        Enable(true);
        boundMeshFilter.mesh = null;
        activatedBounds.Clear();
        //List<Vector3> points = new List<Vector3>();
        for (int i = 0; i < poses.Length; i++)
        {
            Vector3[] vert = GetPoints(poses[i]);
            activatedBounds.Add(poses[i]);
            for (int j = 0; j < vert.Length; j++)
            {
                if (vertDict.ContainsKey(vert[j]))
                {
                    triangleQueue.Enqueue(vertDict[vert[j]]);
                }
                else
                {
                    uvQueue.Enqueue(new Vector2(vert[j].x, vert[j].z)+ (Vector2.one / 2f));
                    triangleQueue.Enqueue(vertDict.Count); 
                    vertDict.Add(vert[j], vertDict.Count);
                }
            }
        }

        Mesh mesh = new Mesh();
        mesh.vertices = vertDict.Keys.ToArray();
        mesh.triangles = triangleQueue.ToArray();
        mesh.uv = uvQueue.ToArray();
        boundMeshFilter.mesh = mesh;
        triangleQueue.Clear();
        uvQueue.Clear();
        vertDict.Clear();
    }
    public void SetGoalPos(Vector3Int pos)
    {
        GoalPreviewOnOff(true);
        goalPreviewer.position = pos;
    }
    public void SetPathLine(Vector3Int[] path)
    {
        pathLine.positionCount = path.Length;
        for (int i = 0; i < path.Length; i++)
        {
            pathLine.SetPosition(i, new Vector3(path[i].x, path[i].y, path[i].z));
        }
        
    }
    public bool IsPosCludeInBound(Vector3Int pos)
    {
        return activatedBounds.Contains(pos);
    }
    public void GoalPreviewOnOff(bool enable)
    {
        if (!enable)
        {
            pathLine.transform.position = new Vector3(9999999f, 9999999f, 999999f);
            goalPreviewer.position = new Vector3(9999999f, 9999999f, 999999f);
        }
        if (goalPreviewer.gameObject.activeSelf != enable)
        {
            goalPreviewer.gameObject.SetActive(enable);
            pathLine.gameObject.SetActive(enable);
        }
    }
    public Vector3[] GetPoints(Vector3 pos)
    {
        Vector3[] points = new Vector3[meshPoints.Length];
        for (int i = 0; i < meshPoints.Length; i++)
        {
            points[i] = pos + meshPoints[i];
        }
        return points;
    }

}
