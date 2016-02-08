#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace NodeSystem
{
    public class Connection : ScriptableObject
    {
        public SocketOut startSocket;
        public SocketIn endSocket;

#if UNITY_EDITOR
        public virtual void DrawConnection()
        {
            if (startSocket == null || endSocket == null)
                return;

            DrawNodeCurve(NodeEditor.GUIRectToScreenRect(startSocket.rect, startSocket.parentNode.rect), NodeEditor.GUIRectToScreenRect(endSocket.rect, endSocket.parentNode.rect));
        }

        public virtual void DrawConnection(Rect endpoint)
        {
            if (startSocket == null)
                return;

            DrawNodeCurve(NodeEditor.GUIRectToScreenRect(startSocket.rect, startSocket.parentNode.rect), endpoint);
        }

        public static void DrawNodeCurve(Rect start, Rect end)
        {
            //Debug.LogWarning("DrawNodeCurve: " + start + " " + end);

            Vector3 startPos = new Vector3(start.x + GUIx.I.socketStyle.fixedWidth, start.y + GUIx.I.socketStyle.fixedHeight*0.5f, 0);
            Vector3 endPos = new Vector3(end.x, end.y + GUIx.I.socketStyle.fixedHeight * 0.5f, 0);
            Vector3 startTan = startPos + Vector3.right * 50;
            Vector3 endTan = endPos + Vector3.left * 50;
            Color shadowCol = new Color(0, 0, 0, .06f);

            for (int i = 0; i < 3; i++)
            {
                Handles.DrawBezier(startPos, endPos, startTan, endTan, shadowCol, null, (i + 1) * 5);
            }

            Handles.DrawBezier(startPos, endPos, startTan, endTan, Color.black, null, 1);
        }

        
#endif
    }
}
