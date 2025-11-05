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

    [ConditionalHide("type", (int)InteractionType.Door, (int)InteractionType.KeyCard, (int)InteractionType.GoldBar,(int)InteractionType.VaultDoor, (int)InteractionType.AlarmBTN)]//금고문,문 카드키
    public Transform target;
    [ConditionalHide("type", (int)InteractionType.Document)]//문서
    public int docsValue;
    [ConditionalHide("type", (int)InteractionType.Document)]//문서
    public bool isFirstTwoDigit;
    [ConditionalHide("type", (int)InteractionType.Document)]//문서
    public DocumentType docsType;
    [ConditionalHide("type", (int)InteractionType.Door, (int)InteractionType.KeyCard, (int)InteractionType.VaultDoor)]//금고문,문 카드키
    public int doorValue;
    [ConditionalHide("type", (int)InteractionType.Door)]//금고문,문
    public DoorLockType lockType;
    [ConditionalHide("lockType", (int)DoorLockType.button)]//버튼식 문
    public bool isRandomValue;
    [ConditionalHide("lockType", (int)DoorLockType.button)]//버튼식 문
    public GameObject block;
    [ConditionalHide("isRandomValue", false)]//고정 값
    public int buttonValue;
    [ConditionalHide("isRandomValue", true)]//랜덤 카운트 최소
    public int minValue;
    [ConditionalHide("isRandomValue", true)]//랜덤 카운트 최대
    public int maxValue;

    [ConditionalHide("type", (int)InteractionType.GoldBar)]//골드바, 현금
    public GameObject[] consumeItems;
    void Start()
    {
        interaction = IInteractable.Factory(type);
        Vector3Int pos;
        if (type == InteractionType.Door)
        {
            pos = Manager.GetVecInt(transform.position + /*(transform.forward/2f)+ */(transform.right / -2f));
        }
        else if(type == InteractionType.Window)
        {
            pos = Manager.GetVecInt(transform.position + /*(transform.forward/2f)+ */(transform.forward / 2f));
        }
        else
        {
            pos = Manager.GetVecInt(transform.position); 
        }

        if (!Manager.Nodes.ContainsKey(pos)) Manager.RegistNode(pos, isWalkAble);
        else
        {
            Manager.Nodes[pos].isWalkable = isWalkAble;
        }
        interaction.tile = pos;
        switch (type)
        {
            case InteractionType.AlarmBTN:
                AlarmButton alarmButton = (AlarmButton)interaction;
                alarmButton.Init(pos, target, buttonValue);
                break;
            case InteractionType.Window:
                Window window = (Window)interaction;
                Vector3Int forward = Manager.GetVecInt(transform.right);
                window.Init(pos,forward);
                break;
            case InteractionType.Door:
                Door door = (Door)interaction;
                if (lockType == DoorLockType.button)
                    door.Init(pos, target, lockType, block, isRandomValue, buttonValue, minValue, maxValue);
                else
                    door.Init(pos, target, lockType, doorValue);
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
                    if (!GameManager.GetInstance.Nodes.ContainsKey(currPos))
                    {
                        GameManager.GetInstance.RegistNode(currPos,false);
                    }
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
            case InteractionType.Document:
                Document document = (Document)interaction;
                document.Init(pos, docsValue, docsType, isFirstTwoDigit);
                break;
            default:
                break;
        }
        Destroy(this);
        target = null;
    }
}