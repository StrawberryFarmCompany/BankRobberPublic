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
        public Action Interaction;
        public Node(Vector3Int center)
        {
            centerPos = center;
        }
        public void ResetInteraction()
        {
            Interaction = null;
        }
    }
}
