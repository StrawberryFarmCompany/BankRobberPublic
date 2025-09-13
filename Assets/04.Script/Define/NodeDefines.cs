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

        private Interaction NodeInteraction;
        public Node(Vector3Int center)
        {
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
