using Campbell.Dialogues;
using UnityEditor;

/// <summary>
/// Custom editor for the DialogueTrigger component.
/// </summary>
[CustomEditor(typeof(DialogueTrigger))]
public class DialogueTriggerEditor : Editor
{
    /// <summary>
    /// Serialized property representing the triggers in the DialogueTrigger component.
    /// </summary>
    private SerializedProperty _triggersProperty;

    /// <summary>
    /// Initializes the serialized properties when the editor is enabled.
    /// </summary>
    private void OnEnable()
    {
        _triggersProperty = serializedObject.FindProperty("_triggers");
    }

    /// <summary>
    /// Overrides the default inspector GUI to provide a custom interface for the DialogueTrigger component.
    /// </summary>
    public override void OnInspectorGUI()
    {
        // Update the serialized object to get the latest data from the target object.
        serializedObject.Update();

        // Draw the property field for the triggers, allowing for nested properties to be edited.
        EditorGUILayout.PropertyField(_triggersProperty, true);

        // Apply any changes made to the serialized properties back to the target object.
        serializedObject.ApplyModifiedProperties();
    }
}