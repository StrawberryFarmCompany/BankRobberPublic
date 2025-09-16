using NodeDefines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeObject : MonoBehaviour
{
    [SerializeField] NodeType nodeType;
    private void Awake()
    {
        GameManager.GetInstance.RegistNode(GameManager.GetInstance.GetVecInt(transform.position), nodeType == NodeType.wall? false : true);
        Destroy(this);
    }
}
public enum NodeType { floor,wall}