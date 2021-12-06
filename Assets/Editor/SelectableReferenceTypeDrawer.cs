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
        if (property.propertyType != SerializedPropertyType.ManagedReference)
            return;

        // Populate the list of possible types, but only if it hasn't been done already.
        if (possibleTypes == null)
            possibleTypes = GetDerivedTypes((SelectableReferenceType)attribute);
        string[] possibleTypeNames = possibleTypes.Select(type => type == null ? "None" : type.ToString()).ToArray(); // The list of plain English names.

        // Obtain the current type and its index.
        // This is surprisingly difficult, since the return value from EditorGUI.Popup is not always updated instantly.
        // Searching through the array of names to find a match is also tricky, because the only way to obtain a type name from the property returns a string with a whole lot of extra baggage.
        int currentTypeIndex = property.managedReferenceFullTypename == "" ? 0 : Array.IndexOf(possibleTypeNames, property.managedReferenceFullTypename.Split(' ')[1]);
        Type currentObjectType = possibleTypes[currentTypeIndex];

        // Display the popup allowing the user to select a new type.
        Rect popupPosition = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        int popupTypeIndex = EditorGUI.Popup(popupPosition, currentTypeIndex, possibleTypeNames);

        // Update the actual managed reference object.
        if (popupTypeIndex >= 0 && popupTypeIndex < possibleTypes.Count)
        {
            if (currentObjectType != possibleTypes[popupTypeIndex])
            {
                property.managedReferenceValue = possibleTypes[popupTypeIndex] == null ?
                    null :
                    Activator.CreateInstance(possibleTypes[popupTypeIndex]);
                currentObjectType = possibleTypes[popupTypeIndex];
            }
        }

        // Display the object's data for editing.
        EditorGUI.PropertyField(position, property, true);
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, true);
    }

    public static List<Type> GetDerivedTypes(SelectableReferenceType parent)
    {
        List<Type> possibleTypes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(assembly => assembly.GetTypes())
            .Where(type => parent.Type.IsAssignableFrom(type) && !type.IsAbstract)
            .ToList();
        possibleTypes.Insert(0, null);

        return possibleTypes;
    }
}