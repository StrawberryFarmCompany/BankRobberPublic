using UnityEngine;

public enum SpecialNodeType
{
    Security,
    Interactable
}

public class SpecialNode : MonoBehaviour
{
    public SpecialNodeType type;
    public Vector3Int gridPos;

    private void Start()
    {
        gridPos = GameManager.GetInstance.GetVecInt(transform.position);
        SpecialNodeManager.GetInstance.RegisterNode(gridPos, type);
    }
}
