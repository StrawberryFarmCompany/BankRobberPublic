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

    [ConditionalHide("type", (int)InteractionType.Door, (int)InteractionType.KeyCard)]//금고문,문 카드키
    public Transform target;
    [ConditionalHide("type", (int)InteractionType.Door, (int)InteractionType.KeyCard)]//금고문,문 카드키
    public int doorValue;
    [ConditionalHide("type", (int)InteractionType.Door)]//금고문,문
    public DoorLockType lockType;
    [ConditionalHide("type", (int)InteractionType.GoldBar)]//골드바, 현금
    public GameObject[] consumeItems;
    void Start()
    {
        interaction = IInteractable.Factory(type);
        Vector3Int pos = Manager.GetVecInt(transform.localPosition);
        if (type == InteractionType.Door)
        {
            pos = Manager.GetVecInt(target.localPosition + (target.forward/2f)+ (transform.right / -2f));
        }

        if (!Manager.Nodes.ContainsKey(pos)) Manager.RegistNode(pos, isWalkAble);
        else
        {
            Manager.Nodes[pos].isWalkable = isWalkAble;
        }
        interaction.tile = pos;
        switch (type)
        {
            case InteractionType.AlamBTN:
                break;
            case InteractionType.Window:
                Window window = (Window)interaction;
                Vector3Int forward = Manager.GetVecInt(transform.forward);
                window.Init(pos,forward-pos);
                break;
            case InteractionType.Door:
                Door door = (Door)interaction;
                door.Init(pos,target, lockType, doorValue);
                break;
            case InteractionType.GoldBar:
                GoldBar gold = (GoldBar)interaction;
                gold.Init(pos, consumeItems);
                break;
            case InteractionType.MoneyBag:
                break;
            case InteractionType.KeyCard:
                KeyCard keyCard = (KeyCard)interaction;
                keyCard.Init(pos,target, doorValue);
                break;
            default:
                break;
        }
        Destroy(this);
        target = null;
    }
}