using System.Collections.Generic;
using UnityEngine;
using System.IO;
using GameDevTV.Inventories;
using Unity.Plastic.Newtonsoft.Json;

namespace RPG.Quests.Editor
{
    public class QuestGenerator 
    {
        [System.Serializable]
        public class QuestData
        {
            public string title;
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

        public static void CreateQuestFromJson(string jsonString, string savePath)
        {
            QuestData questData = JsonConvert.DeserializeObject<QuestData>(jsonString);
            
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
                    quest.AddReward(reward.number, item = item);
                }
                else
                {
                    Debug.LogError($"InventoryItem '{reward.item}' not found in Resources.");
                }
            }
            
            if (!Directory.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
            }

            string path = savePath + "/" + questData.title + ".asset";
            UnityEditor.AssetDatabase.CreateAsset(quest, path);
            UnityEditor.AssetDatabase.SaveAssets();
        }
    }
}