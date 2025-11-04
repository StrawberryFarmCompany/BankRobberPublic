using DG.Tweening;
using NodeDefines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Window : IInteractable
{
    public Vector3Int tile { get; set; }
    private Vector3Int wayOne;
    private Vector3Int wayTwo;
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="tr">대상 문</param>
    /// <param name="type">도어락 타입</param>
    /// <param name="doorValue">키카드 == 카드 인덱스,락핏 == 문을 따는 최소 밸류</param>
    public void Init(Vector3Int tile,Vector3Int forward)
    {
        this.tile = tile;
        wayOne = forward;
        wayTwo = forward * -1;
        RegistInteraction(OnInteraction);
        Debug.LogError(forward);
        Debug.LogError(tile);
    }

    public void OnInteraction(EntityStats stat)
    {
        //1이 아닌 방향을 구해야함

        bool isForwardXAxis = Mathf.Abs(wayOne.x) > 0;  /*wayOne.x == 1;*/
        Vector3Int goal = Vector3Int.zero;
        
        if (isForwardXAxis)
        {
            goal = stat.currNode.GetCenter.x == (tile + wayOne).x ? tile + wayTwo : tile + wayOne;
        }
        else
        {
            goal = stat.currNode.GetCenter.z == (tile + wayOne).z ? tile + wayTwo : tile + wayOne;
        }
        //TODO : 플레이어 강제 움직임 함수 받아서 goal넣어줘야함
        Debug.LogError(goal);
        Debug.LogError(tile);
        stat.ForceMove?.Invoke(goal);
    }
    public void UnInteraction(EntityStats stat)
    {
        
    }
    public void RegistInteraction(Interaction interaction)
    {
        //GameObject.CreatePrimitive(PrimitiveType.Cube).transform.position = tile + wayOne;//디버그용코드
        GameManager.GetInstance.Nodes[tile + wayOne].AddInteraction(OnInteraction, InteractionType.Window.ToString());
        GameManager.GetInstance.Nodes[tile + wayTwo].AddInteraction(OnInteraction, InteractionType.Window.ToString());
    }
    public void ReleaseInteraction(Interaction interaction)
    {
        GameManager.GetInstance.Nodes[tile+wayOne].RemoveInteraction(OnInteraction, InteractionType.Window.ToString());
        GameManager.GetInstance.Nodes[tile + wayTwo].RemoveInteraction(OnInteraction, InteractionType.Window.ToString());
    }
}
