using System.Collections.Generic;
using System.Reflection;
using Campbell.Quests;
using Unity.Plastic.Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace Campbell.Editor.QuestGeneration
{
    public class QuestEditorWindow : EditorWindow
    {
        public const string QuestAssetSavePath = "Assets/Campbell Generated Quests";

        private int _baseTab = 0;
        private int _contextTab = 0;

        private string[] _baseTabNames = { "Context Editor", "Quest Editor" };
        private string[] _contextEditorTabNames = { "Prompt", "Objectives", "Locations", "Characters", "Rewards" };

        private string _formattedQuest;
        private string _formattedQuestWithRewards;

        private Vector2 _contextScrollPosition;
        private Vector2 _questScrollPosition;

        private QuestProcessor _questProcessor;
        
        [MenuItem("Campbell/Quest Editor Window", false, 0)]
        public static void ShowWindow()
        {
            GetWindow<QuestEditorWindow>("Campbell Quest Editor");
        }

        private void OnEnable()
        {
            _questProcessor = new QuestProcessor();
        }

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

        private void FormatContextWindow()
        {
            _contextScrollPosition = EditorGUILayout.BeginScrollView(_contextScrollPosition, GUILayout.ExpandHeight(true));

            EditorGUILayout.Space();

            _questProcessor.PopulateFromSamples();

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
                if (!_questProcessor.ClearQuest(ref _formattedQuestWithRewards))
                {
                    EditorGUILayout.Space();

                    DisplayGeneratedQuest();

                    EditorGUILayout.Space();

                    if (QuestGenerator.DoesQuestAssetExist(_formattedQuestWithRewards, QuestAssetSavePath))
                    {
                        _questProcessor.RecreateQuestAsset(_formattedQuestWithRewards, new Quest.QuestMetadata
                            {
                                formattedQuest = _formattedQuest,
                                locationInformation = _questProcessor.LocationInformation,
                                characterInformation = _questProcessor.CharacterInformation
                            });
                    }
                    else
                    {
                        _questProcessor.CreateQuestAsset(_formattedQuestWithRewards, new Quest.QuestMetadata
                            {
                                formattedQuest = _formattedQuest,
                                locationInformation = _questProcessor.LocationInformation,
                                characterInformation = _questProcessor.CharacterInformation
                            });
                    }
                }
            }
            else
            {
                _questProcessor.GenerateQuest(ref _formattedQuest, ref _formattedQuestWithRewards);
            }
        }

        private void DisplayGeneratedQuest()
        {
            _questScrollPosition = EditorGUILayout.BeginScrollView(_questScrollPosition, GUILayout.ExpandHeight(true));

            _formattedQuestWithRewards = _questProcessor.DisplayQuestInformation(_formattedQuestWithRewards);

            EditorGUILayout.EndScrollView();
        }
    }
}
