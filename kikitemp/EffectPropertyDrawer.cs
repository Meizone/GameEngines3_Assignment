using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Effect)), CanEditMultipleObjects]
public class EffectPropertyDrawer : Editor
{
    public SerializedProperty
        type,
        //description,
        chance;
        //amount;

    void OnEnable()
    {
        type = serializedObject.FindProperty("type");
        //description = serializedObject.FindProperty("description");
        chance = serializedObject.FindProperty("chance");
        //amount = serializedObject.FindProperty("amount");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(type);

        //EditorGUILayout.PropertyField(description, new GUIContent("description"));

        Effect.Type switcher = (Effect.Type)type.enumValueIndex;
        switch (switcher)
        {
            case Effect.Type.Passive:
                break;
            case Effect.Type.OnActivation:
                break;
            case Effect.Type.Chance:
                EditorGUILayout.FloatField(chance.displayName, chance.floatValue);
                break;
            case Effect.Type.AffectResource:
                break;
            default:
                break;
        }

        serializedObject.ApplyModifiedProperties();
    }
}