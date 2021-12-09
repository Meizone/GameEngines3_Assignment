using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[CustomPropertyDrawer(typeof(Resource))]
public class ResourceDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);
        EditorGUI.PrefixLabel(position, label);
        //float lineHeight = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

        float gap = 6;
        Rect r0 = new Rect(position.x, position.y, EditorGUIUtility.labelWidth, position.height);
        Rect r1 = new Rect(r0.x + r0.width, r0.y, EditorGUIUtility.fieldWidth, r0.height);
        Rect r2 = new Rect(r1.x + r1.width + gap, r0.y, position.width - r0.width - (EditorGUIUtility.fieldWidth * 2) - gap - gap, r0.height);
        Rect r3 = new Rect(r2.x + r2.width + gap, r0.y, EditorGUIUtility.fieldWidth, r0.height);
        
        EditorGUI.PropertyField(r1, property.FindPropertyRelative("_min"), new GUIContent(""));

        EditorGUI.Slider(r2,
            property.FindPropertyRelative("_current"),
            property.FindPropertyRelative("_min").floatValue,
            property.FindPropertyRelative("_max").floatValue, new GUIContent(""));

        EditorGUI.PropertyField(r3, property.FindPropertyRelative("_max"), new GUIContent(""));

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return (base.GetPropertyHeight(property, label) + EditorGUIUtility.standardVerticalSpacing);
    }
}
