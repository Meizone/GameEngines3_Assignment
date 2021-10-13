using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[CustomPropertyDrawer(typeof(Payment))]
public class PaymentDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);
        //EditorGUI.PrefixLabel(position, label);
        int indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = indent + 1;
        float lineHeight = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
        float horizontalspacing = -(15 - EditorGUIUtility.standardVerticalSpacing);

        EditorGUI.PropertyField(new Rect(position.x, position.y, position.width, lineHeight),
            property.FindPropertyRelative("resource"), label);

        SerializedProperty direction = property.FindPropertyRelative("direction");
        if ((Payment.Direction)direction.intValue == Payment.Direction.None)
        {
            EditorGUI.PropertyField(new Rect(position.x, position.y + lineHeight, position.width, lineHeight),
                direction, new GUIContent("Amount"));
        }
        else
        {
            EditorGUI.MultiPropertyField(new Rect(position.x, position.y + lineHeight, position.width, lineHeight),
                new GUIContent[] { new GUIContent(""), new GUIContent(""), new GUIContent("") },
                property.FindPropertyRelative("direction"), new GUIContent("Amount"));
        }

        EditorGUI.indentLevel = indent;
        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return (base.GetPropertyHeight(property, label) + EditorGUIUtility.standardVerticalSpacing) * 2;
    }
}