using System.Collections.Generic;
using System.IO;
using Campbell.Combat;
using Campbell.Control;
using Campbell.Dialogues;
using Campbell.InventorySystem;
using Campbell.Quests;
using UnityEditor;
using UnityEditor.Events;
using UnityEngine;
using UnityEngine.Events;

namespace Campbell.Editor.QuestGeneration
{
    public class NpcGenerator
    {
        public static void CreateNpcPrefab(DialogueData dialogueData, Quest questAsset, string savePath)
        {
            MonoBehaviour baseCharacter = Resources.Load<MonoBehaviour>("Base Character");
            
            string path = savePath + "/Resources";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            path += "/" + dialogueData.npc_name + " NPC Prefab.prefab";
            // Create a prefab from the base character
            GameObject npc = PrefabUtility.SaveAsPrefabAsset(baseCharacter.gameObject, path);

            AIConversationHandler aiConversation = npc.AddComponent<AIConversationHandler>();
            aiConversation.ConversantName = dialogueData.npc_name;
            Dialogue dialogueAsset = Resources.Load<Dialogue>($"{dialogueData.npc_name} Dialogue");
            aiConversation.Dialogue = dialogueAsset;

            DialogueTrigger dialogueTrigger = npc.AddComponent<DialogueTrigger>();

            IterateDialogueResults(npc, questAsset, dialogueTrigger, dialogueData.choices);
            PrefabUtility.SavePrefabAsset(npc);
        }

        private static void IterateDialogueResults(GameObject npc, Quest questAsset, DialogueTrigger dialogueTrigger, List<ChoiceData> choices)
        {
            foreach (ChoiceData choice in choices)
            {
                if (!string.IsNullOrWhiteSpace(choice.result))
                {
                    LinkDialogueAction(npc, questAsset, dialogueTrigger, choice.result);
                }

                if (choice.choices is { Count: > 0 })
                {
                    IterateDialogueResults(npc, questAsset, dialogueTrigger, choice.choices);
                }
            }
        }

        private static void LinkDialogueAction(GameObject npc, Quest questAsset, DialogueTrigger dialogueTrigger, string actionString)
        {
            string actionName = actionString.Substring(0, actionString.IndexOf('('));
            string[] parameters = actionString.Substring(actionString.IndexOf('(') + 1, actionString.IndexOf(')') - actionString.IndexOf('(') - 1).Split(',');

            DialogueAction actionType = DialogueAction.None;

            switch (actionName)
            {
                case "receive_quest":
                    QuestGiver questGiver = npc.GetComponent<QuestGiver>();
                    if (questGiver == null)
                    {
                        questGiver = npc.AddComponent<QuestGiver>();
                    }
                    questGiver.Quest = questAsset;
                    UnityEvent onReceiveEvent = new UnityEvent();
                    UnityAction onReceiveAction = questGiver.GiveQuest;
                    UnityEventTools.AddVoidPersistentListener(onReceiveEvent, onReceiveAction);
                    dialogueTrigger.AddTrigger(DialogueAction.GiveQuest, onReceiveEvent);
                    break;

                case "complete_objective":
                    QuestClearer objectiveClearer = npc.GetComponent<QuestClearer>();
                    if (objectiveClearer == null)
                    {
                        objectiveClearer = npc.AddComponent<QuestClearer>();
                    }
                    objectiveClearer.Quest = questAsset;
                    UnityEvent onObjectiveClearEvent = new UnityEvent();
                    UnityAction<string> onObjectiveClearAction = objectiveClearer.CompleteQuestObjective;
                    UnityEventTools.AddStringPersistentListener(onObjectiveClearEvent, onObjectiveClearAction, parameters[0]);
                    dialogueTrigger.AddTrigger(DialogueAction.CompleteObjective, onObjectiveClearEvent);
                    break;
                
                case "complete_quest":
                    QuestClearer questClearer = npc.GetComponent<QuestClearer>();
                    if (questClearer == null)
                    {
                        questClearer = npc.AddComponent<QuestClearer>();
                    }
                    questClearer.Quest = questAsset;
                    UnityEvent onQuestClearEvent = new UnityEvent();
                    UnityAction onQuestClearAction = questClearer.CompleteQuest;
                    UnityEventTools.AddVoidPersistentListener(onQuestClearEvent, onQuestClearAction);
                    dialogueTrigger.AddTrigger(DialogueAction.CompleteQuest, onQuestClearEvent);
                    break;

                case "attack_player":
                    AIController aiController = npc.GetComponent<AIController>();
                    if (aiController == null)
                    {
                        aiController = npc.AddComponent<AIController>();
                    }
                    UnityEvent onAttackEvent = new UnityEvent();
                    UnityAction onAttackAction = aiController.Aggravate;
                    onAttackAction += aiController.AggravateNearbyEnemies;
                    UnityEventTools.AddVoidPersistentListener(onAttackEvent, onAttackAction);
                    dialogueTrigger.AddTrigger(DialogueAction.Attack, onAttackEvent);

                    npc.AddComponent<CombatTarget>();
                    break;

                case "add_item":
                case "remove_item":
                    InventoryChanger inventoryChanger = npc.GetComponent<InventoryChanger>();
                    if (inventoryChanger == null)
                    {
                        inventoryChanger = npc.AddComponent<InventoryChanger>();
                    }
                    InventoryItem item = Resources.Load<InventoryItem>(parameters[0]);
                    inventoryChanger.Item = item;
                    if (actionName == "remove_item")
                    {
                        inventoryChanger.Remove = true;
                    }
                    inventoryChanger.Number = 1;
                    UnityEvent onInventoryEvent = new UnityEvent();
                    UnityAction onInventoryAction = inventoryChanger.EditInventory;
                    UnityEventTools.AddVoidPersistentListener(onInventoryEvent, onInventoryAction);
                    dialogueTrigger.AddTrigger(DialogueAction.EditInventory, onInventoryEvent);
                    break;
            }
        }
    }
}