using System.Collections.Generic;
using UnityEngine;
using System.IO;
using GameDevTV.Inventories;
using RPG.Dialogues;
using Unity.Plastic.Newtonsoft.Json;
using System;
using RPG.Core;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using RPG.UI;

namespace RPG.Quests.Editor
{
    public class AssetGenerator 
    {
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

        [System.Serializable]
        public class DialogueData
        {
            public string npc_name;
            public string npc_dialogue;
            public List<ChoiceData> choices;
        }

        [System.Serializable]
        public class ChoiceData
        {
            public string condition;
            public string player_dialogue;
            public string npc_dialogue;
            public string result;
            public List<ChoiceData> choices;
        }

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
        
        public static bool DoesDialogueAssetExist(string dialogueJson, string savePath, string generatedQuestName)
        {
            DialogueData dialogueData = JsonConvert.DeserializeObject<DialogueData>(dialogueJson);

            string path = savePath + "/" + generatedQuestName + "/Dialogues";
            if (Directory.Exists(path))
            {
                path += "/" + dialogueData.npc_name + ".asset";
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
                    quest.AddReward(reward.number, item);
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
        
        public static void CreateDialogueFromJson(string dialogueJson, string savePath)
        {
            DialogueData dialogueData = JsonConvert.DeserializeObject<DialogueData>(dialogueJson);
            
            Dialogue dialogue = ScriptableObject.CreateInstance<Dialogue>();
            dialogue.name = dialogueData.npc_name;

            string path = savePath + "/Dialogues";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            
            path += "/" + dialogueData.npc_name + ".asset";
            UnityEditor.AssetDatabase.CreateAsset(dialogue, path);
            
            dialogue.CreateRootNode();

            dialogue.DialogueNodes[0].SetIsPlayer(false);
            dialogue.DialogueNodes[0].SetText(dialogueData.npc_dialogue);

            ProcessDialogueChoices(dialogue, dialogue.DialogueNodes[0], dialogueData.choices);

            dialogue.DialogueNodes[0].PositionRect = new Rect(50, 750, DialogueNode.MIN_WIDTH, DialogueNode.MIN_HEIGHT);
            ArrangeDialogueNodes(dialogue, dialogue.DialogueNodes[0]);
            dialogue.EditorScrollPosition = new Vector2(0, 500);

            UnityEditor.AssetDatabase.SaveAssets();

            Scene activeScene = SceneManager.GetActiveScene();
            EditorSceneManager.SaveScene(activeScene);
        }

        private static void ProcessDialogueChoices(Dialogue dialogue, DialogueNode parentNode, List<ChoiceData> choices)
        {
            foreach (ChoiceData choice in choices)
            {
                DialogueNode playerNode = dialogue.CreateNode(parentNode);
                playerNode.SetIsPlayer(true);
                playerNode.SetText(choice.player_dialogue);
                if (!string.IsNullOrWhiteSpace(choice.result))
                {
                    playerNode.OnEnterActions.Add(CreateDialogueAction(choice.result));
                }

                DialogueNode npcResponseNode = dialogue.CreateNode(playerNode);
                npcResponseNode.SetIsPlayer(false);
                npcResponseNode.SetText(choice.npc_dialogue);
                if (!string.IsNullOrWhiteSpace(choice.condition))
                {
                    npcResponseNode.Condition = CreateCondition(choice.condition);
                }

                if (choice.choices != null && choice.choices.Count > 0)
                {
                    ProcessDialogueChoices(dialogue, npcResponseNode, choice.choices);
                }
            }
        }

        private static void ArrangeDialogueNodes(Dialogue dialogue, DialogueNode node)
        {
            IReadOnlyList<DialogueNode> children = dialogue.GetChildrenOfNode(node);

            float startY = node.PositionRect.y - (children.Count - 1) * 150;
            for (int i = 0; i < children.Count; i++)
            {
                DialogueNode childNode = children[i];
                
                childNode.PositionRect = new Rect(
                    node.PositionRect.x + 450, 
                    startY + i * 300, 
                    DialogueNode.MIN_WIDTH, DialogueNode.MIN_HEIGHT
                );
                
                ArrangeDialogueNodes(dialogue, childNode);
            }
        }

        private static Condition CreateCondition(string conditionString)
        {
            Condition condition = new Condition();
            
            string[] andParts = conditionString.Split(new[] { " and " }, StringSplitOptions.None);

            foreach (string andPart in andParts)
            {
                Condition.Disjunction disjunction = new Condition.Disjunction();
                
                string[] orParts = andPart.Split(new[] { " or " }, StringSplitOptions.None);

                foreach (string orPart in orParts)
                {
                    bool negate = orPart.Contains("not ") || orPart.Contains("!");

                    string predicateString = orPart.Replace("not ", "");
                    predicateString = predicateString.Replace("!", "");
                    predicateString = predicateString.Trim();
                    
                    string predicateName = predicateString.Substring(0, predicateString.IndexOf('('));
                    string[] parameters = predicateString.Substring(predicateString.IndexOf('(') + 1, predicateString.IndexOf(')') - predicateString.IndexOf('(') - 1).Split(',');
                    
                    for (int i = 0; i < parameters.Length; i++)
                    {
                        parameters[i] = parameters[i].Trim().Trim('\'');
                    }
                    
                    Condition.PredicateType predicateType = predicateName switch
                    {
                        "has_quest" => Condition.PredicateType.HasQuest,
                        "completed_objective" => Condition.PredicateType.CompletedObjective,
                        "completed_quest" => Condition.PredicateType.CompletedQuest,
                        "has_item" => Condition.PredicateType.HasItem,
                        _ => Condition.PredicateType.None,
                    };
                    
                    Condition.Predicate predicate = new Condition.Predicate
                    {
                        PredicateType = predicateType,
                        Negate = negate,
                        Parameters = parameters
                    };

                    disjunction.Or.Add(predicate);
                }
                
                condition.And.Add(disjunction);
            }

            return condition;
        }

        private static DialogueAction CreateDialogueAction(string actionString)
        {
            string actionName = actionString.Substring(0, actionString.IndexOf('('));

            DialogueAction actionType = actionName switch
            {
                "receive_quest" => DialogueAction.GiveQuest,
                "complete_quest" => DialogueAction.CompleteQuest,
                _ => DialogueAction.None,
            };

            return actionType;
        }
    }
}