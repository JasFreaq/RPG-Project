using Campbell.Dialogues;
using System;
using System.Collections.Generic;
using System.IO;
using Campbell.Core;
using Campbell.Editor.QuestGeneration.Utility;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Campbell.Editor.QuestGeneration
{
    /// <summary>
    /// Class responsible for generating dialogue assets from JSON data.
    /// </summary>
    public class DialogueGenerator
    {
        /// <summary>
        /// Checks if a dialogue asset already exists for the given JSON data at the specified save path.
        /// </summary>
        /// <param name="dialogueJson">JSON string representing the dialogue data.</param>
        /// <param name="savePath">The path where dialogue assets are saved.</param>
        /// <returns>True if the dialogue asset exists, false otherwise.</returns>
        public static bool DoesDialogueAssetExist(string dialogueJson, string savePath)
        {
            DialogueData dialogueData = UtilityLibrary.DeserializeJson<DialogueData>(dialogueJson);
            string path = savePath + "/Resources/Dialogues";
            if (Directory.Exists(path))
            {
                path += "/" + dialogueData.npc_name + ".asset";
                return File.Exists(path);
            }
            return false;
        }

        /// <summary>
        /// Creates a dialogue asset from the provided JSON string and saves it to the specified path.
        /// </summary>
        /// <param name="dialogueJson">JSON string representing the dialogue data.</param>
        /// <param name="savePath">The path where the dialogue asset should be saved.</param>
        public static void CreateDialogueFromJson(string dialogueJson, string savePath)
        {
            DialogueData dialogueData = UtilityLibrary.DeserializeJson<DialogueData>(dialogueJson);
            Dialogue dialogue = ScriptableObject.CreateInstance<Dialogue>();
            dialogue.name = dialogueData.npc_name;
            string path = savePath + "/Resources/Dialogues";
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

        /// <summary>
        /// Processes and adds dialogue choices to the dialogue structure.
        /// </summary>
        /// <param name="dialogue">The dialogue object to add choices to.</param>
        /// <param name="parentNode">The parent node to which choices are added.</param>
        /// <param name="choices">List of choice data to process.</param>
        private static void ProcessDialogueChoices(Dialogue dialogue, DialogueNode parentNode, List<ChoiceData> choices)
        {
            foreach (ChoiceData choice in choices)
            {
                DialogueNode playerNode = dialogue.CreateNode(parentNode);
                playerNode.SetIsPlayer(true);
                playerNode.SetText(choice.player_dialogue);
                if (!string.IsNullOrWhiteSpace(choice.condition))
                {
                    playerNode.Condition = CreateCondition(choice.condition);
                }
                if (!string.IsNullOrWhiteSpace(choice.result))
                {
                    playerNode.DialogueActions.Add(CreateDialogueAction(choice.result));
                }
                DialogueNode npcResponseNode = dialogue.CreateNode(playerNode);
                npcResponseNode.SetIsPlayer(false);
                npcResponseNode.SetText(choice.npc_dialogue);
                if (choice.choices is { Count: > 0 })
                {
                    ProcessDialogueChoices(dialogue, npcResponseNode, choice.choices);
                }
            }
        }

        /// <summary>
        /// Arranges dialogue nodes in a hierarchical structure visually.
        /// </summary>
        /// <param name="dialogue">The dialogue object containing nodes.</param>
        /// <param name="node">The current node to arrange.</param>
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
                    DialogueNode.MIN_WIDTH,
                    DialogueNode.MIN_HEIGHT
                );
                ArrangeDialogueNodes(dialogue, childNode);
            }
        }

        /// <summary>
        /// Creates a condition object from a condition string.
        /// </summary>
        /// <param name="conditionString">The string representing the condition.</param>
        /// <returns>A Condition object parsed from the string.</returns>
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

        /// <summary>
        /// Creates a dialogue action data object from an action string.
        /// </summary>
        /// <param name="actionString">The string representing the dialogue action.</param>
        /// <returns>A DialogueActionData object parsed from the string.</returns>
        private static DialogueActionData CreateDialogueAction(string actionString)
        {
            string actionName = actionString.Substring(0, actionString.IndexOf('('));
            string[] parameters = actionString.Substring(actionString.IndexOf('(') + 1, actionString.IndexOf(')') - actionString.IndexOf('(') - 1).Split(',');
            DialogueAction actionType = actionName switch
            {
                "receive_quest" => DialogueAction.GiveQuest,
                "complete_objective" => DialogueAction.CompleteObjective,
                "complete_quest" => DialogueAction.CompleteQuest,
                "attack_player" => DialogueAction.Attack,
                "add_item" => DialogueAction.EditInventory,
                "remove_item" => DialogueAction.EditInventory,
                _ => DialogueAction.None,
            };
            DialogueActionData actionData = new DialogueActionData { action = actionType };
            if (parameters.Length > 0)
            {
                actionData.parameter = parameters[0];
            }
            return actionData;
        }
    }

    /// <summary>
    /// Represents the data structure for dialogue information.
    /// </summary>
    [System.Serializable]
    public class DialogueData
    {
        public string npc_name;
        public string npc_dialogue;
        public List<ChoiceData> choices;
    }

    /// <summary>
    /// Represents the data structure for dialogue choices.
    /// </summary>
    [System.Serializable]
    public class ChoiceData
    {
        public string condition;
        public string player_dialogue;
        public string npc_dialogue;
        public string result;
        public List<ChoiceData> choices;
    }
}
