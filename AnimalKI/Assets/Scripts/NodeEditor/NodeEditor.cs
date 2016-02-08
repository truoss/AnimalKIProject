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

        public static Node[] GetConnectedNodes(Node node)
        {
            List<Node> nodes = new List<Node>();
            var connections = GetAllOutputConnections(node);
            if(connections != null && connections.Length > 0)
            for (int i = 0; i < connections.Length; i++)
            {
                nodes.Add(connections[i].endSocket.parentNode);
            }

            return nodes.ToArray();
        }

        public static Connection[] GetAllOutputConnections(Node node)
        {
            List<Connection> connections = new List<Connection>();
            if (node.Outputs.Count > 0)
            {
                for (int i = 0; i < node.Outputs.Count; i++)
                {
                    if (node.Outputs[i].connections.Count > 0)
                    {
                        for (int x = 0; x < node.Outputs[i].connections.Count; x++)
                        {
                            connections.Add(node.Outputs[i].connections[x]);
                        }
                    }
                }
            }

            return connections.ToArray();
        }

        public static bool IsConnectionLoop(SocketIn inSocket, Connection connection)
        {
            if (inSocket.parentNode == connection.startSocket.parentNode)
                return true;
            else
            {
                //var nodes = GetAllOutputConnections(inSocket.parentNode);

                return RekursivLoopingCheck(inSocket, connection, inSocket.parentNode);
            }
        }

        private static bool RekursivLoopingCheck(SocketIn inSocket, Connection connection, Node node)
        {
            var connections = GetAllOutputConnections(node);

            if (connections == null || connections.Length == 0)
                return false;

            bool isLooping = false;
            for (int i = 0; i < connections.Length; i++)
            {
                if (connections[i].endSocket.parentNode == connection.startSocket.parentNode)
                {
                    isLooping = true;
                    break;
                }
                else
                    isLooping = RekursivLoopingCheck(inSocket, connection, connections[i].endSocket.parentNode);
            }

            return isLooping;
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
        
    }
}
