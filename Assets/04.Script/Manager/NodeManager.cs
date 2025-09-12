using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace NodeDefines
{
    class NodeManager : SingleTon<NodeManager>
    {
        private Dictionary<Vector3Int, Node> nodes;
        protected override void Init()
        {
            base.Init();
            nodes = new Dictionary<Vector3Int, Node>();
        }
        protected override void Reset()
        {
            base.Reset();
        }
        public void RegistNode(Vector3Int vec)
        {
            nodes.TryAdd(vec, new Node(vec));
        }
        public Node GetNode(Vector3 pos)
        {
            return nodes[new Vector3Int(Mathf.CeilToInt(pos.x), Mathf.CeilToInt(pos.y), Mathf.CeilToInt(pos.z))];
        }



    }
}
