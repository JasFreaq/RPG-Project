using Campbell.InventorySystem;
using Campbell.Quests;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.Plastic.Newtonsoft.Json;
using UnityEngine;

namespace Campbell.Editor.QuestGeneration
{
    public class QuestGenerator 
    {
        public static bool DoesQuestAssetExist(string questJson, string savePath)
        {
            QuestData questData = JsonConvert.DeserializeObject<QuestData>(questJson);

            string path = savePath + "/" + questData.name;
            if (Directory.Exists(path))
            {
                path += "/" + questData.name + ".asset";
                return File.Exists(path);
            }

            return false;
        }

        public static string CreateQuestFromJson(string questJson, string savePath)
        {
            QuestData questData = JsonConvert.DeserializeObject<QuestData>(questJson);

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
                    quest.AddReward((int)reward.number, item);
                }
                else
                {
                    Debug.LogError($"InventoryItem '{reward.item}' not found in Resources.");
                }
            }

            string path = savePath + "/" + questData.name;
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            path += "/" + questData.name + ".asset";
            UnityEditor.AssetDatabase.CreateAsset(quest, path);
            UnityEditor.AssetDatabase.SaveAssets();

            return questData.name;
        }
    }

    [System.Serializable]
    public class QuestData
    {
        public string name;
        public string description;
        public string goal;
        public List<ObjectiveData> objectives;
        public List<RewardData> rewards;
    }

    [System.Serializable]
    public class ObjectiveData
    {
        public string reference;
        public string description;
    }

    [System.Serializable]
    public class RewardData
    {
        public int number;
        public string item;
    }
}