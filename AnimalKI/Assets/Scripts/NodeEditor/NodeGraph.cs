using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace NodeSystem
{
    public class NodeGraph : ScriptableObject
    {
        public string Name = "defaultName";
        public List<Node> nodes = new List<Node>();        
    }
}
