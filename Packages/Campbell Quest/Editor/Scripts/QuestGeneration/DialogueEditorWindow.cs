using Campbell.Editor.QuestGeneration.Utility;
using Campbell.Quests;
using System.Collections;
using System.Collections.Generic;
using Unity.Plastic.Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace Campbell.Editor.QuestGeneration
{
    /// <summary>
    /// A custom editor window for managing and editing dialogues related to a quest.
    /// </summary>
    public class DialogueEditorWindow : EditorWindow
    {
        private List<string> _formattedDialogues = new List<string>();
        private int _dialogueTab = 0;
        private Vector2 _dialoguesScrollPosition;
        private DialogueProcessor _dialogueProcessor;
        private Quest _quest;

        /// <summary>
        /// Opens the Dialogue Editor Window from the Unity editor menu.
        /// </summary>
        [MenuItem("Campbell/Dialogue Editor Window", false, 1)]
        public static void ShowWindow()
        {
            GetWindow<DialogueEditorWindow>("Campbell Dialogue Editor");
        }

        /// <summary>
        /// Initializes the DialogueProcessor when the window is enabled.
        /// </summary>
        private void OnEnable()
        {
            _dialogueProcessor = new DialogueProcessor();
        }

        /// <summary>
        /// Renders the GUI elements of the Dialogue Editor Window.
        /// Handles quest asset selection and dialogue generation.
        /// </summary>
        private void OnGUI()
        {
            Rect rect = EditorGUILayout.GetControlRect();
            _quest = (Quest)EditorGUI.ObjectField(rect, "Quest Asset", _quest, typeof(Quest), false);

            if (_quest == null)
            {
                if (_formattedDialogues.Count > 0)
                {
                    _formattedDialogues.Clear();
                }
                EditorGUILayout.HelpBox("Missing Quest Asset.", MessageType.Info);
            }
            else
            {
                Quest.QuestMetadata metadata = _quest.Metadata;
                _dialogueProcessor.GenerateDialogues(metadata.formattedQuest, metadata.locationInformation, metadata.characterInformation, ref _formattedDialogues);

                if (_formattedDialogues.Count > 0)
                {
                    FormatDialogueWindow();
                }
            }
        }

        /// <summary>
        /// Formats and displays the dialogue window with tabs for each dialogue entry.
        /// Handles dialogue asset creation and recreation.
        /// </summary>
        private void FormatDialogueWindow()
        {
            if (_dialogueProcessor.ClearDialogues(ref _formattedDialogues))
            {
                _dialogueTab = 0;
                return;
            }

            EditorGUILayout.Space();
            Quest.QuestMetadata metadata = _quest.Metadata;
            string[] dialogueTabNames = new string[_formattedDialogues.Count];

            for (int i = 0; i < _formattedDialogues.Count; i++)
            {
                DialogueData dialogueData = UtilityLibrary.DeserializeJson<DialogueData>(_formattedDialogues[i]);
                if (dialogueData == null)
                {
                    _dialogueProcessor.GenerateDialogues(metadata.formattedQuest, metadata.locationInformation, metadata.characterInformation, ref _formattedDialogues);
                    _formattedDialogues.Clear();
                    return;
                }
                dialogueTabNames[i] = $"{dialogueData.npc_name}";
            }

            _dialogueTab = GUILayout.Toolbar(_dialogueTab, dialogueTabNames);
            DisplayGeneratedDialogues();
            EditorGUILayout.Space();

            string questName = UtilityLibrary.DeserializeJson<QuestData>(metadata.formattedQuest).name;
            string dialogueAssetSavePath = QuestEditorWindow.QuestAssetSavePath + "/" + questName;

            if (DialogueGenerator.DoesDialogueAssetExist(_formattedDialogues[_dialogueTab], dialogueAssetSavePath))
            {
                _dialogueProcessor.RecreateDialogueAsset(_formattedDialogues[_dialogueTab], dialogueAssetSavePath);
            }
            else
            {
                _dialogueProcessor.CreateDialogueAsset(_formattedDialogues[_dialogueTab], dialogueAssetSavePath);
            }
        }

        /// <summary>
        /// Displays the generated dialogues in a scrollable view.
        /// </summary>
        private void DisplayGeneratedDialogues()
        {
            _dialoguesScrollPosition = EditorGUILayout.BeginScrollView(_dialoguesScrollPosition, GUILayout.ExpandHeight(true));
            _formattedDialogues[_dialogueTab] = _dialogueProcessor.DisplayDialogueInformation(_formattedDialogues[_dialogueTab]);
            EditorGUILayout.EndScrollView();
        }
    }
}
