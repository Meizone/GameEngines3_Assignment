using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(SelectableReferenceType))]
public class SelectableReferenceTypeDrawer : PropertyDrawer
{
    List<Type> possibleTypes;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (property.propertyType != SerializedPropertyType.ManagedReference) return;

        if (possibleTypes == null)
        {
            possibleTypes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(assembly => assembly.GetTypes())
                .Where(type => ((SelectableReferenceType)attribute).Type.IsAssignableFrom(type) && !type.IsAbstract)
                .ToList();

            possibleTypes.Insert(0, null);
        }

        Rect popupPosition = new Rect(position);
        popupPosition.width -= EditorGUIUtility.labelWidth;
        popupPosition.x += EditorGUIUtility.labelWidth;
        popupPosition.height = EditorGUIUtility.singleLineHeight;

        string[] typePopupNameArray = possibleTypes.Select(type => type == null ? "<null>" : type.ToString()).ToArray();
        string[] typeFullNameArray = possibleTypes.Select(type => type == null ? "" : string.Format("{0} {1}", type.Assembly.ToString().Split(',')[0], type.FullName)).ToArray();

        //Get the type of serialized object 
        int currentTypeIndex = Array.IndexOf(typeFullNameArray, property.managedReferenceFullTypename);
        Type currentObjectType = possibleTypes[currentTypeIndex];

        int selectedTypeIndex = EditorGUI.Popup(popupPosition, currentTypeIndex, typePopupNameArray);
        if (selectedTypeIndex >= 0 && selectedTypeIndex < possibleTypes.Count)
        {
            if (currentObjectType != possibleTypes[selectedTypeIndex])
            {
                if (possibleTypes[selectedTypeIndex] == null)
                    //bug? NullReferenceException occurs when put null 
                    property.managedReferenceValue = null;
                else
                    property.managedReferenceValue = Activator.CreateInstance(possibleTypes[selectedTypeIndex]);
                currentObjectType = possibleTypes[selectedTypeIndex];
            }
        }

        EditorGUI.PropertyField(position, property, label, true);
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, true);
    }
}




//[CustomPropertyDrawer(typeof(SelectableReferenceType))]
//public class SelectImplementationDrawer : PropertyDrawer
//{
//    private Type[] _implementations;
//    private int _implementationTypeIndex;
//
//    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
//    {
//        if (_implementations == null || GUILayout.Button("Refresh implementations"))
//        {
//            _implementations = GetImplementations((attribute as SelectableReferenceType).Type)
//                .Where(impl => !impl.IsSubclassOf(typeof(UnityEngine.Object))).ToArray();
//        }
//
//        EditorGUILayout.LabelField($"Found {_implementations.Count()} implementations");
//
//        _implementationTypeIndex = EditorGUILayout.Popup(new GUIContent("Implementation"),
//            _implementationTypeIndex, _implementations.Select(impl => impl.FullName).ToArray());
//
//        if (GUILayout.Button("Create instance"))
//        {
//            property.managedReferenceValue = Activator.CreateInstance(_implementations[_implementationTypeIndex]);
//        }
//        EditorGUILayout.PropertyField(property, true);
//    }
//
//    public static Type[] GetImplementations(Type interfaceType)
//    {
//        var types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(assembly => assembly.GetTypes());
//        return types.Where(p => interfaceType.IsAssignableFrom(p) && !p.IsAbstract).ToArray();
//    }
//}