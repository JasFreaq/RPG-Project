using System.Collections.Generic;
using System.IO;
using Campbell.Attributes;
using Campbell.Combat;
using Campbell.Control;
using Campbell.Dialogues;
using Campbell.Editor.QuestGeneration.Utility;
using Campbell.InventorySystem;
using Campbell.Quests;
using UnityEditor;
using UnityEditor.Events;
using UnityEngine;
using UnityEngine.Events;
using static Codice.Client.Common.Connection.AskCredentialsToUser;

namespace Campbell.Editor.QuestGeneration
{
    public class NpcGenerator
    {
        public static bool DoesNpcAssetExist(string npcName, string savePath)
        {
            string path = savePath + "/Resources/Npcs";
            if (Directory.Exists(path))
            {
                path += "/" + npcName + " NPC.prefab";
                return File.Exists(path);
            }

            return false;
        }

        public static void CreateEnemyPrefab(string enemyJson, Quest questAsset, string savePath)
        {
            MonoBehaviour baseCharacter = Resources.Load<MonoBehaviour>("Base Character");

            EnemyData enemyData = UtilityLibrary.DeserializeJson<EnemyData>(enemyJson);

            string path = savePath + "/Resources/Npcs";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            path += "/" + enemyData.enemy_name + " NPC.prefab";

            // Create a prefab from the base character
            GameObject npc = PrefabUtility.SaveAsPrefabAsset(baseCharacter.gameObject, path);

            SetupEnemy(npc);

            QuestClearer objectiveClearer = npc.GetComponent<QuestClearer>();
            if (objectiveClearer == null)
            {
                objectiveClearer = npc.AddComponent<QuestClearer>();
            }
            objectiveClearer.Quest = questAsset;
            Health health = npc.GetComponent<Health>();
            UnityAction<string> onObjectiveClearAction = objectiveClearer.CompleteQuestObjective;
            UnityEventTools.AddStringPersistentListener(health.OnDeath, onObjectiveClearAction, enemyData.objective_reference);

            PrefabUtility.SavePrefabAsset(npc);
        }

        public static void CreateNpcPrefab(Dialogue dialogueAsset, Quest questAsset, string savePath)
        {
            MonoBehaviour baseCharacter = Resources.Load<MonoBehaviour>("Base Character");
            
            DialogueData dialogueData = UtilityLibrary.DeserializeJson<DialogueData>(dialogueAsset.RawDialogueJson);

            string path = savePath + "/Resources/Npcs";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            path += "/" + dialogueData.npc_name + " NPC.prefab";

            // Create a prefab from the base character
            GameObject npc = PrefabUtility.SaveAsPrefabAsset(baseCharacter.gameObject, path);

            AIConversationHandler aiConversation = npc.AddComponent<AIConversationHandler>();
            aiConversation.ConversantName = dialogueData.npc_name;
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
                    SetupEnemy(npc);

                    AIController aiController = npc.GetComponent<AIController>();
                    UnityEvent onAttackEvent = new UnityEvent();
                    UnityAction onAttackAction = aiController.Aggravate;
                    onAttackAction += aiController.AggravateNearbyEnemies;
                    UnityEventTools.AddVoidPersistentListener(onAttackEvent, onAttackAction);
                    dialogueTrigger.AddTrigger(DialogueAction.Attack, onAttackEvent);
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

        private static void SetupEnemy(GameObject npc)
        {
            if (npc.GetComponent<CombatTarget>() == null)
            {
                npc.AddComponent<CombatTarget>();

                Health health = npc.GetComponent<Health>();
                AIController aiController = npc.GetComponent<AIController>();
                if (aiController == null)
                {
                    aiController = npc.AddComponent<AIController>();
                }

                UnityAction onAggravateAction = aiController.Aggravate;
                UnityEventTools.AddVoidPersistentListener(health.OnTakeDamage, onAggravateAction);
                UnityAction onAggravateNearbyEnemiesAction = aiController.AggravateNearbyEnemies;
                UnityEventTools.AddVoidPersistentListener(health.OnTakeDamage, onAggravateNearbyEnemiesAction);

                EnemyRandomDropper enemyRandomDropper = npc.GetComponent<EnemyRandomDropper>();
                if (enemyRandomDropper == null)
                {
                    enemyRandomDropper = npc.AddComponent<EnemyRandomDropper>();
                }

                DropLibrary dropLibrary = Resources.Load<DropLibrary>("Drop Library");
                enemyRandomDropper.DropLibrary = dropLibrary;
                UnityAction onDeathAction = enemyRandomDropper.RandomDrop;
                UnityEventTools.AddVoidPersistentListener(health.OnDeath, onDeathAction);
            }
        }
    }

    [System.Serializable]
    public class EnemyData
    {
        public string enemy_name;
        public string objective_reference;
    }
}