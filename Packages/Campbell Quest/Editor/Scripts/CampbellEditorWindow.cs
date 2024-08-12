using RPG.Quests.Editor;
using System.Collections.Generic;
using Unity.Plastic.Newtonsoft.Json;
using UnityEditor;
using UnityEngine;
using static RPG.Quests.Editor.AssetGenerator;

namespace Campbell.Editor
{
    public class CampbellEditorWindow : EditorWindow
    {
        private const string _sampleFilesPath = "Packages/Campbell Quest/Context Samples";
        private const string _questAssetSavePath = "Assets/Campbell Generated Quests/Resources";

        private int _baseTab = 0;
        private int _contextTab = 0;
        private int _dialogueTab = 0;

        private string[] _baseTabNames = { "Context Editor", "Quest Editor" };
        private string[] _contextEditorTabNames = { "Prompt", "Objectives", "Locations", "Characters", "Rewards" };

        private string _formattedQuest;
        private string _formattedQuestWithRewards;
        private List<string> _formattedDialogues = new List<string>();
        private string _generatedQuestName;

        private Vector2 _contextScrollPosition;
        private Vector2 _questScrollPosition;
        private Vector2 _dialoguesScrollPosition;

        private QuestProcessor _questProcessor;
        private DialogueProcessor _dialogueProcessor;

        [MenuItem("Campbell/Campbell Editor Window")]
        public static void ShowWindow()
        {
            GetWindow<CampbellEditorWindow>("Campbell Editor");
        }

        private void OnEnable()
        {
            _questProcessor = new QuestProcessor();
            _dialogueProcessor = new DialogueProcessor();
        }

        private void OnGUI()
        {
            string[] tabNames = _baseTabNames;
            if (_formattedDialogues.Count > 0)
            {
                tabNames = new string[_baseTabNames.Length + 1];
                _baseTabNames.CopyTo(tabNames, 0);
                tabNames[_baseTabNames.Length] = "Dialogue Editor";
            }

            _baseTab = GUILayout.Toolbar(_baseTab, tabNames);

            if (_baseTab == 0)
            {
                FormatContextWindow();
            }
            else if (_baseTab == 1)
            {
                FormatQuestWindow();
            }
            else if (_baseTab == 2)
            {
                FormatDialogueWindow();
            }
        }

        private void FormatContextWindow()
        {
            _contextScrollPosition = EditorGUILayout.BeginScrollView(_contextScrollPosition, GUILayout.ExpandHeight(true));

            EditorGUILayout.Space();

            _questProcessor.PopulateFromSamples(_sampleFilesPath);

            EditorGUILayout.Space();

            _contextTab = GUILayout.Toolbar(_contextTab, _contextEditorTabNames);

            if (_contextTab == 0)
            {
                _questProcessor.DisplayQuestPrompt();
            }
            else if (_contextTab == 1)
            {
                _questProcessor.DisplayObjectiveInformation();
            }
            else if (_contextTab == 2)
            {
                _questProcessor.DisplayLocationInformation();
            }
            else if (_contextTab == 3)
            {
                _questProcessor.DisplayCharacterInformation();
            }
            else if (_contextTab == 4)
            {
                _questProcessor.DisplayRewardInformation();
            }

            EditorGUILayout.Space();
            
            if (_questProcessor.GenerateQuest(ref _formattedQuest, ref _formattedQuestWithRewards))
            {
                _baseTab = 1;
            }

            EditorGUILayout.EndScrollView();
        }

        private void FormatQuestWindow()
        {
            if (!string.IsNullOrWhiteSpace(_formattedQuestWithRewards))
            {
                _questProcessor.ClearQuest(ref _formattedQuestWithRewards);

                EditorGUILayout.Space();

                DisplayGeneratedQuest();

                EditorGUILayout.Space();

                if (DoesQuestAssetExist(_formattedQuestWithRewards, _questAssetSavePath))
                {
                    _questProcessor.RecreateQuestAssets(_formattedQuestWithRewards, _questAssetSavePath, ref _generatedQuestName);

                    EditorGUILayout.Space();

                    if (_dialogueProcessor.GenerateDialogues(_formattedQuestWithRewards, _questProcessor.LocationInformation, _questProcessor.CharacterInformation, ref _formattedDialogues))
                    {
                        _baseTab = 2;
                    }
                }
                else
                {
                    _questProcessor.CreateQuestAssets(_formattedQuestWithRewards, _questAssetSavePath, ref _generatedQuestName);
                }
            }
            else
            {
                _questProcessor.GenerateQuest(ref _formattedQuest, ref _formattedQuestWithRewards);
            }
        }

        private void FormatDialogueWindow()
        {
            if (_dialogueProcessor.ClearDialogues(ref _formattedDialogues))
            {
                _dialogueTab = 0;
                _baseTab = 1;
            }

            EditorGUILayout.Space();

            string[] dialogueTabNames = new string[_formattedDialogues.Count];
            for (int i = 0; i < _formattedDialogues.Count; i++)
            {
                DialogueData dialogueData = JsonConvert.DeserializeObject<DialogueData>(_formattedDialogues[i]);
                dialogueTabNames[i] = $"{dialogueData.npc_name}";
            }

            _dialogueTab = GUILayout.Toolbar(_dialogueTab, dialogueTabNames);
            
            DisplayGeneratedDialogues();

            if (DoesDialogueAssetExist(_formattedDialogues[_dialogueTab], _questAssetSavePath, _generatedQuestName))
            {
                _dialogueProcessor.RecreateDialogueAssets(_formattedDialogues[_dialogueTab], _questAssetSavePath, _generatedQuestName);
            }
            else
            {
                _dialogueProcessor.CreateDialogueAssets(_formattedDialogues[_dialogueTab], _questAssetSavePath, _generatedQuestName);
            }
        }

        private void DisplayGeneratedQuest()
        {
            GUIStyle textStyle = new GUIStyle(EditorStyles.textField)
            {
                padding = new RectOffset(5, 5, 5, 5),
                wordWrap = true
            };

            _questScrollPosition = EditorGUILayout.BeginScrollView(_questScrollPosition, GUILayout.ExpandHeight(true));

            _formattedQuestWithRewards = EditorGUILayout.TextArea(_formattedQuestWithRewards, textStyle, GUILayout.MinWidth(100),
                GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));

            EditorGUILayout.EndScrollView();
        }

        private void DisplayGeneratedDialogues()
        {
            GUIStyle textStyle = new GUIStyle(EditorStyles.textField)
            {
                padding = new RectOffset(5, 5, 5, 5),
                wordWrap = true
            };

            _dialoguesScrollPosition = EditorGUILayout.BeginScrollView(_dialoguesScrollPosition, GUILayout.ExpandHeight(true));

            _formattedDialogues[_dialogueTab] = EditorGUILayout.TextArea(_formattedDialogues[_dialogueTab], textStyle, GUILayout.MinWidth(100),
                GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));

            EditorGUILayout.Space();

            EditorGUILayout.EndScrollView();
        }
    }
}
