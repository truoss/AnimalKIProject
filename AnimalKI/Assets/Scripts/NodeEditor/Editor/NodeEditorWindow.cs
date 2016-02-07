using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace NodeSystem
{
    public class NodeEditorWindow : EditorWindow
    {
        public NodeGraph Graph;
        public static NodeEditorWindow editorWindow;

        Rect sideWindowRect;
        public static string StatusMsg;// = "nothing loaded!";

        [MenuItem("Window/Node Editor")]
        static void ShowEditor()
        {
#if UNITY_EDITOR            
            if(editorWindow)
                editorWindow.Close();

            editorWindow = GetWindow<NodeEditorWindow>();
            //editorWindow.position = new Rect(100,100,512,512);
            //editorWindow.minSize = new Vector2(512,512);
            //Debug.LogWarning("ShowEditor: " + editorWindow.position);
#endif            
        }

        NodeGraph MakeTestGraph()
        {
            Debug.LogWarning("MakeTestGraph");
            var graph = CreateInstance<NodeGraph>();

            var node = CreateInstance<GUITestNode>();
            node.Create(new Vector2(45, 30));

            graph.nodes.Add(node);

            return graph;
        }


        void OnGUI()
        {
            if(editorWindow)
                editorWindow.minSize = new Vector2(512, 512);

            if (Graph == null)// && Input.GetMouseButtonDown(1))
            {
                //Graph = MakeTestGraph();
            }

            if (Graph)
            {
                GUI.skin = GUIx.I.skin;
                Event e = Event.current;
                              

                try
                {
                    if (NodeEditor.UpdateInput(Graph, Event.current))//mainNodeCanvas, mainEditorState
                        Repaint();
                }
                catch (UnityException ex)
                {
                    // on exceptions in drawing flush the canvas to avoid locking the ui.              
                    //NewNodeCanvas();
                    //Debug.LogError("Unloaded Canvas due to exception when drawing!");
                    Debug.LogError(ex);
                    //editorWindow.Close();
                }
                

                //Create nodes
                if (e.button == 1 && e.type == EventType.MouseDown && !NodeEditor.IsDragging)
                {
                    //Debug.LogWarning("Create");

                    GenericMenu menu = new GenericMenu();

                    menu.AddItem(new GUIContent("Add Test Node"), false, ContextCallback, "GUITestNode");
                    //menu.AddItem(new GUIContent("Add Output Node"), false, ContextCallback, "outputNode");
                    //menu.AddItem(new GUIContent("Add Calculation Node"), false, ContextCallback, "calcNode");
                    //menu.AddItem(new GUIContent("Add Comparison Node"), false, ContextCallback, "compNode");

                    menu.ShowAsContext();
                }

                //check if select output socket
                if (e.button == 0 && e.type == EventType.MouseDown && !NodeEditor.IsDragging)
                {
                    var node = NodeEditor.GetNodeFromMousePos(NodeEditor.mousePos, Graph.nodes);
                    if (node)
                    {
                        var socket = NodeEditor.GetSocketFromMousePos(node, NodeEditor.mousePos);
                        if (socket && socket is SocketOut)
                        {
                            Debug.LogWarning("socket: " + socket);
                            NodeEditor.CreateConnectionMode = true;
                            NodeEditor.startSocket = socket as SocketOut;
                            NodeEditor.tentativeConnection = CreateInstance<Connection>();
                            NodeEditor.tentativeConnection.startSocket = NodeEditor.startSocket;
                            e.Use();
                        }
                        //else
                            //Debug.LogWarning("no socket clicked!");
                    }
                    //else
                        //Debug.LogWarning("no node clicked!");
                }

                if (e.button == 0 && e.type == EventType.MouseUp && NodeEditor.CreateConnectionMode && !NodeEditor.IsDragging)
                {
                    var node = NodeEditor.GetNodeFromMousePos(NodeEditor.mousePos, Graph.nodes);
                    if (node)
                    {
                        var socket = NodeEditor.GetSocketFromMousePos(node, NodeEditor.mousePos);
                        if (socket && socket is SocketIn
                            && socket.parent != NodeEditor.startSocket.parent
                            && !NodeEditor.IsConnectionLoop(socket as SocketIn, NodeEditor.tentativeConnection))
                        {
                            Debug.LogWarning("socket: " + socket);
                            NodeEditor.CreateConnectionMode = false;
                            NodeEditor.endSocket = socket as SocketIn;

                            var link = CreateInstance<Connection>();
                            link.startSocket = NodeEditor.startSocket;
                            link.endSocket = NodeEditor.endSocket;
                            NodeEditor.startSocket.connections.Add(link);
                            NodeEditor.endSocket.connections.Add(link);

                            DestroyImmediate(NodeEditor.tentativeConnection);
                            NodeEditor.tentativeConnection = null;
                            e.Use();
                        }
                    }

                    NodeEditor.CreateConnectionMode = false;
                    DestroyImmediate(NodeEditor.tentativeConnection);
                    NodeEditor.tentativeConnection = null;
                }

                //handle input first => !draw order
                if (!NodeEditor.CreateConnectionMode)
                    DragNodes(e, Graph.nodes);
            }

            try
            {
                BeginWindows();

                if (editorWindow)
                    GUI.Window(0, new Rect(0, 0, editorWindow.position.width, editorWindow.position.height), DrawNodeWindow, "");
                else
                    ShowEditor();

                EndWindows();
            }
            catch (UnityException ex)
            {
                Debug.LogError(ex);
                //editorWindow.Close();
            }


            if (Graph)
            {
                
                if (Event.current.button == 0 && NodeEditor.CreateConnectionMode)
                {
                    NodeEditor.tentativeConnection.DrawConnection(new Rect(NodeEditor.mousePos.x, NodeEditor.mousePos.y, 1, 1));
                    Repaint();
                }


                //draw curves
                for (int i = Graph.nodes.Count - 1; i >= 0; i -= 1)
                {
                    for (int n = 0; n < Graph.nodes[i].Outputs.Count; n++)
                    {
                        for (int k = 0; k < Graph.nodes[i].Outputs[n].connections.Count; k++)
                        {
                            Graph.nodes[i].Outputs[n].connections[k].DrawConnection();
                            Repaint();
                        }
                    }
                }
            }


            try
            {
                if (editorWindow)
                {
                    if (!sideWindowRect.Contains(Event.current.mousePosition) && (Event.current.button == 0 || Event.current.button == 1) && Event.current.type == EventType.MouseDown)
                        GUI.FocusControl("");

                    //var sideWindowWidth = Mathf.Min(600, Mathf.Max(200, (int)(position.width / 5)));
                    sideWindowRect = new Rect(0, 0, 155, editorWindow.position.height);
                    //GUI.Box(sideWindowRect, "");
                    //Debug.LogWarning(sideWindowRect);
                    GUILayout.BeginArea(sideWindowRect, GUI.skin.box);
                    //DrawSideWindow();      
                    if (Graph == null)
                        StatusMsg = "nothing loaded!";
                    else
                        StatusMsg = "loaded " + Graph.Name;
                    GUILayout.Label("Status: " + StatusMsg);
                    NodeEditor.NoteGraphName = GUILayout.TextField(NodeEditor.NoteGraphName, 32, GUILayout.ExpandWidth(true));

                    if (GUILayout.Button("Create"))
                    {
                        var asset = CreateInstance<NodeGraph>();
                        MakePrefabLibrary.CreateDataAsset(NodeEditor.NoteGraphName, asset);

                        if (asset != null)
                        {
                            asset.Name = NodeEditor.NoteGraphName;
                            Graph = asset;
                            StatusMsg = "created " + Graph.Name;                            
                        }
                    }

                    if (GUILayout.Button("Load"))
                    {
                        NodeGraph tmp = Resources.Load<NodeGraph>(NodeEditor.NoteGraphName);

                        if (tmp != null)
                        {
                            StatusMsg = "loaded " + tmp.Name;
                            Graph = tmp;
                        }
                    }

                    if (GUILayout.Button("Save"))
                    {

                    }

                    GUILayout.EndArea();
                }
            }
            catch (UnityException ex)
            {
                Debug.LogError(ex);
            }            
        }

        

        private void UpdateSelection(Vector2 mousePos)
        {
            //throw new NotImplementedException();
        }

        void DragNodes(Event e, List<Node> nodes)
        {
            var node = NodeEditor.GetNodeFromMousePos(e.mousePosition, nodes);
            if (node != NodeEditor.selectedNode && !NodeEditor.IsDragging)
                NodeEditor.selectedNode = node;

            if (NodeEditor.selectedNode)
            {
                if (e.button == 0 && e.type == EventType.MouseDown)
                {
                    //Debug.LogWarning("down");
                    NodeEditor.lastMousePos = e.mousePosition;
                    NodeEditor.IsDragging = true;
                }
            }

            if (NodeEditor.IsDragging)
            {
                if (e.button == 0 && e.type == EventType.MouseDrag)
                {
                    //Debug.LogWarning("move");
                    var offset = e.mousePosition - NodeEditor.lastMousePos;
                    NodeEditor.lastMousePos = e.mousePosition;
                    if(NodeEditor.selectedNode)
                         NodeEditor.selectedNode.rect = new Rect(NodeEditor.selectedNode.rect.x + offset.x, NodeEditor.selectedNode.rect.y + offset.y, NodeEditor.selectedNode.rect.width, NodeEditor.selectedNode.rect.height);
                    Repaint();
                }

                if (e.button == 0 && e.type == EventType.MouseUp)
                {
                    //Debug.LogWarning("up");
                    NodeEditor.lastMousePos = e.mousePosition;
                    NodeEditor.IsDragging = false;
                }
            }
        }

        void DrawNodeWindow(int id)
        {
            if (Graph)
            {
                for (int i = 0; i < Graph.nodes.Count; i++)
                {
                    Graph.nodes[i].DrawNode();
                }
            }
        }

        public void ContextCallback(object obj)
        {
            string clb = obj.ToString();

            if (clb.Equals("GUITestNode"))
            {
                var node = CreateInstance<GUITestNode>();
                node.Create(NodeEditor.mousePos);
                Graph.nodes.Add(node);
            }
            /*
            else if (clb.Equals("outputNode"))
            {
                OutputNode outputNode = ScriptableObject.CreateInstance<OutputNode>(); //new OutputNode();
                outputNode.windowRect = new Rect(mousePos.x, mousePos.y, 200, 100);

                windows.Add(outputNode);
            }
            else if (clb.Equals("calcNode"))
            {
                CalcNode calcNode = ScriptableObject.CreateInstance<CalcNode>(); //new CalcNode();
                calcNode.windowRect = new Rect(mousePos.x, mousePos.y, 200, 100);

                windows.Add(calcNode);
            }
            else if (clb.Equals("compNode"))
            {
                ComparisonNode comparisonNode = ScriptableObject.CreateInstance<ComparisonNode>(); //new ComparisonNode();
                comparisonNode.windowRect = new Rect(mousePos.x, mousePos.y, 200, 100);

                windows.Add(comparisonNode);
            }
            else if (clb.Equals("makeTransition"))
            {
                bool clickedOnWindow = false;
                int selectIndex = -1;

                for (int i = 0; i < windows.Count; i++)
                {
                    if (windows[i].windowRect.Contains(mousePos))
                    {
                        selectIndex = i;
                        clickedOnWindow = true;
                        break;
                    }
                }

                if (clickedOnWindow)
                {
                    selectednode = windows[selectIndex];
                    makeTransitionMode = true;
                }
            }
            else if (clb.Equals("deleteNode"))
            {
                bool clickedOnWindow = false;
                int selectIndex = -1;

                for (int i = 0; i < windows.Count; i++)
                {
                    if (windows[i].windowRect.Contains(mousePos))
                    {
                        selectIndex = i;
                        clickedOnWindow = true;
                        break;
                    }
                }

                if (clickedOnWindow)
                {
                    BaseNode selNode = windows[selectIndex];
                    windows.RemoveAt(selectIndex);

                    foreach (BaseNode n in windows)
                    {
                        n.NodeDeleted(selNode);
                    }
                }
            }
            */
        }
    }
}
