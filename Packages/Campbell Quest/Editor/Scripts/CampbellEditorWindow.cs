using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Campbell.Editor
{
    public class CampbellEditorWindow : EditorWindow
    {
        private const string _sampleFilesPath = "Packages/Campbell Quest/Context Samples";
        private const string _questAssetSavePath = "Assets/Campbell Generated Quests/Resources";

        private int _selectedTab = 0;
        private string[] _initialTabNames = { "Context Editor", "Quest Editor" };

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
            string[] tabNames = _initialTabNames;
            if (_formattedDialogues.Count > 0)
            {
                tabNames = new string[_initialTabNames.Length + 1];
                _initialTabNames.CopyTo(tabNames, 0);
                tabNames[_initialTabNames.Length] = "Dialogue Editor";
            }

            _selectedTab = GUILayout.Toolbar(_selectedTab, tabNames);

            if (_selectedTab == 0)
            {
                FormatContextWindow();
            }
            else if (_selectedTab == 1)
            {
                FormatQuestWindow();
            }
            else if (_selectedTab == 2)
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

            _questProcessor.DisplayQuestPrompt();

            EditorGUILayout.Space();

            _questProcessor.DisplayObjectiveInformation();

            EditorGUILayout.Space();

            _questProcessor.DisplayLocationInformation();

            EditorGUILayout.Space();

            _questProcessor.DisplayCharacterInformation();

            EditorGUILayout.Space();

            _questProcessor.DisplayRewardInformation();

            EditorGUILayout.Space();

            if (_questProcessor.GenerateQuest(ref _formattedQuest, ref _formattedQuestWithRewards))
            {
                _selectedTab = 1;
            }

            EditorGUILayout.EndScrollView();
        }

        private void FormatQuestWindow()
        {
            if (!string.IsNullOrWhiteSpace(_formattedQuestWithRewards))
            {
                _questProcessor.ClearQuest(ref _formattedQuestWithRewards);
            }
            else
            {
                _questProcessor.GenerateQuest(ref _formattedQuest, ref _formattedQuestWithRewards);
            }

            EditorGUILayout.Space();

            DisplayGeneratedQuest();

            if (!string.IsNullOrWhiteSpace(_formattedQuestWithRewards))
            {
                EditorGUILayout.Space();

                EditorGUILayout.BeginHorizontal();

                _questProcessor.CreateQuestAssets(_formattedQuestWithRewards, _questAssetSavePath, ref _generatedQuestName);

                EditorGUILayout.Space();

                _dialogueProcessor.GenerateDialogues(_formattedQuestWithRewards, _questProcessor.LocationInformation, _questProcessor.CharacterInformation, ref _formattedDialogues);

                EditorGUILayout.EndHorizontal();
            }
        }

        private void FormatDialogueWindow()
        {
            if (_formattedDialogues.Count > 0)
            {
                _dialogueProcessor.ClearDialogues(ref _formattedDialogues);
            }
            else
            {
                _dialogueProcessor.GenerateDialogues(_formattedQuestWithRewards, _questProcessor.LocationInformation, _questProcessor.CharacterInformation, ref _formattedDialogues);
            }

            EditorGUILayout.Space();

            DisplayGeneratedDialogues();

            if (_formattedDialogues.Count > 0)
            {
                EditorGUILayout.Space();

                _dialogueProcessor.CreateDialogueAssets(_formattedDialogues, _questAssetSavePath, _generatedQuestName);
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

            for (int i = 0; i < _formattedDialogues.Count; i++)
            {
                _formattedDialogues[i] = EditorGUILayout.TextArea(_formattedDialogues[i], textStyle, GUILayout.MinWidth(100),
                    GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));

                EditorGUILayout.Space();
            }

            EditorGUILayout.EndScrollView();
        }
    }
}
