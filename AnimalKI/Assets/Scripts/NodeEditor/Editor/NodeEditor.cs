using UnityEngine;
using UnityEditor;

namespace NodeSystem
{
    public class NodeEditor : EditorWindow
    {
        public NodeGraph Graph;
        public static NodeEditor editorWindow;

        Vector2 lastMousePos;
        Vector2 mousePos;
        public Node selectedNode;
        bool IsDragging = false;
        bool CreateConnectionMode = false;
        SocketOut startSocket;
        SocketIn endSocket;
        Connection tentativeConnection;

        [MenuItem("Window/Node Editor")]
        static void ShowEditor()
        {
#if UNITY_EDITOR
            editorWindow = GetWindow<NodeEditor>();
#endif            
        }

        NodeGraph MakeTestGraph()
        {
            Debug.LogWarning("MakeTestGraph");
            var graph = CreateInstance<NodeGraph>();

            var node = CreateInstance<GUITestNode>();
            node.Create(new Vector2(45,30));

            graph.nodes.Add(node);

            return graph;
        }

        void OnSelectNode()
        {
        }

        void OnDeselectNode()
        {
        }

        void OnGUI()
        {
            GUI.skin = GUIx.I.skin;
            Event e = Event.current;

            mousePos = e.mousePosition;

            if (Graph == null)// && Input.GetMouseButtonDown(1))
            {
                Graph = MakeTestGraph();
            }

            if (Graph)
            {
                //Create nodes
                if (e.button == 1 && e.type == EventType.MouseDown && !IsDragging)
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
                if (e.button == 0 && e.type == EventType.MouseDown && !IsDragging)
                {
                    var node = ClickedOnNode(mousePos);
                    if (node)
                    {
                        var socket = ClickedOnSocket(node, mousePos);
                        if (socket && socket is SocketOut)
                        {
                            Debug.LogWarning("socket: " + socket);
                            CreateConnectionMode = true;
                            startSocket = socket as SocketOut;
                            tentativeConnection = CreateInstance<Connection>();
                            tentativeConnection.startSocket = startSocket;
                            e.Use();
                        }
                        else
                            Debug.LogWarning("no socket clicked!");
                    }
                    else
                        Debug.LogWarning("no node clicked!");
                }

                if (e.button == 0 && e.type == EventType.MouseUp && CreateConnectionMode && !IsDragging)
                {
                    var node = ClickedOnNode(mousePos);
                    if (node)
                    {
                        var socket = ClickedOnSocket(node, mousePos);
                        if (socket && socket is SocketIn && socket.parent != startSocket.parent)
                        {
                            Debug.LogWarning("socket: " + socket);
                            CreateConnectionMode = false;
                            endSocket = socket as SocketIn;

                            var link = CreateInstance<Connection>();
                            link.startSocket = startSocket;
                            link.endSocket = endSocket;
                            startSocket.connections.Add(link);
                            endSocket.connections.Add(link);

                            DestroyImmediate(tentativeConnection);
                            tentativeConnection = null;
                            e.Use();
                        }
                    }

                    CreateConnectionMode = false;
                    DestroyImmediate(tentativeConnection);
                    tentativeConnection = null;
                }                

                //handle input first => !draw order
                if(!CreateConnectionMode)
                    DragNodes(e);
                

                BeginWindows();

                if (editorWindow)
                    GUI.Window(0, new Rect(0, 0, editorWindow.position.width, editorWindow.position.height), DrawNodeWindow, "");
                else
                    ShowEditor();

                EndWindows();


                if (e.button == 0 && CreateConnectionMode)
                {
                    tentativeConnection.DrawConnection(new Rect(mousePos.x, mousePos.y, 1, 1));
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
        }        

        private void DragNodes(Event e)
        {
            var node = ClickedOnNode(e.mousePosition);
            if (node != selectedNode && !IsDragging)
                selectedNode = node;

            if (selectedNode)
            {
                if (e.button == 0 && e.type == EventType.MouseDown)
                {
                    //Debug.LogWarning("down");
                    lastMousePos = e.mousePosition;
                    IsDragging = true;
                }                
            }

            if (IsDragging)
            {
                if (e.button == 0 && e.type == EventType.MouseDrag)
                {
                    //Debug.LogWarning("move");
                    var offset = e.mousePosition - lastMousePos;
                    lastMousePos = e.mousePosition;
                    selectedNode.rect = new Rect(selectedNode.rect.x + offset.x, selectedNode.rect.y + offset.y, selectedNode.rect.width, selectedNode.rect.height);
                    Repaint();
                }

                if (e.button == 0 && e.type == EventType.MouseUp)
                {
                    //Debug.LogWarning("up");
                    lastMousePos = e.mousePosition;
                    IsDragging = false;
                }
            }
        }

        public static Rect GetAbsoluteRect(Rect rect, Rect parentRect)
        {
            return new Rect(rect.x + parentRect.x, rect.y + parentRect.y, rect.width, rect.height);
        }
                
        public Rect GUIToScreenRect(Rect rect)
        {
            Vector2 screenPoint = GUIUtility.GUIToScreenPoint(new Vector2(rect.x, rect.y));
            return new Rect(screenPoint.x, screenPoint.y, rect.width, rect.height);
        }

        void DrawNodeWindow(int id)
        {
            for (int i = 0; i < Graph.nodes.Count; i++)
            {
                Graph.nodes[i].DrawNode();                
            }
        }


        Node ClickedOnNode(Vector2 mousePos)
        {
            for (int i = Graph.nodes.Count - 1; i >= 0; i -= 1)
            {
                if (Graph.nodes[i].rect.Contains(mousePos))
                {
                    return Graph.nodes[i];
                }
            }
            return null;
        }


        Socket ClickedOnSocket(Node node, Vector2 mousePos)
        {
            for (int i = 0; i < node.Inputs.Count; i++)
            {
                var absoluteRect = GetAbsoluteRect(node.Inputs[i].rect, node.rect);
                //GUI.Box(screenRect, GUIx.empty);
                //Debug.LogWarning(absoluteRect + " " + node.Inputs[i].rect + " " + mousePos);
                if (absoluteRect.Contains(mousePos))
                {
                    return node.Inputs[i];
                }
            }

            for (int i = 0; i < node.Outputs.Count; i++)
            {
                //var screenRect = GUIToScreenRect(node.Inputs[i].rect);
                var absoluteRect = GetAbsoluteRect(node.Outputs[i].rect, node.rect);
                //GUI.Box(screenRect, GUIx.empty);
                if (absoluteRect.Contains(mousePos))
                {
                    return node.Outputs[i];
                }
            }

            return null;
        }

        
        void ContextCallback(object obj)
        {
            string clb = obj.ToString();

            if (clb.Equals("GUITestNode"))
            {
                var node = CreateInstance<GUITestNode>();
                node.Create(mousePos);
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
