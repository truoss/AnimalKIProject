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
        Rect scrollViewRect;
        public static string StatusMsg;// = "nothing loaded!";

        //Vector2 offset = Vector2.zero;
        Vector2 lastMousePos = Vector2.zero;

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
            GUI.skin = GUIx.I.skin;
            Event e = Event.current;

            //remove mousewheel input
            if (e.type == EventType.ScrollWheel)
                e.Use();

            if (editorWindow)
                editorWindow.minSize = new Vector2(512, 512);
            else
                ShowEditor();
            

            //init rects
            sideWindowRect = new Rect(0, 0, 155, editorWindow.position.height);
            scrollViewRect = new Rect(sideWindowRect.width, 0, editorWindow.position.width - sideWindowRect.width, editorWindow.position.height);

            if (Graph)
            {
                //Init NodeEditor
                NodeEditor.mousePos = e.mousePosition;
                NodeEditor.Graph = Graph;
                NodeEditor.viewOffset.x = scrollViewRect.x;
                NodeEditor.viewOffset.y = scrollViewRect.y;
                NodeEditor.canvasSize = new Vector2(4000,4000);

                //Update Hover
                NodeEditor.hoveredNode = GetNodeFromMousePos(NodeEditor.mousePos, Graph.nodes);

                if (NodeEditor.hoveredNode)
                    NodeEditor.hoveredSocket = GetSocketFromMousePos(NodeEditor.hoveredNode, NodeEditor.mousePos);
                Repaint();


                //Create nodes
                ContextMenu(e);

                //check if select output socket
                if (e.button == 0 && e.type == EventType.MouseDown && !NodeEditor.IsDragging)
                {
                    var node = GetNodeFromMousePos(NodeEditor.mousePos, Graph.nodes);
                    //Debug.LogWarning("node: " + node);
                    if (node)
                    {
                        var socket = GetSocketFromMousePos(node, NodeEditor.mousePos);
                        //Debug.LogWarning("socket: " + socket);
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
                    var node = GetNodeFromMousePos(NodeEditor.mousePos, Graph.nodes);
                    if (node)
                    {
                        var socket = GetSocketFromMousePos(node, NodeEditor.mousePos);
                        if (socket && socket is SocketIn
                            && socket.parentNode != NodeEditor.startSocket.parentNode
                            && !NodeEditor.IsConnectionLoop(socket as SocketIn, NodeEditor.tentativeConnection)
                            && !NodeEditor.IsDoubleConnection(socket as SocketIn, NodeEditor.tentativeConnection.startSocket))
                        {
                            //Debug.LogWarning("socket: " + socket);
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


                //GUI.Box(scrollViewRect, GUIx.empty, GUIx.I.window);
                Graph.scrollPos = GUI.BeginScrollView(scrollViewRect, Graph.scrollPos, new Rect(0, 0, NodeEditor.canvasSize.x, NodeEditor.canvasSize.y), false, false);
                
                //background
                //TODO: only draw visible
                for (int i = 0; i < NodeEditor.canvasSize.x / GUIx.I.background.fixedWidth; i++)
                {
                    GUI.DrawTexture(new Rect(GUIx.I.background.fixedWidth * i, 0, GUIx.I.background.fixedWidth, GUIx.I.background.fixedHeight), GUIx.I.background.normal.background);
                    GUI.DrawTexture(new Rect(GUIx.I.background.fixedWidth * i, GUIx.I.background.fixedWidth, GUIx.I.background.fixedWidth, GUIx.I.background.fixedHeight), GUIx.I.background.normal.background);
                    GUI.DrawTexture(new Rect(GUIx.I.background.fixedWidth * i, GUIx.I.background.fixedWidth * 2, GUIx.I.background.fixedWidth, GUIx.I.background.fixedHeight), GUIx.I.background.normal.background);
                    GUI.DrawTexture(new Rect(GUIx.I.background.fixedWidth * i, GUIx.I.background.fixedWidth * 3, GUIx.I.background.fixedWidth, GUIx.I.background.fixedHeight), GUIx.I.background.normal.background);
                    GUI.DrawTexture(new Rect(GUIx.I.background.fixedWidth * i, GUIx.I.background.fixedWidth * 4, GUIx.I.background.fixedWidth, GUIx.I.background.fixedHeight), GUIx.I.background.normal.background);
                    GUI.DrawTexture(new Rect(GUIx.I.background.fixedWidth * i, GUIx.I.background.fixedWidth * 5, GUIx.I.background.fixedWidth, GUIx.I.background.fixedHeight), GUIx.I.background.normal.background);
                    GUI.DrawTexture(new Rect(GUIx.I.background.fixedWidth * i, GUIx.I.background.fixedWidth * 6, GUIx.I.background.fixedWidth, GUIx.I.background.fixedHeight), GUIx.I.background.normal.background);
                    GUI.DrawTexture(new Rect(GUIx.I.background.fixedWidth * i, GUIx.I.background.fixedWidth * 7, GUIx.I.background.fixedWidth, GUIx.I.background.fixedHeight), GUIx.I.background.normal.background);
                    //GUI.DrawTexture(new Rect(GUIx.I.background.fixedWidth * i, GUIx.I.background.fixedWidth * 8, GUIx.I.background.fixedWidth, GUIx.I.background.fixedHeight), GUIx.I.background.normal.background);
                }                
                
                // Draw nodes
                for (int i = 0; i < Graph.nodes.Count; i++)
                {
                    Graph.nodes[i].DrawNode();
                }

                //GUILayout.EndScrollView();
                GUI.EndScrollView();


                // Check if the mouse is above our scrollview.
                if (e.button == 2 && e.type == EventType.MouseDown && scrollViewRect.Contains(Event.current.mousePosition) && !NodeEditor.isPanning)
                    NodeEditor.isPanning = true;

                if (e.button == 2 && e.type == EventType.MouseUp && NodeEditor.isPanning)
                    NodeEditor.isPanning = false;

                //Debug.LogWarning(NodeEditor.isPanning);
                if (NodeEditor.isPanning)
                {
                    // Only move if the distance between the last mouse position and the current is less than 50.
                    // Without this it jumps during the drag.
                    if (Vector2.Distance(NodeEditor.mousePos, NodeEditor.lastMousePos) < 50)
                    {
                        // Calculate the delta x and y.
                        float x = NodeEditor.lastMousePos.x - NodeEditor.mousePos.x;
                        float y = NodeEditor.lastMousePos.y - NodeEditor.mousePos.y;

                        // Add the delta moves to the scroll position.
                        Graph.scrollPos.x += x;
                        Graph.scrollPos.y += y;
                        //Event.current.Use();
                        Repaint();
                    }
                }


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

            if (!sideWindowRect.Contains(Event.current.mousePosition) && (Event.current.button == 0 || Event.current.button == 1) && Event.current.type == EventType.MouseDown)
                GUI.FocusControl("");

            //Debug.LogWarning(sideWindowRect);
            GUILayout.BeginArea(sideWindowRect, GUI.skin.box);
            //DrawSideWindow();      
            if (Graph == null)
                StatusMsg = "nothing loaded!";
            else
                StatusMsg = "loaded " + Graph.Name;
            GUILayout.Label("Status: " + StatusMsg);
            NodeEditor.NoteGraphName = GUILayout.TextField(NodeEditor.NoteGraphName, 32, GUILayout.ExpandWidth(true));

            GUI.enabled = NodeEditor.NoteGraphName != "";
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

            GUI.enabled = NodeEditor.NoteGraphName != "";
            if (GUILayout.Button("Load"))
            {
                NodeGraph tmp = Resources.Load<NodeGraph>(NodeEditor.NoteGraphName);

                if (tmp != null)
                {
                    StatusMsg = "loaded " + tmp.Name;
                    Graph = tmp;
                }
            }

            GUI.enabled = Graph != null;
            if (GUILayout.Button("Center View"))
            {
                Graph.scrollPos = new Vector2(NodeEditor.canvasSize.x * 0.5f, NodeEditor.canvasSize.y * 0.5f);
            }
            GUILayout.EndArea();

            NodeEditor.lastMousePos = Event.current.mousePosition;
        }

        private void ContextMenu(Event e)
        {
            if (e.button == 1 && e.type == EventType.MouseDown && !NodeEditor.IsDragging)
            {
                NodeEditor.selectedNode = GetNodeFromMousePos();

                if (NodeEditor.selectedNode)
                {
                    NodeEditor.selectedSocket = GetSocketFromMousePos();
                    if (NodeEditor.selectedSocket)
                    {
                        GenericMenu menu = new GenericMenu();

                        menu.AddItem(new GUIContent("Delete Connection"), false, ContextCallback, "DeleteConnection");

                        menu.ShowAsContext();
                    }
                    else
                    {
                        GenericMenu menu = new GenericMenu();

                        menu.AddItem(new GUIContent("Delete Node"), false, ContextCallback, "DeleteNode");

                        menu.ShowAsContext();
                    }
                }
                else
                {
                    GenericMenu menu = new GenericMenu();

                    menu.AddItem(new GUIContent("Add Test Node"), false, ContextCallback, "GUITestNode");
                    //menu.AddItem(new GUIContent("Add Output Node"), false, ContextCallback, "outputNode");
                    //menu.AddItem(new GUIContent("Add Calculation Node"), false, ContextCallback, "calcNode");
                    //menu.AddItem(new GUIContent("Add Comparison Node"), false, ContextCallback, "compNode");

                    menu.ShowAsContext();
                }
            }
        }

        

        public Node GetNodeFromMousePos()
        {
            return GetNodeFromMousePos(NodeEditor.mousePos, Graph.nodes);
        }

        public Node GetNodeFromMousePos(Vector2 mousePos, List<Node> nodes)
        {
            for (int i = nodes.Count - 1; i >= 0; i -= 1)
            {
                var _rect = GUIRectToScreenRect(nodes[i].rect);
                //Debug.LogWarning(nodes[i].rect + " screen: " + _rect + " mousePos " + mousePos + " scrollPos: " + Graph.scrollPos);                              
                if (_rect.Contains(mousePos))
                {
                    return nodes[i];
                }
            }
            return null;
        }

        public Socket GetSocketFromMousePos()
        {
            return GetSocketFromMousePos(NodeEditor.selectedNode, NodeEditor.mousePos);
        }

        public Socket GetSocketFromMousePos(Node node, Vector2 mousePos)
        {
            for (int i = 0; i < node.Inputs.Count; i++)
            {
                var absoluteRect = GUIRectToScreenRect(node.Inputs[i].rect, node.rect);
                if (absoluteRect.Contains(mousePos))
                    return node.Inputs[i];                
            }

            for (int i = 0; i < node.Outputs.Count; i++)
            {
                var absoluteRect = GUIRectToScreenRect(node.Outputs[i].rect, node.rect);
                if (absoluteRect.Contains(mousePos))
                    return node.Outputs[i];
            }

            return null;
        }


        public Rect GUIRectToScreenRect(Rect rect)
        {
            //need all nested rects + scrollPos + windowPos
            return new Rect(rect.x + sideWindowRect.width - Graph.scrollPos.x, rect.y - Graph.scrollPos.y, rect.width, rect.height);
        }

        public Rect GUIRectToScreenRect(Rect rect, Rect parentRect)
        {
            //need all nested rects + scrollPos + windowPos
            return new Rect(rect.x + sideWindowRect.width - Graph.scrollPos.x + parentRect.x, rect.y - Graph.scrollPos.y + parentRect.y, rect.width, rect.height);
        }

        public Vector2 ScreenPosToGUIPos(Vector2 screen)
        {
            return new Vector2(screen.x - sideWindowRect.width + Graph.scrollPos.x, screen.y + Graph.scrollPos.y);
        }

        public Rect GUIRectToScreenRectScrollView(Rect rect)
        {
            Vector2 screenPoint = EditorGUIUtility.GUIToScreenPoint(new Vector2(rect.x, rect.y));
            return new Rect(screenPoint.x - Graph.scrollPos.x - sideWindowRect.width, screenPoint.y - Graph.scrollPos.y, rect.width, rect.height);
        }

        void DragNodes(Event e, List<Node> nodes)
        {
            /*
            var node = GetNodeFromMousePos(e.mousePosition, nodes);
            if (node != NodeEditor.selectedNode && !NodeEditor.IsDragging)
                NodeEditor.selectedNode = node;
            */
            if (e.button == 0 && e.type == EventType.MouseDown)
            {
                NodeEditor.selectedNode = GetNodeFromMousePos();

                if (NodeEditor.selectedNode)
                {
                
                    //Debug.LogWarning("down");
                    lastMousePos = e.mousePosition;
                    NodeEditor.IsDragging = true;
                }
            }

            if (NodeEditor.IsDragging)
            {
                if (e.button == 0 && e.type == EventType.MouseDrag)
                {
                    //Debug.LogWarning("move");
                    var offset = e.mousePosition - lastMousePos;
                    //Debug.LogWarning(offset.ToString("f4") + " " + NodeEditor.mousePos + " " + NodeEditor.lastMousePos);
                    //offset = ScreenPosToGUIPos(offset);                    
                    lastMousePos = e.mousePosition;
                    if (NodeEditor.selectedNode)
                        NodeEditor.selectedNode.rect = new Rect(NodeEditor.selectedNode.rect.x + offset.x, NodeEditor.selectedNode.rect.y + offset.y, NodeEditor.selectedNode.rect.width, NodeEditor.selectedNode.rect.height);
                    else
                        Debug.Log("Selected node == null!");
                    Repaint();
                }

                if (e.button == 0 && e.type == EventType.MouseUp)
                {
                    //Debug.LogWarning("up");
                    lastMousePos = e.mousePosition;
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
                node.Create(NodeEditor.ScreenToGUIPos(NodeEditor.mousePos));
                Graph.nodes.Add(node);
            }
            else if (clb.Equals("DeleteNode"))
            {
                //remove connections
                var allSockets = NodeEditor.selectedNode.GetAllSockets();
                for (int i = 0; i < allSockets.Length; i++)
                {
                    for (int x = 0; x < allSockets[i].connections.Count; x++)
                    {
                        allSockets[i].connections[x].startSocket = null;
                        allSockets[i].connections[x].endSocket = null;
                    }

                    allSockets[i].connections.Clear();
                }
                //remove sockets
                //remove node
                Graph.nodes.Remove(NodeEditor.selectedNode);
            }
            else if (clb.Equals("DeleteConnection"))
            {
                //remove connections
                for (int i = 0; i < NodeEditor.selectedSocket.connections.Count; i++)
                {
                    NodeEditor.selectedSocket.connections[i].startSocket = null;
                    NodeEditor.selectedSocket.connections[i].endSocket = null;
                    NodeEditor.selectedSocket.connections.Clear();
                }
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
