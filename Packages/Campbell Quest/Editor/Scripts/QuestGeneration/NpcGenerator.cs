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
            
            string path = savePath + "/Resources/Npcs";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            path += "/" + dialogueAsset.name + " NPC.prefab";

            // Create a prefab from the base character
            GameObject npc = PrefabUtility.SaveAsPrefabAsset(baseCharacter.gameObject, path);

            AIConversationHandler aiConversation = npc.AddComponent<AIConversationHandler>();
            aiConversation.ConversantName = dialogueAsset.name;
            aiConversation.Dialogue = dialogueAsset;

            DialogueTrigger dialogueTrigger = npc.AddComponent<DialogueTrigger>();

            IterateDialogueResults(npc, questAsset, dialogueTrigger, dialogueAsset, dialogueAsset.DialogueNodes[0]);
            PrefabUtility.SavePrefabAsset(npc);
        }

        private static void IterateDialogueResults(GameObject npc, Quest questAsset, DialogueTrigger dialogueTrigger, Dialogue dialogueAsset, DialogueNode node)
        {
            foreach (DialogueActionData action in node.DialogueActions)
            {
                LinkDialogueAction(npc, questAsset, dialogueTrigger, action);
            }

            foreach (DialogueNode childNode in dialogueAsset.GetChildrenOfNode(node))
            {
                IterateDialogueResults(npc, questAsset, dialogueTrigger, dialogueAsset, childNode);
            }
        }

        private static void LinkDialogueAction(GameObject npc, Quest questAsset, DialogueTrigger dialogueTrigger, DialogueActionData action)
        {
            switch (action.action)
            {
                case DialogueAction.GiveQuest:
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

                case DialogueAction.CompleteObjective:
                    QuestClearer objectiveClearer = npc.GetComponent<QuestClearer>();
                    if (objectiveClearer == null)
                    {
                        objectiveClearer = npc.AddComponent<QuestClearer>();
                    }
                    objectiveClearer.Quest = questAsset;
                    UnityEvent onObjectiveClearEvent = new UnityEvent();
                    UnityAction<string> onObjectiveClearAction = objectiveClearer.CompleteQuestObjective;
                    UnityEventTools.AddStringPersistentListener(onObjectiveClearEvent, onObjectiveClearAction, action.parameter);
                    dialogueTrigger.AddTrigger(DialogueAction.CompleteObjective, onObjectiveClearEvent, action.parameter);
                    break;
                
                case DialogueAction.CompleteQuest:
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

                case DialogueAction.Attack:
                    SetupEnemy(npc);

                    AIController aiController = npc.GetComponent<AIController>();
                    UnityEvent onAttackEvent = new UnityEvent();
                    UnityAction onAttackAction = aiController.Aggravate;
                    onAttackAction += aiController.AggravateNearbyEnemies;
                    UnityEventTools.AddVoidPersistentListener(onAttackEvent, onAttackAction);
                    dialogueTrigger.AddTrigger(DialogueAction.Attack, onAttackEvent);
                    break;

                case DialogueAction.EditInventory:
                    InventoryChanger inventoryChanger = npc.GetComponent<InventoryChanger>();
                    if (inventoryChanger == null)
                    {
                        inventoryChanger = npc.AddComponent<InventoryChanger>();
                    }
                    InventoryItem item = Resources.Load<InventoryItem>(action.parameter);
                    inventoryChanger.Item = item;
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