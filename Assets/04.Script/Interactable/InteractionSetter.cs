using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class InteractionSetter : MonoBehaviour
{
    [SerializeField]InteractionType type;
    IInteractable interaction;
    [SerializeField] bool isWalkAble = false;
    private GameManager Manager {get{return GameManager.GetInstance; } }

    [ConditionalHide("type", (int)InteractionType.BankVault, (int)InteractionType.Door)]//금고문,문
    public Transform target;
    void Start()
    {
        interaction = IInteractable.Factory(type);
        Vector3Int pos = Manager.GetVecInt(transform.position);
        Vector3Int[] nearPos = Manager.GetNearNodes(pos).ToArray();
        if (!Manager.Nodes.ContainsKey(pos)) Manager.RegistNode(pos, isWalkAble);
        interaction.tile = pos;
        GameManager.GetInstance.RegistEvent(nearPos, interaction.OnInteraction , type.ToString());
    }
}