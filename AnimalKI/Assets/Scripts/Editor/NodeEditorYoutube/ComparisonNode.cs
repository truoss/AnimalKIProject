﻿using UnityEngine;
using System.Collections;
using UnityEditor;

namespace NodeEditor_Youtube
{
    public class ComparisonNode : BaseInputNode
    {

        private ComparisonType comparisonType;

        public enum ComparisonType
        {
            Greater,
            Less,
            Equal
        }

        private BaseInputNode input1;
        private Rect input1Rect;

        private BaseInputNode input2;
        private Rect input2Rect;

        //private string compareText = "";

        public ComparisonNode()
        {
            windowTitle = "Comparison Node";
            hasInputs = true;
        }

        public override void DrawWindow()
        {
            base.DrawWindow();

            Event e = Event.current;
            comparisonType = (ComparisonType)EditorGUILayout.EnumPopup("Comparison Type", comparisonType);

            string input1Title = "None";

            if (input1)
            {
                input1Title = input1.getResult();
            }

            //draw a label
            GUILayout.Label("Input 1: " + input1Title);

            //same as before
            if (e.type == EventType.Repaint)
            {
                input1Rect = GUILayoutUtility.GetLastRect();

            }

            string input2Title = "None";

            if (input2)
            {
                input2Title = input2.getResult();
            }

            GUILayout.Label("Input 2: " + input2Title);

            if (e.type == EventType.Repaint)
            {
                input2Rect = GUILayoutUtility.GetLastRect();

            }
        }

        public override void SetInput(BaseInputNode input, Vector2 clickPos)
        {
            clickPos.x -= windowRect.x;
            clickPos.y -= windowRect.y;

            if (input1Rect.Contains(clickPos))
            {
                input1 = input;

            }
            else if (input2Rect.Contains(clickPos))
            {
                input2 = input;
            }
        }

        public override string getResult()
        {
            float input1Value = 0;
            float input2Value = 0;

            if (input1)
            {
                string input1Raw = input1.getResult();
                float.TryParse(input1Raw, out input1Value);
            }

            if (input2)
            {
                string input2Raw = input2.getResult();
                float.TryParse(input2Raw, out input2Value);
            }

            string result = "false";

            switch (comparisonType)
            {
                case ComparisonType.Equal:
                    if (input1Value == input2Value)
                    {
                        result = "true";
                    }
                    break;
                case ComparisonType.Greater:
                    if (input1Value > input2Value)
                    {
                        result = "true";
                    }
                    break;
                case ComparisonType.Less:
                    if (input1Value < input2Value)
                    {
                        result = "true";
                    }
                    break;
            }

            return result;
        }

        public override void NodeDeleted(BaseNode node)
        {
            if (node.Equals(input1))
            {
                input1 = null;
            }

            if (node.Equals(input2))
            {
                input2 = null;
            }
        }

        public override BaseInputNode ClickedOnInput(Vector2 pos)
        {
            BaseInputNode retVal = null;

            pos.x -= windowRect.x;
            pos.y -= windowRect.y;

            if (input1Rect.Contains(pos))
            {
                retVal = input1;
                input1 = null;
            }
            else if (input2Rect.Contains(pos))
            {
                retVal = input2;
                input2 = null;
            }

            return retVal;
        }

        public override void DrawCurves()
        {
            if (input1)
            {
                Rect rect = windowRect;
                rect.x += input1Rect.x;
                rect.y += input1Rect.y + input1Rect.height / 2;
                rect.width = 1;
                rect.height = 1;

                NodeEditor.DrawNodeCurve(input1.windowRect, rect);
            }

            if (input2)
            {
                Rect rect = windowRect;
                rect.x += input2Rect.x;
                rect.y += input2Rect.y + input2Rect.height / 2;
                rect.width = 1;
                rect.height = 1;

                NodeEditor.DrawNodeCurve(input2.windowRect, rect);
            }
        }
    }
}








