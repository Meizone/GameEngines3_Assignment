//using System.Collections;
//using System.Collections.Generic;
//using UnityEditor;
//using UnityEditor.UIElements;
//using UnityEngine;
//using UnityEngine.UIElements;

//[CustomPropertyDrawer(typeof(Effect))]
//public class IngredientDrawerUIE : PropertyDrawer
//{
//    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
//    {
//        EditorGUI.BeginProperty(position, label, property);
//        EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
//        int indent = EditorGUI.indentLevel;
//        EditorGUI.indentLevel = indent + 1;
//        float lineHeight = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

//        LinkedList<SerializedProperty> properties = new LinkedList<SerializedProperty>();
//        properties.AddLast(property.FindPropertyRelative("type"));
//        switch ((Effect.Type)properties.Last.Value.intValue)
//        {
//            case Effect.Type.AffectResource:
//                properties.AddLast(property.FindPropertyRelative("target"));
//                properties.AddLast(property.FindPropertyRelative("resource"));
//                properties.AddLast(property.FindPropertyRelative("amount"));
//                break;
//            case Effect.Type.Chance:
//                properties.AddLast(property.FindPropertyRelative("chance"));
//                break;
//            default:
//                break;
//        }

//        foreach (SerializedProperty serializedProperty in properties)
//        {
//            position = new Rect(position.x, position.y + lineHeight, position.width, EditorGUIUtility.singleLineHeight);
//            EditorGUI.PropertyField(position, serializedProperty);
//        }

//        EditorGUI.indentLevel = indent;
//        EditorGUI.EndProperty();



//        //SerializedProperty p = property.FindPropertyRelative("amount");
//        //
//        //EditorGUI.BeginProperty(position, label, property);
//        //
//        //// Draw label
//        ////position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
//        //EditorGUI.LabelField(new Rect(position.x, position.y, position.width, 30), label);
//        //
//        //// Calculate rects
//        //var amountRect = new Rect(position.x, position.y, 200, position.height);
//        //
//        //// Draw fields - passs GUIContent.none to each so they are drawn without labels
//        //EditorGUI.PropertyField(amountRect, p);
//        //
//        //EditorGUI.EndProperty();
//    }

//    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
//    {
//        return (base.GetPropertyHeight(property, label) + EditorGUIUtility.standardVerticalSpacing) * 4;
//    }
//}