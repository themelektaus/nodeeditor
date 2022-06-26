﻿using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace NodeEditor.DataRenderer2D.Editors
{
    [CustomPropertyDrawer(typeof(Point))]
    public class PointPropertyDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 4.5f * EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty sp, GUIContent label)
        {
            var labelPos = position;
            labelPos.x = 0;

            EditorGUI.LabelField(labelPos, (Regex.Match(label.text, @"\d+").Value).ToString());

            var pos = position;
            pos.height = EditorGUIUtility.singleLineHeight;

            EditorGUI.PropertyField(pos, sp.Position());
            pos.y += EditorGUIUtility.singleLineHeight;

            EditorGUI.PropertyField(pos, sp.Width());
            pos.y += EditorGUIUtility.singleLineHeight;

            EditorGUI.PropertyField(pos, sp.NextOffset());
            pos.y += EditorGUIUtility.singleLineHeight;


            EditorGUI.PropertyField(pos, sp.PrevOffset());
            pos.y += EditorGUIUtility.singleLineHeight;

            pos.height /= 2f;
            EditorGUI.DrawRect(pos, Color.black);
        }
    }

    static class SerializedPropertyAccessor
    {
        public static SerializedProperty Position(this SerializedProperty sp)
        {
            return sp.FindPropertyRelative("position");
        }

        public static SerializedProperty PrevOffset(this SerializedProperty sp)
        {
            return sp.FindPropertyRelative("previousControlOffset");
        }

        public static SerializedProperty NextOffset(this SerializedProperty sp)
        {
            return sp.FindPropertyRelative("nextControlOffset");
        }

        public static SerializedProperty Width(this SerializedProperty sp)
        {
            return sp.FindPropertyRelative("width");
        }
    }
}