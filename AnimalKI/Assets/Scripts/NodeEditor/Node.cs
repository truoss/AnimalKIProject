using System;
using System.Collections.Generic;
using UnityEngine;

namespace NodeSystem
{
    public abstract class Node : ScriptableObject
    {
        public Rect rect;
        internal Vector2 contentOffset = Vector2.zero;

        public List<SocketIn> Inputs = new List<SocketIn>();
        public List<SocketOut> Outputs = new List<SocketOut>();

        [NonSerialized]
        internal bool calculated = true;

        public abstract string GetID { get; }

        public abstract Node Create(Vector2 pos);

        public abstract void NodeGUI();

        public abstract bool Perform();

        protected internal virtual void OnDelete() { }

        public virtual void DrawNode()
        {
            //Debug.LogWarning("DrawNode: " + rect);
            //var width = GUIx.GUIContentSize(GUIx.I.skin.GetStyle("Label"), new GUIContent(GetID)).x;
            //Debug.LogWarning();
            rect = new Rect(rect.x, rect.y, 130, (Inputs.Count + Outputs.Count) * 10 + 30+50);
            //nodeRect.position += NodeEditor.curEditorState.zoomPanAdjust;
            //contentOffset = new Vector2(0, 20);            

            GUI.BeginGroup(rect);
            if(NodeEditor.IsHoveredNode(this) || NodeEditor.IsSelectedNode(this))
                GUI.color = GUIx.I.nodeHighlightColor;
            else
                GUI.color = GUIx.I.nodeColor;
            GUI.Label(new Rect(0, 0, rect.width, rect.height), GUIx.empty, GUIx.I.nodeStyle);
            GUI.color = Color.white;

            GUI.changed = false;      
            
            GUILayout.BeginArea(new Rect(0, 0, rect.width, rect.height));

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(name);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(4);
            
            NodeGUI();

            GUILayout.EndArea();
            GUI.EndGroup();            
        }

        protected internal bool allInputsReady()
        {
            for (int inCnt = 0; inCnt < Inputs.Count; inCnt++)
            {
                if (Inputs[inCnt] == null || Inputs[inCnt].IsValueNull)
                    return false;
            }
            return true;
        }
    }
    
    [Serializable]
    public class GUITestNode : Node
    {
        public const string ID = "GUITestNode";
        public override string GetID { get { return ID; } }

        public override Node Create(Vector2 pos)
        {
            //GUITestNode node = CreateInstance<GUITestNode>();

            Inputs.Add((SocketIn)CreateInstance<SocketIn>().Create(this, new TypeData(new FloatType())));
            Inputs.Add((SocketIn)CreateInstance<SocketIn>().Create(this, new TypeData(new FloatType())));

            Outputs.Add((SocketOut)CreateInstance<SocketOut>().Create(this, new TypeData(new FloatType())));
            Outputs.Add((SocketOut)CreateInstance<SocketOut>().Create(this, new TypeData(new FloatType())));

            rect = new Rect(pos.x, pos.y, 120, (Inputs.Count + Outputs.Count) * 10 + 10);
            name = "GUITestNode";
            //Debug.LogWarning(rect);

            return this;
        }

        public override void NodeGUI()
        {
            for (int i = 0; i < Inputs.Count; i++)
            {
                GUILayout.BeginHorizontal();
                if (NodeEditor.IsHoveredSocket(Inputs[i]) || NodeEditor.IsSelectedSocket(Inputs[i]))
                    GUI.color = Color.yellow * 1.3f;
                else
                    GUI.color = Color.yellow; //Inputs[i].typeData.declaration.col;
                GUILayout.Label(GUIx.empty, GUIx.I.socketStyle);
                if (Event.current.type == EventType.Repaint)
                    Inputs[i].rect = GUILayoutUtility.GetLastRect();
                GUI.color = Color.white;

                GUILayout.Label("In"); //Inputs[i].typeData.declaration.Name);
                
                GUILayout.FlexibleSpace();

                GUILayout.EndHorizontal();
            }


            GUILayout.Space(10);

            
            for (int i = 0; i < Outputs.Count; i++)
            {
                GUILayout.BeginHorizontal();
                
                GUILayout.FlexibleSpace();

                GUILayout.Label("Out"); //Inputs[i].typeData.declaration.Name);

                if (NodeEditor.IsHoveredSocket(Outputs[i]) || NodeEditor.IsSelectedSocket(Outputs[i]))
                    GUI.color = Color.yellow * 1.3f;
                else
                    GUI.color = Color.yellow; //Inputs[i].typeData.declaration.col;
                GUILayout.Label(GUIx.empty, GUIx.I.socketStyle);
                if (Event.current.type == EventType.Repaint)
                    Outputs[i].rect = GUILayoutUtility.GetLastRect();
                GUI.color = Color.white;

                GUILayout.EndHorizontal();
            }
        }

        public override bool Perform()
        {
            if (!allInputsReady())
                return false;
            Outputs[0].SetValue<float>(Inputs[0].GetValue<float>() * 5); //TODO GET SET
            Outputs[1].SetValue<float>(Inputs[1].GetValue<float>() * 10);
            return true;
        }
    }

    /*
    public class InputNode : Node
    {
        public const string ID = "InputNode";
        public override string GetID { get { return ID; } }

        public override Node Create(Vector2 pos)
        {
            throw new NotImplementedException();
        }

        public override void NodeGUI()
        {
            throw new NotImplementedException();
        }

        public override bool Update()
        {
            throw new NotImplementedException();
        }
    }

    public class OutputNode : Node
    {
        public const string ID = "OutputNode";
        public override string GetID { get { return ID; } }

        public override Node Create(Vector2 pos)
        {
            throw new NotImplementedException();
        }

        public override void NodeGUI()
        {
            throw new NotImplementedException();
        }

        public override bool Update()
        {
            throw new NotImplementedException();
        }
    }
    */
}
