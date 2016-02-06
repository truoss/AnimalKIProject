using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;

namespace NodeSystem
{
    public static class NodeEditor
    {
        public static string NoteGraphName = "";

        public static Vector2 lastMousePos;
        public static Vector2 mousePos;

        public static bool IsDragging = false;
        public static bool CreateConnectionMode = false;
        public static SocketOut startSocket;
        public static SocketIn endSocket;
        public static Connection tentativeConnection;

        public static Node hoveredNode;
        public static bool IsHoveredNode(Node node)
        {
            if (node == hoveredNode)
                return true;
            else
                return false;
        }

        public static Node selectedNode;
        public static bool IsSelectedNode(Node node)
        {
            if (node == selectedNode)
                return true;
            else
                return false;
        }

        public static Socket hoveredSocket;
        public static bool IsHoveredSocket(Socket socket)
        {
            if (socket == hoveredSocket)
                return true;
            else
                return false;
        }

        public static Socket selectedSocket;
        public static bool IsSelectedSocket(Socket socket)
        {
            if (socket == hoveredSocket)
                return true;
            else
                return false;
        }

        public static bool UpdateInput(NodeGraph Graph, Event e)
        {
            mousePos = e.mousePosition;

            //return UpdateHovering(mousePos, Graph.nodes);

            return UpdateSelecting(mousePos, Graph.nodes);            
        }

        static bool UpdateHovering(Vector2 mousePos, List<Node> nodes)
        {
            bool changes = false;
            var tmp = GetNodeFromMousePos(mousePos, nodes);
            if (tmp != hoveredNode)
                changes = true;

            hoveredNode = tmp;

            if (hoveredNode)
            {
                var tmp2 = GetSocketFromMousePos(hoveredNode, mousePos);
                if (tmp2 != hoveredSocket)
                {
                    changes |= true;
                }
                hoveredSocket = tmp2;                
            }

            return changes;
        }

        static bool UpdateSelecting(Vector2 mousePos, List<Node> nodes)
        {
            bool changes = false;
            var tmp = GetNodeFromMousePos(mousePos, nodes);
            if (tmp != selectedNode)
                changes = true;

            selectedNode = tmp;

            if (selectedNode)
            {
                var tmp2 = GetSocketFromMousePos(selectedNode, mousePos);
                if (tmp2 != selectedSocket)
                {
                    changes |= true;
                }
                selectedSocket = tmp2;
            }

            return changes;
        }



        public static bool IsConnectionLoop(SocketIn inSocket, Connection connection)
        {
            if (inSocket.parent == connection.startSocket.parent)
                return true;
            else
                return false;
        }

        static public Rect GetAbsoluteRect(Rect rect, Rect parentRect)
        {
            return new Rect(rect.x + parentRect.x, rect.y + parentRect.y, rect.width, rect.height);
        }

        static public Rect GUIToScreenRect(Rect rect)
        {
            Vector2 screenPoint = GUIUtility.GUIToScreenPoint(new Vector2(rect.x, rect.y));
            return new Rect(screenPoint.x, screenPoint.y, rect.width, rect.height);
        }
        

        public static Node GetNodeFromMousePos(Vector2 mousePos, List<Node> nodes)
        {
            for (int i = nodes.Count - 1; i >= 0; i -= 1)
            {
                if (nodes[i].rect.Contains(mousePos))
                {
                    return nodes[i];
                }
            }
            return null;
        }


        public static Socket GetSocketFromMousePos(Node node, Vector2 mousePos)
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


        
    }
}
