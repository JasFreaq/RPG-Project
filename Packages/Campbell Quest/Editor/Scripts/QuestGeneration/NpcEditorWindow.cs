using Campbell.Quests;
using System.Collections.Generic;
using Campbell.Dialogues;
using Campbell.Editor.QuestGeneration.Utility;
using UnityEditor;
using UnityEngine;

namespace Campbell.Editor.QuestGeneration
{
    /// <summary>
    /// Represents the editor window for managing NPCs within the Campbell Quest Generation system.
    /// </summary>
    public class NpcEditorWindow : EditorWindow
    {
        private List<string> _formattedEnemies = new List<string>();
        private Vector2 _enemiesScrollPosition;
        private int _enemyTab = 0;
        private NpcProcessor _npcProcessor;
        private Quest _quest;
        private Dialogue _dialogue;

        /// <summary>
        /// Displays the NPC Editor Window within the Unity Editor.
        /// </summary>
        [MenuItem("Campbell/Npc Editor Window", false, 3)]
        public static void ShowWindow()
        {
            GetWindow<NpcEditorWindow>("Campbell Npc Editor");
        }

        /// <summary>
        /// Initializes necessary components when the editor window is enabled.
        /// </summary>
        private void OnEnable()
        {
            _npcProcessor = new NpcProcessor();
        }

        /// <summary>
        /// Renders the GUI elements of the NPC Editor Window.
        /// </summary>
        private void OnGUI()
        {
            Rect rect = EditorGUILayout.GetControlRect();
            _quest = (Quest)EditorGUI.ObjectField(rect, "Quest Asset", _quest, typeof(Quest), false);
            rect = EditorGUILayout.GetControlRect();
            _dialogue = (Dialogue)EditorGUI.ObjectField(rect, "Dialogue Asset", _dialogue, typeof(Dialogue), false);

            if (_quest == null)
            {
                if (_formattedEnemies.Count > 0)
                {
                    _formattedEnemies.Clear();
                }
                EditorGUILayout.HelpBox("Missing Quest Asset.", MessageType.Info);
            }

            if (_dialogue == null)
            {
                EditorGUILayout.HelpBox("Missing Dialogue Asset.", MessageType.Info);
            }

            if (_quest != null)
            {
                Quest.QuestMetadata metadata = _quest.Metadata;
                string questName = UtilityLibrary.DeserializeJson<QuestData>(metadata.formattedQuest).name;
                string npcAssetSavePath = QuestEditorWindow.QuestAssetSavePath + "/" + questName;
                _npcProcessor.GenerateEnemies(metadata, ref _formattedEnemies);

                if (_formattedEnemies.Count > 0)
                {
                    FormatEnemyWindow(npcAssetSavePath);
                }

                if (_dialogue != null)
                {
                    if (NpcGenerator.DoesNpcAssetExist(_dialogue.name, npcAssetSavePath))
                    {
                        _npcProcessor.RecreateNpcAsset(_dialogue, _quest, npcAssetSavePath);
                    }
                    else
                    {
                        _npcProcessor.CreateNpcAsset(_dialogue, _quest, npcAssetSavePath);
                    }
                }
            }
        }

        /// <summary>
        /// Formats and displays the enemy window for generated enemies.
        /// </summary>
        /// <param name="npcAssetSavePath">The path where the NPC assets are saved.</param>
        private void FormatEnemyWindow(string npcAssetSavePath)
        {
            if (_npcProcessor.ClearEnemies(ref _formattedEnemies))
            {
                _enemyTab = 0;
                return;
            }

            EditorGUILayout.Space();
            string[] enemyTabNames = new string[_formattedEnemies.Count];
            for (int i = 0; i < _formattedEnemies.Count; i++)
            {
                EnemyData enemyData = UtilityLibrary.DeserializeJson<EnemyData>(_formattedEnemies[i]);
                enemyTabNames[i] = $"{enemyData.enemy_name}";
            }

            _enemyTab = GUILayout.Toolbar(_enemyTab, enemyTabNames);
            DisplayGeneratedEnemies();
            EditorGUILayout.Space();

            EnemyData enemy = UtilityLibrary.DeserializeJson<EnemyData>(_formattedEnemies[_enemyTab]);
            if (NpcGenerator.DoesNpcAssetExist(enemy.enemy_name, npcAssetSavePath))
            {
                _npcProcessor.RecreateEnemyAsset(_formattedEnemies[_enemyTab], _quest, npcAssetSavePath);
            }
            else
            {
                _npcProcessor.CreateEnemyAsset(_formattedEnemies[_enemyTab], _quest, npcAssetSavePath);
            }
        }

        /// <summary>
        /// Displays information for generated enemies in a scrollable view.
        /// </summary>
        private void DisplayGeneratedEnemies()
        {
            _enemiesScrollPosition = EditorGUILayout.BeginScrollView(_enemiesScrollPosition, GUILayout.ExpandHeight(true));
            _formattedEnemies[_enemyTab] = _npcProcessor.DisplayEnemyInformation(_formattedEnemies[_enemyTab]);
            EditorGUILayout.EndScrollView();
        }
    }
}
