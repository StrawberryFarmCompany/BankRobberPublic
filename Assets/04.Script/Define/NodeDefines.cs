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
        public bool isWalkable;

        private bool isSecurityArea;
        public bool IsSecurityArea { get { return isSecurityArea; } set { isSecurityArea = value; } } //나중을 위한 보안 구역

        Dictionary<string, Interaction> nodeInteractions;
        private List<EntityStats> standing;
        public List<EntityStats> Standing { get { return standing; } }

        public Node(Vector3Int center,bool isWalkable)
        {
            this.isWalkable = isWalkable;
            standing = new List<EntityStats>();
            centerPos = center;
        }

        public bool HasAnyInteraction()
        {
            return (NodeEvent != null) || (nodeInteractions != null && nodeInteractions.Count > 0);
        }

        public string GetPrimaryImageKey()
        {
            if (nodeInteractions != null && nodeInteractions.Count > 0)
            {
                foreach (var k in nodeInteractions.Keys) return k;
            }
            return null;
        }

        public string GetInteractionNames()
        {
            return string.Join(',', nodeInteractions.Keys);
        }
        public string[] GetInteractionNameArray()
        {
            return nodeInteractions == null ? null : nodeInteractions.Keys.ToArray();
        }


        public void AddCharacter(EntityStats stat)
        {
            standing.Add(stat);
            NodeEvent?.Invoke(stat);
        }
        public void RemoveCharacter(EntityStats stat)
        {
            standing.Remove(stat);
            if (standing.Count == 0) standing = new List<EntityStats>();//더블링 해소
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
            for (int i = 0; i < standing.Count; i++)
            {
                add(standing[i]);
            }
        }

        public void ResetEvent()
        {
            NodeEvent = null;
        }
        public void ResetInteraction()
        {
            string[] keys = nodeInteractions.Keys.ToArray();
            for (int i = 0; i < keys.Length; i++)
            {
                nodeInteractions[keys[i]] = null;
            }
            nodeInteractions.Clear();
        }
        public void InvokeEvent(EntityStats stat)
        {
            NodeEvent.Invoke(stat);
        }

        //노드에 등록된 상호작용 키 열거
        public IEnumerable<string> EnumerateInteractionKeys()
        {
            if (nodeInteractions == null) yield break;
            foreach (var k in nodeInteractions.Keys) yield return k;
        }

        //특정 키의 상호작용 실행
        public bool TryInvokeInteraction(string key, EntityStats actor)
        {
            if (nodeInteractions == null) return false;
            if (!nodeInteractions.TryGetValue(key, out var del) || del == null) return false;
            del.Invoke(actor);
            return true;
        }

        //매칭
        public bool HasInteractionKey(string key)
        {
            if (nodeInteractions == null) return false;
            return nodeInteractions.ContainsKey(key);
        }

        //키 포함 여부
        public bool HasInteractionKeyLike(string part)
        {
            if (nodeInteractions == null) return false;
            foreach (var k in nodeInteractions.Keys)
                if (k.IndexOf(part, StringComparison.OrdinalIgnoreCase) >= 0) return true;
            return false;
        }
    }
    public delegate void Interaction(EntityStats stat);
}
