using NodeDefines;
using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering.PostProcessing;
public class NodeBaker : MonoBehaviour
{
    [SerializeField]NavMeshSurface surface;
    HashSet<Vector3Int> vectors = new HashSet<Vector3Int>();
    private void Awake()
    {
        if (vectors.Count == 0) OnBake();
        RegistNodes();
    }

    private void RegistNodes()
    {
        foreach (Vector3Int v in vectors)
        {
            GameManager.GetInstance.RegistNode(v,true);
        }
        vectors = null;
        Destroy(this);
    }

    public void OnBake()
    {
        vectors = null;
        vectors = new HashSet<Vector3Int>();
        var triangulation = NavMesh.CalculateTriangulation();

        Vector3 min = triangulation.vertices[0];
        Vector3 max = triangulation.vertices[0];

        foreach (Vector3 v in triangulation.vertices)
        {
            min = Vector3.Min(min, v);
            max = Vector3.Max(max, v);
        }
        float tileSize = 1f;

        Debug.Log($"NavMesh Bounds: Min {min}, Max {max}");

        int sizeX = Mathf.CeilToInt((max.x - min.x) / tileSize); //반올림피자
        int sizeY = Mathf.CeilToInt((max.y - min.y) / tileSize);
        int sizeZ = Mathf.CeilToInt((max.z - min.z) / tileSize);

        bool[,,] walkable = new bool[sizeX, sizeY, sizeZ];

        for (int x = 0; x < sizeX; x++)
        {
            for (int y = 0; y < sizeY; y++)
            {
                for (int z = 0; z < sizeZ; z++)
                {
                    Vector3 worldPos = new Vector3(min.x + x * tileSize + tileSize * 0.5f, min.y + y * tileSize + tileSize * 0.5f, min.z + z * tileSize + tileSize * 0.5f);

                    // NavMesh 위에 있는지 검사
                    if (NavMesh.SamplePosition(worldPos, out NavMeshHit hit, 0.5f, NavMesh.AllAreas))
                    {
                        walkable[x, y, z] = true;
                        vectors.Add(new Vector3Int(Mathf.CeilToInt(worldPos.x), Mathf.CeilToInt(worldPos.y), Mathf.CeilToInt(worldPos.z)));
                    }
                    else
                    {
                        walkable[x, y, z] = false;
                        vectors.Remove(new Vector3Int(Mathf.CeilToInt(worldPos.x), Mathf.CeilToInt(worldPos.y), Mathf.CeilToInt(worldPos.z)));
                    }
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        foreach (Vector3Int item in vectors)
        {
            Gizmos.DrawCube(item, Vector3.one);
        }

    }
#endif

}
#if UNITY_EDITOR
[CustomEditor(typeof(NodeBaker))]
public class NodeBakerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        NodeBaker baker = (NodeBaker)target;
        if (GUILayout.Button("baking"))
        {
            baker.OnBake();
        }
    }
}
#endif
