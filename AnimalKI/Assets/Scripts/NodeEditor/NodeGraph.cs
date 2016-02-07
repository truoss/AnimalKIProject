using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace NodeSystem
{
    public class NodeGraph : ScriptableObject
    {
        public string Name = "defaultName";
        public Vector2 scrollPos = new Vector2(NodeEditor.canvasSize.x*0.5f, NodeEditor.canvasSize.y*0.5f);
        public List<Node> nodes = new List<Node>();
    }
}
