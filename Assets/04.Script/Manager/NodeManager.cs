using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

using static UnityEditor.PlayerSettings;

namespace NodeDefines
{
    class NodeManager : SingleTon<NodeManager>
    {
        private Dictionary<Vector3Int, Node> nodes;
        //현재 팔방, 추후 4방이면 4방으로 바꿔야함
        private readonly Vector3Int[] nearNode = new Vector3Int[8]{Vector3Int.forward,Vector3Int.right,Vector3Int.back,Vector3Int.left, new Vector3Int(-1, 0, -1), new Vector3Int(1, 0, 1), new Vector3Int(-1,0,1), new Vector3Int(1, 0, -1) };
        protected override void Init()
        {
            base.Init();
            nodes = new Dictionary<Vector3Int, Node>();
        }
        protected override void Reset()
        {
            base.Reset();
        }
        public void RegistNode(Vector3Int vec,bool isWalkable)
        {
            nodes.TryAdd(vec, new Node(vec, isWalkable));
        }
        public Node GetNode(Vector3 pos)
        {
            nodes.TryGetValue(new Vector3Int(Mathf.CeilToInt(pos.x), Mathf.CeilToInt(pos.y), Mathf.CeilToInt(pos.z)), out Node result);
            return result;
        }
        public Vector3Int GetVecInt(Vector3 pos)
        {
            return new Vector3Int(Mathf.CeilToInt(pos.x), Mathf.CeilToInt(pos.y), Mathf.CeilToInt(pos.z));
        }
        public bool IsExistNode(Vector3Int vec)
        {
            return nodes.ContainsKey(vec);
        }
        public void RegistEvent(Vector3 pos,Interaction a)
        {
            for (int i = 0; i < nearNode.Length; i++)
            {
                if (nodes.TryGetValue(nearNode[i],out Node node))
                {
                    node.AddInteraction(a);
                }
            }
        }
    }
}
