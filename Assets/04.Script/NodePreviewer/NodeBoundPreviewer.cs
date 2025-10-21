using System.Collections;
using System.Linq;
using System;
using System.Collections.Generic;
using UnityEngine;

public class NodeBoundPreviewer
{
    #region 범위프리뷰어
    GameObject boundPreviewerGOBJ;
    [SerializeField]MeshFilter boundMeshFilter;
    public bool isBoundActivated { get { return boundPreviewerGOBJ.activeSelf; } }
    #endregion

    #region 골프리뷰어
    Transform goalPreviewer;
    public bool isGoalActivated { get { return goalPreviewer.gameObject.activeSelf; } }

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

    public NodeBoundPreviewer()
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

        Vector3[] GPVerts = GetPoints(Vector3.up*0.03f);
        goalPreviewerMesh.vertices = GPVerts;

        int[] triangle = Enumerable.Range(0, 6).ToArray();
        goalPreviewerMesh.triangles = triangle;

        List<Vector2> goalPreviewUV = new List<Vector2>();
        foreach (var item in meshPoints) goalPreviewUV.Add(new Vector2(item.x+0.5f,item.z + 0.5f));

        goalPreviewerMesh.uv = goalPreviewUV.ToArray();
        goalPreviewer.gameObject.AddComponent<MeshFilter>().mesh = goalPreviewerMesh;


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
        //List<Vector3> points = new List<Vector3>();
        for (int i = 0; i < poses.Length; i++)
        {
            Vector3[] vert = GetPoints(poses[i]);
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
    public void SetGoalPos(Vector3 pos)
    {
        goalPreviewer.position = pos;
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
