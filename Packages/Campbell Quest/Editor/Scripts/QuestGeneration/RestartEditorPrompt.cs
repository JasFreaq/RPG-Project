using UnityEditor;
using UnityEngine;
using System.IO;

namespace Campbell.Editor.QuestGeneration
{
    public class RestartEditorPrompt : EditorWindow
    {
        public static void ShowWindow()
        {
            RestartEditorPrompt window =
                (RestartEditorPrompt)GetWindow(typeof(RestartEditorPrompt), true, "Campbell Quest");
            window.Show();
        }

        private void OnGUI()
        {
            GUIContent message = new GUIContent("The Unity Editor needs to be restarted to install package dependencies.\nDo you want to restart the Unity Editor?");
            Vector2 messageSize = EditorStyles.boldLabel.CalcSize(message);

            GUILayout.BeginVertical(GUILayout.Width(messageSize.x), GUILayout.Height(messageSize.y + EditorGUIUtility.singleLineHeight * 2));

            GUILayout.Label(message, EditorStyles.boldLabel);

            if (GUILayout.Button("Restart"))
            {
                RestartEditor();
            }

            if (GUILayout.Button("Cancel"))
            {
                Close();
            }

            GUILayout.EndVertical();
        }

        private static void RestartEditor()
        {
            EditorApplication.OpenProject(Directory.GetCurrentDirectory());
        }
    }
}