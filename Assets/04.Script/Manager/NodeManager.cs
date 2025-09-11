using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace NodeDefine
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



    }
}
