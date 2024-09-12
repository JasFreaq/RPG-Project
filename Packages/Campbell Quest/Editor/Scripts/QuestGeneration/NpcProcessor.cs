using Campbell.Dialogues;
using Campbell.Editor.QuestGeneration.Utility;
using Campbell.Quests;
using System.Collections.Generic;
using Unity.Plastic.Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace Campbell.Editor.QuestGeneration
{
    /// <summary>
    /// Class responsible for processing NPCs and generating related assets in the quest generation context.
    /// </summary>
    public class NpcProcessor
    {
        /// <summary>
        /// The selected model type for enemy generation.
        /// </summary>
        public ModelType selectedModel = 0;

        /// <summary>
        /// Generates enemies based on the provided quest metadata and updates the formatted enemies list.
        /// </summary>
        /// <param name="metadata">Metadata of the quest used for generating enemies.</param>
        /// <param name="formattedEnemies">Reference to a list of strings that will store formatted enemy data.</param>
        public void GenerateEnemies(Quest.QuestMetadata metadata, ref List<string> formattedEnemies)
        {
            selectedModel = (ModelType)EditorGUILayout.EnumPopup("Model", selectedModel);
            if (GUILayout.Button("Generate Enemies"))
            {
                if (formattedEnemies.Count > 0)
                {
                    formattedEnemies.Clear();
                }
                string quest = UtilityLibrary.FormatStringForPython(metadata.formattedQuest);
                string requiredEnemiesSchema = UtilityLibrary.FormatStringForPython(Resources.Load<TextAsset>("campbellRequiredEnemiesSchema").text);
                string enemyTemplate = UtilityLibrary.FormatStringForPython(Resources.Load<TextAsset>("campbellEnemyTemplate").text);
                string model = UtilityLibrary.GetModelString(selectedModel);
                string enemyScript = "import UnityEngine;\n" +
                                     "from campbell_quest import enemy_generator\n" +
                                     "\n" +
                                     $"formatted_quest = \"{quest}\"\n" +
                                     $"required_enemies_schema = \"{requiredEnemiesSchema}\"\n" +
                                     $"enemy_template = \"{enemyTemplate}\"\n" +
                                     $"model = \"{model}\"\n" +
                                     "enemies = enemy_generator.get_enemies(formatted_quest, required_enemies_schema, enemy_template, model)\n" +
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

        /// <summary>
        /// Displays and allows editing of enemy information from a JSON string.
        /// </summary>
        /// <param name="enemyJson">JSON string containing enemy data.</param>
        /// <returns>Modified JSON string after applying any changes.</returns>
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

        /// <summary>
        /// Clears the list of formatted enemies if the respective button is pressed.
        /// </summary>
        /// <param name="formattedEnemies">Reference to a list of strings that stores formatted enemy data.</param>
        /// <returns>True if enemies were cleared, otherwise false.</returns>
        public bool ClearEnemies(ref List<string> formattedEnemies)
        {
            if (GUILayout.Button("Clear Enemies"))
            {
                formattedEnemies.Clear();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Creates an enemy asset using the provided item, quest asset, and save path.
        /// </summary>
        /// <param name="item">The item used to generate the enemy asset.</param>
        /// <param name="questAsset">The quest asset which the enemy belongs to.</param>
        /// <param name="npcAssetSavePath">The path where the enemy asset should be saved.</param>
        public void CreateEnemyAsset(string item, Quest questAsset, string npcAssetSavePath)
        {
            if (GUILayout.Button("Create Enemy Assets"))
            {
                NpcGenerator.CreateEnemyPrefab(item, questAsset, npcAssetSavePath);
            }
        }

        /// <summary>
        /// Creates an NPC asset using the provided dialogue asset, quest asset, and save path.
        /// </summary>
        /// <param name="dialogueAsset">The dialogue asset associated with the NPC.</param>
        /// <param name="questAsset">The quest asset which the NPC belongs to.</param>
        /// <param name="npcAssetSavePath">The path where the NPC asset should be saved.</param>
        public void CreateNpcAsset(Dialogue dialogueAsset, Quest questAsset, string npcAssetSavePath)
        {
            if (GUILayout.Button("Create Npc Asset"))
            {
                NpcGenerator.CreateNpcPrefab(dialogueAsset, questAsset, npcAssetSavePath);
            }
        }

        /// <summary>
        /// Recreates an enemy asset using the provided item, quest asset, and save path.
        /// </summary>
        /// <param name="item">The item used to regenerate the enemy asset.</param>
        /// <param name="questAsset">The quest asset which the enemy belongs to.</param>
        /// <param name="npcAssetSavePath">The path where the enemy asset should be saved.</param>
        public void RecreateEnemyAsset(string item, Quest questAsset, string npcAssetSavePath)
        {
            if (GUILayout.Button("Recreate Enemy Assets"))
            {
                NpcGenerator.CreateEnemyPrefab(item, questAsset, npcAssetSavePath);
            }
        }

        /// <summary>
        /// Recreates an NPC asset using the provided dialogue asset, quest asset, and save path.
        /// </summary>
        /// <param name="dialogueAsset">The dialogue asset associated with the NPC.</param>
        /// <param name="questAsset">The quest asset which the NPC belongs to.</param>
        /// <param name="npcAssetSavePath">The path where the NPC asset should be saved.</param>
        public void RecreateNpcAsset(Dialogue dialogueAsset, Quest questAsset, string npcAssetSavePath)
        {
            if (GUILayout.Button("Recreate Npc Asset"))
            {
                NpcGenerator.CreateNpcPrefab(dialogueAsset, questAsset, npcAssetSavePath);
            }
        }
    }
}
