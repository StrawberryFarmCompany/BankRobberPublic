using DG.Tweening;
using NodeDefines;
using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.UIElements;

public class Door : IInteractable
{
    public Vector3Int tile { get; set; }
    public Transform tr;
    public DoorLockType type;
    public ILock lockModule;
    public GameObject block;
    public int index;
    private Vector3 defaultRotation;
    /// <summary>
    /// 
    /// </summary>
    /// <param name="tr">대상 문</param>
    /// <param name="type">도어락 타입</param>
    /// <param name="doorValue">키카드 == 카드 인덱스,락핏 == 문을 따는 최소 밸류</param>

    private bool isOpen;
    private string keyOpen;
    private string keyClose;

    private string doorId;

    /// <summary>
    /// 키카드, 락픽, 일반 문, 패스워드 일 때 초기화
    /// </summary>
    /// <param name="tile"></param>
    /// <param name="tr"></param>
    /// <param name="type"></param>
    /// <param name="doorValue"></param>
    public void Init(Vector3Int tile, Transform tr, DoorLockType type, int doorValue)
    {
        this.tile = tile;
        this.tr = tr;
        this.type = type;

        if(type == DoorLockType.password) index = doorValue;

        lockModule = ILock.Factory(type, doorValue);
        defaultRotation = tr.rotation.eulerAngles;


        //문 인스턴스 식별자(좌표 기반)
        doorId = $"{tile.x},{tile.z}";

        //유니크 키(같은 타입 문 여러 개여도 각각 분리됨)
        keyOpen = $"{lockModule}:{InteractionType.Door}:{doorId}:Open";
        keyClose = $"{lockModule}:{InteractionType.Door}:{doorId}:Close";
        
         isOpen = false;

        RegistInteraction(OnInteraction);
        
    }

    /// <summary>
    /// 버튼식 도어
    /// </summary>
    /// <param name="tile"></param>
    /// <param name="tr"></param>
    /// <param name="type"></param>
    /// <param name="block"></param>
    /// <param name="isrand"></param>
    /// <param name="buttonValue"></param>
    /// <param name="min"></param>
    /// <param name="max"></param>
    public void Init(Vector3Int tile, Transform tr, DoorLockType type, GameObject block, bool isrand, int buttonValue = 0, int min = 0, int max = 0)
    {
        this.tile = tile;
        this.tr = tr;

        this.block = block;
        if (isrand) index = Random.Range(min, max+1);
        else index = buttonValue;

        ////문 인스턴스 식별자(좌표 기반)
        //doorId = $"{tile.x},{tile.z}";

        ////유니크 키(같은 타입 문 여러 개여도 각각 분리됨)
        //keyOpen = $"{lockModule}:{InteractionType.Door}:{doorId}:Open";
        //keyClose = $"{lockModule}:{InteractionType.Door}:{doorId}:Close";

        GameManager.GetInstance.RegisterButtonDoor(this);
    }

    public void OnInteraction(EntityStats stat)
    {
        if (lockModule.IsLock(stat) && !isOpen)
        {
            //이동 가능 불가 여부 추후 추가 필요
            Vector3 targetRot;

            targetRot = defaultRotation + (Vector3.up * 90);
            tr.transform.DORotate(targetRot, 0.7f);/*.OnComplete(RebuildAllNavMeshes);*/

            isOpen = true;

            NodePlayerManager.GetInstance.GetCurrentPlayer().animationController.InteractionState(tr.transform.position);
            GameManager.GetInstance.Nodes[tile].isWalkable = true;

            ReleaseInteraction(OnInteraction);
            RegistInteraction(UnInteraction);

            NodePlayerManager.GetInstance.GetCurrentPlayer().TurnOnHighlighter();
        }
        else if (!lockModule.IsLock(stat) && type == DoorLockType.password)
        {
            UIManager.GetInstance.SetPasswordUI(index, tr);
        }

    }
    public void UnInteraction(EntityStats stat)
    {
        NodePlayerManager.GetInstance.GetCurrentPlayer().animationController.InteractionState(tr.transform.position);
        if (!isOpen) return;
        //이동 가능 불가 여부 추후 추가 필요
        tr.transform.DORotate(defaultRotation, 0.7f)/*.OnComplete(RebuildAllNavMeshes)*/;
        GameManager.GetInstance.Nodes[tile].isWalkable = false;

        isOpen = false;

        ReleaseInteraction(UnInteraction);
        RegistInteraction(OnInteraction);
    }
    public void RegistInteraction(Interaction interaction)
    {
        List<Vector3Int> vecs = GameManager.GetInstance.GetNearNodes(tile);
        string key = (interaction == OnInteraction) ? keyOpen : keyClose;

        for (int i = 0; i < vecs.Count; i++)
        {
            //GameObject.CreatePrimitive(PrimitiveType.Cube).transform.position = vecs[i];//디버그코드
            GameManager.GetInstance.Nodes[vecs[i]].AddInteraction(interaction, key/*OnInteraction, lockModule.ToString() + InteractionType.Door.ToString()*/);
        }
    }
    public void ReleaseInteraction(Interaction interaction)
    {
        List<Vector3Int> vecs = GameManager.GetInstance.GetNearNodes(tile);
        string key = (interaction == OnInteraction) ? keyOpen : keyClose;
        for (int i = 0; i < vecs.Count; i++)
        {
            GameManager.GetInstance.Nodes[vecs[i]].RemoveInteraction(interaction, key/*OnInteraction, lockModule.ToString() + InteractionType.Door.ToString()*/);
        }
    }

/*    private static NavMeshSurface[] _cached;
    private static void RebuildAllNavMeshes()
    {
        if (_cached == null || _cached.Length == 0)
            _cached = Object.FindObjectsOfType<NavMeshSurface>(); // 씬 내 Surface 전부 수집

        for (int i = 0; i < _cached.Length; i++)
        {
            if (_cached[i] != null)
            {
                _cached[i].BuildNavMesh();
            }
        }

        //foreach (var s in _cached)
        //{
        //    if (s != null) s.BuildNavMesh();
        //}
    }*/
}
public enum DoorLockType{none,lockPick,keyCard,button,password}