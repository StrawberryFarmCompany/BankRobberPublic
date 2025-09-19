using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NodeDefines
{
    public class Node
    {
        private Vector3Int centerPos;
        //TODO : 캐릭터 
        public Vector3Int GetCenter { get{ return centerPos; } }

        private Interaction NodeEvent;
        private bool isWalkable;
        public bool IsWalkable { get { return isWalkable; } }

        private bool isSecurityArea;
        public bool IsSecurityArea { get { return isSecurityArea; } set { isSecurityArea = value; } } //나중을 위한 보안 구역

        Dictionary<string, Interaction> nodeInteractions;

        public PlayerStats standingCharactor;

        string[] GetInteractionID { get { return nodeInteractions.Keys.ToArray(); } }

        public Node(Vector3Int center,bool isWalkable)
        {
            this.isWalkable = isWalkable;
            centerPos = center;
        }
        public void RemoveInteraction(Interaction remove, string interactionName)
        {
            nodeInteractions.Remove(interactionName);
            if (nodeInteractions.Count == 0) nodeInteractions = null;
        }
        public void AddInteraction(Interaction add,string interactionName)
        {
            if (nodeInteractions == null) nodeInteractions = new Dictionary<string, Interaction>();
            if (nodeInteractions.TryAdd(interactionName, add))
            {

            }
            else
            {
                nodeInteractions[interactionName] += add;
            }
        }
        public void RemoveEvent(Interaction remove)
        {
            NodeEvent -= remove;
        }
        public void AddEvent(Interaction add)
        {
            NodeEvent += add;
        }

        public void ResetEvent()
        {
            NodeEvent = null;
        }
        public void InvokeEvent()
        {
            NodeEvent.Invoke();
        }
    }
    public delegate void Interaction();
}
