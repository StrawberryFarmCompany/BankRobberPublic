using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class NodeBoundPreviewer
{
    GameObject obj;
    [SerializeField]MeshFilter meshFilter;
    public bool isActivated { get { return obj.activeSelf; } }
    //GC call 최적화를 위해 클래스 변수로 선언
    private Dictionary<Vector3, int> vertDict;
    Queue<int> triangleQueue;
    Queue<Vector2> uvQueue;
    readonly Vector3[] meshPoints = new Vector3[6]
    {
        new Vector3(0.5f , 0f , -0.5f),
        new Vector3(-0.5f , 0f, -0.5f),
        new Vector3(-0.5f , 0f, 0.5f),

        new Vector3(0.5f , 0f, 0.5f),
        new Vector3(0.5f , 0f, -0.5f),
        new Vector3(-0.5f , 0f, 0.5f)
    };

    public NodeBoundPreviewer()
    {
        obj = new GameObject("NodeBoundPreviewer");
        GameObject.DontDestroyOnLoad(obj);
        obj.transform.position = Vector3.zero;
        obj.transform.eulerAngles = Vector3.zero;
        obj.transform.localScale = Vector3.one;
        meshFilter = obj.AddComponent<MeshFilter>();
        ResourceManager.GetInstance.LoadAsync<Material>("NodePreviewerMat", (mat) => { obj.AddComponent<MeshRenderer>().material = mat; });
        vertDict = new Dictionary<Vector3, int>();
        triangleQueue = new Queue<int>();
        uvQueue = new Queue<Vector2>();
    }
    public void Enable(bool onOff)
    {
        obj.SetActive(onOff);
    }
    public void SetMesh(Vector3Int[] poses)
    {
        Enable(true);
        meshFilter.mesh = null;
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
        meshFilter.mesh = mesh;
        triangleQueue.Clear();
        uvQueue.Clear();
        vertDict.Clear();
    }
    public void SetMesh(Vector3[] poses)
    {
        Enable(true);
        meshFilter.mesh = null;
        //List<Vector3> points = new List<Vector3>();
        Dictionary<Vector3, int> vertDict = new Dictionary<Vector3, int>();
        Queue<int> triangleQueue = new Queue<int>();
        Queue<Vector2> uv = new Queue<Vector2>();
        for (int i = 0; i < poses.Length; i++)
        {
            Vector3[] vert = GetPoints(poses[i]);
            for (int j = 0; j < vert.Length; j++)
            {
                Debug.Log($"정점봐표 수집 {vert[j]}");
                if (vertDict.ContainsKey(vert[j]))
                {
                    triangleQueue.Enqueue(vertDict[vert[j]]);
                }
                else
                {
                    uv.Enqueue(new Vector2(vert[j].x, vert[j].z)+ (Vector2.one / 2f));
                    triangleQueue.Enqueue(vertDict.Count); 
                    vertDict.Add(vert[j], vertDict.Count);
                }
            }
        }

        Mesh mesh = new Mesh();
        mesh.vertices = vertDict.Keys.ToArray();
        mesh.triangles = triangleQueue.ToArray();
        mesh.uv = uv.ToArray();
        meshFilter.mesh = mesh;
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
