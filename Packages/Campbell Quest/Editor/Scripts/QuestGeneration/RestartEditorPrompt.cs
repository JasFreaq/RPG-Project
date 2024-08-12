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
                (RestartEditorPrompt)GetWindow(typeof(RestartEditorPrompt), true, "Example Package");
            window.Show();
        }

        private void OnGUI()
        {
            GUILayout.Label(
                "The Unity Editor needs to be restarted to install package dependencies.\nDo you want to restart the Unity Editor?",
                EditorStyles.boldLabel);
            if (GUILayout.Button("Restart"))
            {
                RestartEditor();
            }

            if (GUILayout.Button("Cancel"))
            {
                Close();
            }
        }

        private static void RestartEditor()
        {
            EditorApplication.OpenProject(Directory.GetCurrentDirectory());
        }
    }
}