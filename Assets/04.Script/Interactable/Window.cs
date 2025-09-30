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

        UIManager.GetInstance.StartCoroutine(CoRegisterWhenNodesReady(OnInteraction));
    }

    private IEnumerator CoRegisterWhenNodesReady(Interaction interaction)
    {
        var gm = GameManager.GetInstance;

        var span = (wayOne.x != 0) ? new Vector3Int(0, 0, 1) : new Vector3Int(1, 0, 0);

        var frontA = tile + wayOne;
        var frontB = tile + span + wayOne;
        var backA = tile + wayTwo;
        var backB = tile + span + wayTwo;

        while (!gm.IsExistNode(frontA) || !gm.IsExistNode(frontB) ||
               !gm.IsExistNode(backA) || !gm.IsExistNode(backB))
            yield return null;

        RegistInteraction(interaction);
    }

    public void OnInteraction(EntityStats stat)
    {
        //1이 아닌 방향을 구해야함

        bool isForwardXAxis = (wayOne.x != 0);
        Vector3Int goal;

        if (isForwardXAxis)
        {
            goal = stat.currNode.GetCenter.x == (tile + wayOne).x ? tile + wayTwo : tile + wayOne;
        }
        else
        {
            goal = stat.currNode.GetCenter.z == (tile + wayOne).z ? tile + wayTwo : tile + wayOne;
        }
        //TODO : 플레이어 강제 움직임 함수 받아서 goal넣어줘야함
        var span = (wayOne.x != 0) ? new Vector3Int(0, 0, 1) : new Vector3Int(1, 0, 0);
        var frontA = tile + wayOne;
        var frontB = frontA + span;
        var backA = tile + wayTwo;
        var backB = backA + span;

        var here = stat.currNode.GetCenter;

        //4칸(앞2/뒤2) 중에 서있지 않으면 무시
        if (here != frontA && here != frontB && here != backA && here != backB)
            return;

        //같은 라인 보정
        if (here == frontB && goal == backA) goal = backB;
        else if (here == backB && goal == frontA) goal = frontB;
        else if (here == frontA && goal == backB) goal = backA;
        else if (here == backA && goal == frontB) goal = frontA;

        var gm = GameManager.GetInstance;
        if (!gm.Nodes.TryGetValue(goal, out var goalNode) || !goalNode.isWalkable)
            return;

        //실제 이동 처리
        var prevNode = stat.currNode;
        if (prevNode != null) prevNode.RemoveCharacter(stat);

        stat.currNode = goalNode;
        goalNode.AddCharacter(stat);

        var npm = NodePlayerManager.GetInstance;
        var cur = npm?.GetCurrentPlayer();
        if (cur == null || cur.playerStats != stat) return;

        var pos = cur.transform.position;
        cur.transform.position = new Vector3(goal.x, pos.y, goal.z);
    }
    public void UnInteraction(EntityStats stat)
    {
        
    }
    public void RegistInteraction(Interaction interaction)
    {
        List<Vector3Int> vecs = GameManager.GetInstance.GetNearNodes(tile);
        for (int i = 0; i < vecs.Count; i++)
        {
            GameManager.GetInstance.Nodes[vecs[i]].AddInteraction(OnInteraction,InteractionType.Window.ToString());
        }
    }
    public void ReleaseInteraction(Interaction interaction)
    {
        List<Vector3Int> vecs = GameManager.GetInstance.GetNearNodes(tile);
        for (int i = 0; i < vecs.Count; i++)
        {
            GameManager.GetInstance.Nodes[vecs[i]].RemoveInteraction(OnInteraction,InteractionType.Window.ToString());
        }
    }
}
