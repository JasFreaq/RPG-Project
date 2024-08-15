using Campbell.Dialogues;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DialogueTrigger))]
public class DialogueTriggerEditor : Editor
{
    private SerializedProperty _triggersProperty;

    private void OnEnable()
    {
        _triggersProperty = serializedObject.FindProperty("_triggers");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(_triggersProperty, true);

        serializedObject.ApplyModifiedProperties();
    }
}