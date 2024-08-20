using Campbell.Quests;
using System.Collections;
using System.Collections.Generic;
using Campbell.Dialogues;
using Campbell.Editor.QuestGeneration.Utility;
using UnityEditor;
using UnityEngine;

namespace Campbell.Editor.QuestGeneration
{
    public class NpcEditorWindow : EditorWindow
    {
        private List<string> _formattedEnemies = new List<string>();

        private Vector2 _enemiesScrollPosition;

        private int _enemyTab = 0;

        private NpcProcessor _npcProcessor;

        private Quest _quest;

        private Dialogue _dialogue;

        [MenuItem("Campbell/Npc Editor Window", false, 3)]
        public static void ShowWindow()
        {
            GetWindow<NpcEditorWindow>("Campbell Npc Editor");
        }

        private void OnEnable()
        {
            _npcProcessor = new NpcProcessor();
        }

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
                    DialogueData dialogueData = UtilityLibrary.DeserializeJson<DialogueData>(_dialogue.RawDialogueJson);
                    if (dialogueData != null)
                    {
                        if (NpcGenerator.DoesNpcAssetExist(dialogueData.npc_name, npcAssetSavePath))
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
        }

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

        private void DisplayGeneratedEnemies()
        {
            _enemiesScrollPosition = EditorGUILayout.BeginScrollView(_enemiesScrollPosition, GUILayout.ExpandHeight(true));

            _formattedEnemies[_enemyTab] = _npcProcessor.DisplayEnemyInformation(_formattedEnemies[_enemyTab]);

            EditorGUILayout.EndScrollView();
        }
    }
}