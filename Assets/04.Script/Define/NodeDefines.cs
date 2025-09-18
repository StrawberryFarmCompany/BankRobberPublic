using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NodeDefines
{
    public enum Team { Ally, Enemy, Neutral}

    public class Node
    {
        private Vector3Int centerPos;
        //TODO : 캐릭터 
        public Vector3Int GetCenter { get{ return centerPos; } }

        private Interaction NodeInteraction;
        private bool isWalkable;
        public bool IsWalkable { get { return isWalkable; } }

        private bool isSecurityArea;
        public bool IsSecurityArea { get { return isSecurityArea; } set { isSecurityArea = value; } } //나중을 위한 보안 구역

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

    public class CharacterInfo : MonoBehaviour
    {
        public Team team = Team.Ally;
        public EntityData baseStats; // SO (사거리 등 읽기용)

        public int WeaponRangeTiles =>
            Mathf.Max(0, Mathf.RoundToInt(baseStats ? baseStats.attackRange : 0f));

        void OnEnable()
        {
            if (team == Team.Enemy) GameManager.GetInstance.RegisterEnemy(transform);
        }

        void OnDisable()
        {
            if (team == Team.Enemy) GameManager.GetInstance.UnregisterEnemy(transform);
        }
    }

    public delegate void Interaction();
}
