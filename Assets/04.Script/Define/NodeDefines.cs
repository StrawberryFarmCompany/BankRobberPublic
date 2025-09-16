using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NodeDefines
{
    public class Node
    {
        private Vector3Int centerPos;
        //TODO : 캐릭터 
        public Vector3Int GetCenter { get{ return centerPos; } }

        private Interaction NodeInteraction;
        private bool isWalkable;
        public bool IsWalkable { get { return isWalkable; } }
        public Node(Vector3Int center,bool isWalkable)
        {
            this.isWalkable = isWalkable;
            centerPos = center;
        }
        public void RemoveInteraction(Interaction remove)
        {
            NodeInteraction -= remove;
        }
        public void AddInteraction(Interaction add)
        {
            NodeInteraction += add;
        }

        public void ResetInteraction()
        {
            NodeInteraction = null;
        }
        public void InvokeInteraction()
        {
            NodeInteraction.Invoke();
        }
    }
    public delegate void Interaction();
}
