using System;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace RPG.Dialogue.Editor
{
    public class DialogueEditor : EditorWindow
    {
        private Dialogue _selectedDialogue = null;
        private GUIStyle _nodeStyle;

        [MenuItem("Window/Dialogue Editor")]
        public static void ShowEditorWindow()
        {
            GetWindow(typeof(DialogueEditor), false, "Dialogue Editor");
        }

        [OnOpenAsset(1)]
        public static bool OnOpenDialogue(int instanceID, int line)
        {
            if (EditorUtility.InstanceIDToObject(instanceID) is Dialogue)
            {
                ShowEditorWindow();
                return true;
            }

            return false;
        }

        private void OnEnable()
        {
            Selection.selectionChanged += OnDialogueSelected;
            OnDialogueSelected();

            _nodeStyle = new GUIStyle();
            _nodeStyle.normal.background = Texture2D.grayTexture;
        }
        
        private void OnGUI()
        {
            if (_selectedDialogue)
            {
                foreach (DialogueNode node in _selectedDialogue.DialogueNodes)
                {
                    OnGUINode(node);
                }
            }
            else
            {
                EditorGUILayout.LabelField("No Dialogue asset selected.");
            }
        }
        
        private void OnDisable()
        {
            Selection.selectionChanged -= OnDialogueSelected;
        }

        private void OnGUINode(DialogueNode node)
        {
            GUILayout.BeginArea(node.positionRect, _nodeStyle);
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.LabelField("Node:");

            string newID = EditorGUILayout.TextField(node.NodeID);
            string newText = EditorGUILayout.TextField(node.Text);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(_selectedDialogue, "Dialogue Text Edit");

                node.NodeID = newID;
                node.Text = newText;
            }
            GUILayout.EndArea();
        }

        private void OnDialogueSelected()
        {
            Dialogue tempDialogue = Selection.activeObject as Dialogue;
            if (tempDialogue)
            {
                _selectedDialogue = tempDialogue;
                Repaint();
            }
        }
    }
}