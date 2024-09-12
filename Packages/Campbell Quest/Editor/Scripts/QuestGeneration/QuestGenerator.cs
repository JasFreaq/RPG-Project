using Campbell.Editor.QuestGeneration.Utility;
using Campbell.InventorySystem;
using Campbell.Quests;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Campbell.Editor.QuestGeneration
{
    /// <summary>
    /// Class for generating and managing quest assets based on JSON data.
    /// </summary>
    public class QuestGenerator
    {
        /// <summary>
        /// Checks if a quest asset already exists at the specified save path.
        /// </summary>
        /// <param name="questJson">The JSON string representing the quest data.</param>
        /// <param name="savePath">The path where the quest asset should be saved.</param>
        /// <returns>Returns true if the quest asset exists, otherwise false.</returns>
        public static bool DoesQuestAssetExist(string questJson, string savePath)
        {
            QuestData questData = UtilityLibrary.DeserializeJson<QuestData>(questJson);
            string path = savePath + "/" + questData.name + "/Resources";
            if (Directory.Exists(path))
            {
                path += "/" + questData.name + ".asset";
                return File.Exists(path);
            }
            return false;
        }

        /// <summary>
        /// Creates a quest asset from JSON data and saves it with the provided metadata.
        /// </summary>
        /// <param name="questJson">The JSON string representing the quest data.</param>
        /// <param name="metadata">Metadata for the quest.</param>
        public static void CreateQuestFromJson(string questJson, Quest.QuestMetadata metadata)
        {
            QuestData questData = UtilityLibrary.DeserializeJson<QuestData>(questJson);
            Quest quest = ScriptableObject.CreateInstance<Quest>();
            quest.QuestDescription = questData.description;
            quest.QuestGoal = questData.goal;

            foreach (ObjectiveData objective in questData.objectives)
            {
                quest.AddObjective(objective.reference, objective.description);
            }

            foreach (RewardData reward in questData.rewards)
            {
                InventoryItem item = Resources.Load<InventoryItem>(reward.item);
                if (item != null)
                {
                    quest.AddReward(reward.number, item);
                }
                else
                {
                    Debug.LogError($"InventoryItem '{reward.item}' not found in Resources.");
                }
            }

            quest.Metadata = metadata;
            string path = QuestEditorWindow.QuestAssetSavePath + "/" + questData.name + "/Resources";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            path += "/" + questData.name + ".asset";
            UnityEditor.AssetDatabase.CreateAsset(quest, path);
            UnityEditor.AssetDatabase.SaveAssets();
        }
    }

    /// <summary>
    /// Serializable class to format quest metadata.
    /// </summary>
    [System.Serializable]
    public class QuestMetadataFormat
    {
        public string title;
        public string name;
        public string description;
        public string goal;
        public List<ObjectiveData> objectives;
    }

    /// <summary>
    /// Serializable class representing the data structure of a quest.
    /// </summary>
    [System.Serializable]
    public class QuestData
    {
        public string name;
        public string description;
        public string goal;
        public List<ObjectiveData> objectives;
        public List<RewardData> rewards;
    }

    /// <summary>
    /// Serializable class for quest objective data.
    /// </summary>
    [System.Serializable]
    public class ObjectiveData
    {
        public string reference;
        public string description;
    }

    /// <summary>
    /// Serializable class for quest reward data.
    /// </summary>
    [System.Serializable]
    public class RewardData
    {
        public int number;
        public string item;
    }
}
