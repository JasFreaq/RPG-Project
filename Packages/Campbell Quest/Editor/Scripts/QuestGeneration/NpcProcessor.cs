using Campbell.Dialogues;
using Campbell.Editor.QuestGeneration.Utility;
using Campbell.Quests;
using System.Collections;
using System.Collections.Generic;
using Unity.Plastic.Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace Campbell.Editor.QuestGeneration
{
    public class NpcProcessor 
    {
        public void GenerateEnemies(Quest.QuestMetadata metadata, ref List<string> formattedEnemies)
        {
            if (GUILayout.Button("Generate Enemies"))
            {
                if (formattedEnemies.Count > 0)
                {
                    formattedEnemies.Clear();
                }

                string quest = UtilityLibrary.FormatStringForPython(metadata.formattedQuest);

                string requiredEnemiesSchema = UtilityLibrary.FormatStringForPython(Resources.Load<TextAsset>("campbellRequiredEnemiesSchema").text);
                string enemyTemplate = UtilityLibrary.FormatStringForPython(Resources.Load<TextAsset>("campbellEnemyTemplate").text);
                
                string enemyScript = "import UnityEngine;\n" +
                                     "from campbell_quest import enemy_generator\n" +
                                     "\n" +
                                     $"formatted_quest = \"{quest}\"\n" +
                                     $"required_enemies_schema = \"{requiredEnemiesSchema}\"\n" +
                                     $"enemy_template = \"{enemyTemplate}\"\n" +
                                     "enemies = enemy_generator.get_enemies(formatted_quest, required_enemies_schema, enemy_template)\n" +
                                     "for enemy in enemies:\n" +
                                     "\tprint(enemy)\n" +
                                     "\tprint(\"@\")\n";

                string enemies = UtilityLibrary.RunPythonScript(enemyScript);

                List<string> enemiesBuilder = new List<string>();
                formattedEnemies.AddRange(enemies.Split('@'));

                foreach (string dialogue in formattedEnemies)
                {
                    if (!string.IsNullOrWhiteSpace(dialogue))
                    {
                        enemiesBuilder.Add(dialogue);
                    }
                }

                formattedEnemies = enemiesBuilder;
                if (formattedEnemies.Count == 0)
                {
                    Debug.LogWarning("No enemies generated.");
                }
            }
        }

        public string DisplayEnemyInformation(string enemyJson)
        {
            EnemyData enemyData = UtilityLibrary.DeserializeJson<EnemyData>(enemyJson);

            // Start custom inspector
            EditorGUILayout.LabelField("Enemy Name", EditorStyles.boldLabel);
            enemyData.enemy_name = EditorGUILayout.TextField(enemyData.enemy_name);

            EditorGUILayout.LabelField("Objective Reference", EditorStyles.boldLabel);
            enemyData.objective_reference = EditorGUILayout.TextField(enemyData.objective_reference);
            
            // Apply changes
            if (GUI.changed)
            {
                enemyJson = JsonConvert.SerializeObject(enemyData);
            }

            return enemyJson;
        }

        public bool ClearEnemies(ref List<string> formattedEnemies)
        {
            if (GUILayout.Button("Clear Enemies"))
            {
                formattedEnemies.Clear();
                return true;
            }

            return false;
        }

        public void CreateEnemyAsset(string item, Quest questAsset, string npcAssetSavePath)
        {
            if (GUILayout.Button("Create Enemy Assets"))
            {
                NpcGenerator.CreateEnemyPrefab(item, questAsset, npcAssetSavePath);
            }
        }

        public void CreateNpcAsset(Dialogue dialogueAsset, Quest questAsset, string npcAssetSavePath)
        {
            if (GUILayout.Button("Create Npc Asset"))
            {
                NpcGenerator.CreateNpcPrefab(dialogueAsset, questAsset, npcAssetSavePath);
            }
        }

        public void RecreateEnemyAsset(string item, Quest questAsset, string npcAssetSavePath)
        {
            if (GUILayout.Button("Recreate Enemy Assets"))
            {
                NpcGenerator.CreateEnemyPrefab(item, questAsset, npcAssetSavePath);
            }
        }

        public void RecreateNpcAsset(Dialogue dialogueAsset, Quest questAsset, string npcAssetSavePath)
        {
            if (GUILayout.Button("Recreate Npc Asset"))
            {
                NpcGenerator.CreateNpcPrefab(dialogueAsset, questAsset, npcAssetSavePath);
            }
        }
    }
}