using Campbell.Quests;
using UnityEditor;
using UnityEngine;

namespace Campbell.Editor.QuestGeneration
{
    /// <summary>
    /// The QuestEditorWindow class provides a custom editor window for creating and managing quests.
    /// </summary>
    public class QuestEditorWindow : EditorWindow
    {
        /// <summary>
        /// The default path where quest assets are saved.
        /// </summary>
        public const string QuestAssetSavePath = "Assets/Campbell Generated Quests";

        private int _baseTab = 0;
        private int _previousFrameContextTab = 0;
        private int _contextTab = 0;
        private string[] _baseTabNames = { "Context Editor", "Quest Editor" };
        private string[] _contextEditorTabNames = { "Prompt", "Objectives", "Locations", "Characters", "Rewards" };
        private Vector2 _contextScrollPosition;
        private Vector2 _questScrollPosition;
        private QuestProcessor _questProcessor;

        /// <summary>
        /// Opens the Quest Editor Window from the Unity menu.
        /// </summary>
        [MenuItem("Campbell/Quest Editor Window", false, 0)]
        public static void ShowWindow()
        {
            GetWindow<QuestEditorWindow>("Campbell Quest Editor");
        }

        /// <summary>
        /// Initializes the QuestProcessor when the editor window is enabled.
        /// </summary>
        private void OnEnable()
        {
            _questProcessor = new QuestProcessor();
        }

        /// <summary>
        /// Handles the GUI rendering and logic for the editor window.
        /// </summary>
        private void OnGUI()
        {
            _baseTab = GUILayout.Toolbar(_baseTab, _baseTabNames);
            if (_baseTab == 0)
            {
                FormatContextWindow();
            }
            else if (_baseTab == 1)
            {
                FormatQuestWindow();
            }
        }

        /// <summary>
        /// Formats the context editing section of the editor window.
        /// </summary>
        private void FormatContextWindow()
        {
            _contextScrollPosition = EditorGUILayout.BeginScrollView(_contextScrollPosition, GUILayout.ExpandHeight(true));
            EditorGUILayout.Space();
            _questProcessor.PopulateFromSamples();
            EditorGUILayout.Space();
            _contextTab = GUILayout.Toolbar(_contextTab, _contextEditorTabNames);
            if (_contextTab != _previousFrameContextTab)
            {
                _previousFrameContextTab = _contextTab;
                GUI.FocusControl(null);
            }
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
            if (_questProcessor.GenerateQuest())
            {
                _baseTab = 1;
                GUI.FocusControl(null);
            }
            EditorGUILayout.EndScrollView();
        }

        /// <summary>
        /// Formats the quest editing section of the editor window.
        /// </summary>
        private void FormatQuestWindow()
        {
            if (!string.IsNullOrWhiteSpace(_questProcessor.formattedQuestWithRewards))
            {
                if (!_questProcessor.ClearQuest())
                {
                    EditorGUILayout.Space();
                    DisplayGeneratedQuest();
                    EditorGUILayout.Space();
                    if (QuestGenerator.DoesQuestAssetExist(_questProcessor.formattedQuestWithRewards, QuestAssetSavePath))
                    {
                        Quest.QuestMetadata metadata = new Quest.QuestMetadata
                        {
                            formattedQuest = _questProcessor.formattedQuest,
                            locationInformation = _questProcessor.locationInformation,
                            characterInformation = _questProcessor.characterInformation
                        };
                        _questProcessor.RecreateQuestAsset(metadata);
                    }
                    else
                    {
                        Quest.QuestMetadata metadata = new Quest.QuestMetadata
                        {
                            formattedQuest = _questProcessor.formattedQuest,
                            locationInformation = _questProcessor.locationInformation,
                            characterInformation = _questProcessor.characterInformation
                        };
                        _questProcessor.CreateQuestAsset(metadata);
                    }
                }
            }
            else
            {
                _questProcessor.GenerateQuest();
            }
        }

        /// <summary>
        /// Displays the information of the generated quest in the editor window.
        /// </summary>
        private void DisplayGeneratedQuest()
        {
            _questScrollPosition = EditorGUILayout.BeginScrollView(_questScrollPosition, GUILayout.ExpandHeight(true));
            _questProcessor.DisplayQuestInformation();
            EditorGUILayout.EndScrollView();
        }
    }
}
