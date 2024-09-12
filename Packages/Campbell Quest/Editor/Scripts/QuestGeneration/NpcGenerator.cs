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

namespace Campbell.Editor.QuestGeneration
{
    /// <summary>
    /// Class responsible for generating NPCs (Non-Player Characters) and their associated prefabs in the game.
    /// </summary>
    public class NpcGenerator
    {
        /// <summary>
        /// Checks if an NPC asset already exists in the specified path.
        /// </summary>
        /// <param name="npcName">The name of the NPC.</param>
        /// <param name="savePath">The directory path where the NPC is supposed to be saved.</param>
        /// <returns>True if the NPC asset exists, otherwise false.</returns>
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

        /// <summary>
        /// Creates an enemy NPC prefab from a JSON representation and associates it with a quest.
        /// </summary>
        /// <param name="enemyJson">The JSON string containing enemy data.</param>
        /// <param name="questAsset">The quest asset to associate with the enemy NPC.</param>
        /// <param name="savePath">The directory path where the NPC prefab will be saved.</param>
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

        /// <summary>
        /// Creates a non-enemy NPC prefab associated with a dialogue and quest.
        /// </summary>
        /// <param name="dialogueAsset">The dialogue asset associated with the NPC.</param>
        /// <param name="questAsset">The quest asset to associate with the NPC.</param>
        /// <param name="savePath">The directory path where the NPC prefab will be saved.</param>
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

        /// <summary>
        /// Recursively iterates through dialogue results to link actions with the NPC.
        /// </summary>
        /// <param name="npc">The NPC game object.</param>
        /// <param name="questAsset">The quest asset associated with the NPC.</param>
        /// <param name="dialogueTrigger">The dialogue trigger component of the NPC.</param>
        /// <param name="dialogueAsset">The dialogue asset associated with the NPC.</param>
        /// <param name="node">The current dialogue node being processed.</param>
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

        /// <summary>
        /// Links a dialogue action to the NPC, setting up the appropriate components and events.
        /// </summary>
        /// <param name="npc">The NPC game object.</param>
        /// <param name="questAsset">The quest asset associated with the NPC.</param>
        /// <param name="dialogueTrigger">The dialogue trigger component of the NPC.</param>
        /// <param name="action">The dialogue action data to be linked.</param>
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

        /// <summary>
        /// Sets up an NPC as an enemy, adding necessary components and event listeners.
        /// </summary>
        /// <param name="npc">The NPC game object to be set up as an enemy.</param>
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

    /// <summary>
    /// Serializable class representing the data structure for an enemy NPC.
    /// </summary>
    [System.Serializable]
    public class EnemyData
    {
        /// <summary>
        /// Name of the enemy.
        /// </summary>
        public string enemy_name;

        /// <summary>
        /// Reference for the objective associated with the enemy.
        /// </summary>
        public string objective_reference;
    }
}
