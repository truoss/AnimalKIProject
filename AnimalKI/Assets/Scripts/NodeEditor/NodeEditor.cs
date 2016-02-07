using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;

namespace NodeSystem
{
    public static class NodeEditor
    {
        public static string NoteGraphName = "";
        public static NodeGraph Graph;

        public static Vector2 viewOffset;
        public static Vector2 canvasSize;

        public static Vector2 lastMousePos;
        public static Vector2 mousePos;
        public static Vector2 mouseOffset;
        //public static Vector2 scrollpos;

        public static bool isPanning = false;
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

        public static Vector2 GetMouseOffset()
        {
            return mousePos - lastMousePos;
        }

        public static Socket selectedSocket;
        public static bool IsSelectedSocket(Socket socket)
        {
            if (socket == hoveredSocket)
                return true;
            else
                return false;
        }

        public static bool UpdateInput(NodeGraph Graph)
        {
            bool changed = false;

            //return UpdateHovering(mousePos, Graph.nodes);

            //changed = UpdateSelecting(mousePos, Graph.nodes);

            return changed;
        }

        /*
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
        */

        public static bool IsConnectionLoop(SocketIn inSocket, Connection connection)
        {
            if (inSocket.parent == connection.startSocket.parent)
                return true;
            else
                return false;
        }

        public static bool IsDoubleConnection(SocketIn inSocket, SocketOut outSocket)
        {
            for (int i = 0; i < inSocket.connections.Count; i++)
            {
                if (inSocket.connections[i].startSocket == outSocket)
                    return true;
            }
            return false;
        }

        /*
        public static Rect GetAbsoluteRect(Rect rect, Rect parentRect)
        {
            return new Rect(rect.x + parentRect.x, rect.y + parentRect.y, rect.width, rect.height);
        }
        */

        public static Rect GUIRectToScreenRect(Rect rect)
        {
            //need all nested rects + scrollPos + windowPos
            return new Rect(rect.x + viewOffset.x - Graph.scrollPos.x, rect.y + viewOffset.y - Graph.scrollPos.y, rect.width, rect.height);
        }

        public static Rect GUIRectToScreenRect(Rect rect, Rect parentRect)
        {
            //need all nested rects + scrollPos + windowPos
            return new Rect(rect.x + viewOffset.x - Graph.scrollPos.x + parentRect.x, rect.y + viewOffset.y - Graph.scrollPos.y + parentRect.y, rect.width, rect.height);
        }

        public static Vector2 ScreenToGUIPos(Vector2 v2)
        {
            return new Vector2(v2.x - viewOffset.x + Graph.scrollPos.x, v2.y - viewOffset.y + Graph.scrollPos.y);
        }


        /*
        static public Rect GUIToScreenRect(Rect rect)
        {
            Vector2 screenPoint = GUIUtility.GUIToScreenPoint(new Vector2(rect.x, rect.y));
            return new Rect(screenPoint.x-scrollpos.x, screenPoint.y-scrollpos.x, rect.width, rect.height);
        }
        */

        /*
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
        */
    }
}
