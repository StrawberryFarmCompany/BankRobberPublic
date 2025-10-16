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

    [ConditionalHide("type", (int)InteractionType.Door, (int)InteractionType.KeyCard, (int)InteractionType.GoldBar,(int)InteractionType.VaultDoor)]//금고문,문 카드키
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
            pos = Manager.GetVecInt(transform.localPosition + (transform.forward/2f)+ (transform.right / -2f));
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
                Vector3Int forward = Manager.GetVecInt(transform.right);
                window.Init(pos,forward);
                break;
            case InteractionType.Door:
                Door door = (Door)interaction;
                door.Init(pos,target, lockType, doorValue);
                break;
            case InteractionType.GoldBar:
                GoldBar gold = (GoldBar)interaction;
                gold.Init(pos, target, consumeItems);
                break;
            case InteractionType.MoneyBag:
                break;
            case InteractionType.KeyCard:
                KeyCard keyCard = (KeyCard)interaction;
                keyCard.Init(pos,target, doorValue);
                break;
            case InteractionType.VaultDoor:
                VaultDoor vaultDoor = (VaultDoor)interaction;
                Vector3Int[] doorPoints = new Vector3Int[2];
                for (int i = 1; i <= 4; i++)
                {
                    Vector3Int currPos = pos + (GameManager.GetInstance.GetVecInt(-transform.right * i));
                    GameManager.GetInstance.Nodes[currPos].isWalkable = i == 2 || i == 3 ? true : false;
                    if (i == 2 || i == 3)
                    {
                        GameManager.GetInstance.Nodes[currPos].isWalkable = true;
                        doorPoints[i - 2] = currPos;
                    }
                    else
                    {
                        GameManager.GetInstance.Nodes[currPos].isWalkable = false;
                    }
                }
                vaultDoor.Init(doorPoints,target,doorValue);
                break;
            case InteractionType.EscapeCar:
                EscapeCar car = (EscapeCar)interaction;
                car.Init(pos);
                break;
            default:
                break;
        }
        Destroy(this);
        target = null;
    }
}