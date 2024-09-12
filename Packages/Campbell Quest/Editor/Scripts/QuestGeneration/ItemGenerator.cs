using System;
using Campbell.Editor.QuestGeneration.Utility;
using Campbell.InventorySystem;
using Campbell.Stats;
using System.Collections.Generic;
using System.IO;
using Campbell.Attributes;
using Campbell.Combat;
using Campbell.InventorySystem.Pickups;
using UnityEngine;
using Campbell.Quests;
using UnityEditor.Events;
using UnityEngine.Events;
using UnityEditor;

namespace Campbell.Editor.QuestGeneration
{
    /// <summary>
    /// Class responsible for generating items from JSON data and creating associated assets.
    /// </summary>
    public class ItemGenerator
    {
        /// <summary>
        /// Checks if an item asset already exists at the given save path.
        /// </summary>
        /// <param name="itemJson">The JSON representation of the item.</param>
        /// <param name="savePath">The path where the item asset would be saved.</param>
        /// <returns>True if the item asset exists, otherwise false.</returns>
        public static bool DoesItemAssetExist(string itemJson, string savePath)
        {
            ItemData itemData = UtilityLibrary.DeserializeJson<ItemData>(itemJson);
            string path = savePath + "/Resources/Items";
            if (Directory.Exists(path))
            {
                path += "/" + itemData.item.name + " Item.asset";
                return File.Exists(path);
            }
            return false;
        }

        /// <summary>
        /// Creates an item from JSON data and saves it as an asset, including the creation of associated prefabs.
        /// </summary>
        /// <param name="itemJson">The JSON representation of the item.</param>
        /// <param name="questAsset">The quest asset associated with the item.</param>
        /// <param name="savePath">The path where the item and prefabs will be saved.</param>
        public static void CreateItemFromJson(string itemJson, Quest questAsset, string savePath)
        {
            ItemData itemData = UtilityLibrary.DeserializeJson<ItemData>(itemJson);
            string path = savePath + "/Resources/Items";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            // Create the pickup prefab
            Pickup basePickup = Resources.Load<Pickup>("Default Proximity Pickup");
            string pickupPath = path + "/" + itemData.item.name + " Pickup.prefab";
            GameObject pickup = PrefabUtility.SaveAsPrefabAsset(basePickup.gameObject, pickupPath);

            foreach (ItemObjective objective in itemData.objectives)
            {
                AssignPickupObjectives(pickup, questAsset, objective);
            }
            PrefabUtility.SavePrefabAsset(pickup);

            // Create the item asset
            InventoryItem item;
            switch (itemData.item_type)
            {
                case "Action Item":
                    item = CreateActionItem(itemData.item);
                    break;
                case "Equipment":
                case "Stat-Boosting Equipment":
                    item = CreateEquipment(itemData);
                    break;
                default:
                    throw new Exception("Invalid item type: " + itemData.item_type);
            }
            item.Pickup = pickup.GetComponent<Pickup>();
            string itemPath = path + "/" + itemData.item.name + " Item.asset";
            UnityEditor.AssetDatabase.CreateAsset(item, itemPath);

            // Create the pickup spawner prefab
            PickupSpawner baseSpawner = Resources.Load<PickupSpawner>("Pickup Spawner");
            string spawnerPath = path + "/" + itemData.item.name + " Pickup Spawner.prefab";
            GameObject spawner = PrefabUtility.SaveAsPrefabAsset(baseSpawner.gameObject, spawnerPath);
            item = Resources.Load<InventoryItem>($"Items/{itemData.item.name} Item");
            spawner.GetComponent<PickupSpawner>().Item = item;
            PrefabUtility.SavePrefabAsset(spawner);
        }

        /// <summary>
        /// Creates an ActionItem from serialized data.
        /// </summary>
        /// <param name="itemSerializedData">The serialized data of the item.</param>
        /// <returns>A new instance of ActionItem.</returns>
        private static InventoryItem CreateActionItem(ItemSerializedData itemSerializedData)
        {
            ActionItem actionItem = ScriptableObject.CreateInstance<ActionItem>();
            actionItem.DisplayName = itemSerializedData.name;
            actionItem.Description = itemSerializedData.description;
            actionItem.Stackable = itemSerializedData.stackable;
            actionItem.Consumable = itemSerializedData.consumable;
            return actionItem;
        }

        /// <summary>
        /// Creates an Equipment item from item data.
        /// </summary>
        /// <param name="itemData">The data representing the item.</param>
        /// <returns>A new instance of InventoryItem configured as Equipment.</returns>
        private static InventoryItem CreateEquipment(ItemData itemData)
        {
            if (System.Enum.TryParse(itemData.item.allowedEquipLocation, true, out EquipLocation location))
            {
                if (location == EquipLocation.Weapon)
                {
                    return CreateWeaponConfig(itemData.item);
                }
                if (itemData.item_type == "Equipment")
                {
                    return CreateStatBoostingEquipableItem(itemData.item);
                }
                if (itemData.item_type == "Stat-Boosting Equipment")
                {
                    return CreateEquipableItem(itemData.item);
                }
            }
            Debug.LogError("Invalid equip location: " + itemData.item.allowedEquipLocation);
            return null;
        }

        /// <summary>
        /// Creates a WeaponConfig from serialized data.
        /// </summary>
        /// <param name="itemSerializedData">The serialized data of the item.</param>
        /// <returns>A new instance of WeaponConfig.</returns>
        private static InventoryItem CreateWeaponConfig(ItemSerializedData itemSerializedData)
        {
            WeaponConfig weaponConfig = ScriptableObject.CreateInstance<WeaponConfig>();
            EncodeInventoryItem(weaponConfig, itemSerializedData);
            weaponConfig.AllowedEquipLocation = EquipLocation.Weapon;
            if (itemSerializedData.additiveBonuses != null)
            {
                weaponConfig.AdditiveBonuses = EncodeBonuses(itemSerializedData.additiveBonuses);
            }
            if (itemSerializedData.multiplicativeBonuses != null)
            {
                weaponConfig.MultiplicativeBonuses = EncodeBonuses(itemSerializedData.multiplicativeBonuses);
            }
            return weaponConfig;
        }

        /// <summary>
        /// Creates an EquipableItem from serialized data.
        /// </summary>
        /// <param name="itemSerializedData">The serialized data of the item.</param>
        /// <returns>A new instance of EquipableItem.</returns>
        private static InventoryItem CreateEquipableItem(ItemSerializedData itemSerializedData)
        {
            EquipableItem equipableItem = ScriptableObject.CreateInstance<EquipableItem>();
            EncodeInventoryItem(equipableItem, itemSerializedData);
            EncodeAllowedLocation(equipableItem, itemSerializedData);
            return equipableItem;
        }

        /// <summary>
        /// Creates a StatsEquipableItem from serialized data.
        /// </summary>
        /// <param name="itemSerializedData">The serialized data of the item.</param>
        /// <returns>A new instance of StatsEquipableItem.</returns>
        private static InventoryItem CreateStatBoostingEquipableItem(ItemSerializedData itemSerializedData)
        {
            StatsEquipableItem statEquipableItem = ScriptableObject.CreateInstance<StatsEquipableItem>();
            EncodeInventoryItem(statEquipableItem, itemSerializedData);
            EncodeAllowedLocation(statEquipableItem, itemSerializedData);
            if (itemSerializedData.additiveBonuses != null)
            {
                statEquipableItem.AdditiveBonuses = EncodeBonuses(itemSerializedData.additiveBonuses);
            }
            if (itemSerializedData.multiplicativeBonuses != null)
            {
                statEquipableItem.MultiplicativeBonuses = EncodeBonuses(itemSerializedData.multiplicativeBonuses);
            }
            return statEquipableItem;
        }

        /// <summary>
        /// Encodes the basic inventory item properties from serialized data.
        /// </summary>
        /// <param name="item">The equipable item to encode data into.</param>
        /// <param name="itemSerializedData">The serialized data of the item.</param>
        public static void EncodeInventoryItem(EquipableItem item, ItemSerializedData itemSerializedData)
        {
            item.DisplayName = itemSerializedData.name;
            item.Description = itemSerializedData.description;
            item.Stackable = itemSerializedData.stackable;
        }

        /// <summary>
        /// Encodes the allowed equip location for an equipable item.
        /// </summary>
        /// <param name="equipableItem">The equipable item to set the location for.</param>
        /// <param name="itemSerializedData">The serialized data of the item.</param>
        public static void EncodeAllowedLocation(EquipableItem equipableItem, ItemSerializedData itemSerializedData)
        {
            if (System.Enum.TryParse(itemSerializedData.allowedEquipLocation, true, out EquipLocation location))
            {
                equipableItem.AllowedEquipLocation = location;
            }
            else
            {
                Debug.LogError("Invalid equip location: " + itemSerializedData.allowedEquipLocation);
            }
        }

        /// <summary>
        /// Encodes bonuses from a list of item stat bonuses into modifiers.
        /// </summary>
        /// <param name="bonuses">The list of item stat bonuses.</param>
        /// <returns>An array of modifiers representing the bonuses.</returns>
        public static Modifier[] EncodeBonuses(List<ItemStatBonus> bonuses)
        {
            Modifier[] modifiers = new Modifier[bonuses.Count];
            for (int i = 0; i < bonuses.Count; i++)
            {
                ItemStatBonus bonus = bonuses[i];
                Modifier modifier = new Modifier { stat = (Stat)System.Enum.Parse(typeof(Stat), bonus.stat, true), value = bonus.value };
                modifiers[i] = modifier;
            }
            return modifiers;
        }

        /// <summary>
        /// Assigns objectives to a pickup based on the item objectives.
        /// </summary>
        /// <param name="pickup">The pickup game object to assign objectives to.</param>
        /// <param name="questAsset">The quest asset associated with the objectives.</param>
        /// <param name="objective">The item objective to assign.</param>
        public static void AssignPickupObjectives(GameObject pickup, Quest questAsset, ItemObjective objective)
        {
            QuestClearer questClearer = pickup.GetComponent<QuestClearer>();
            if (questClearer == null)
            {
                questClearer = pickup.AddComponent<QuestClearer>();
            }
            questClearer.Quest = questAsset;
            if (objective.objective_type.Contains("Pickup"))
            {
                PickupTrigger pickupTrigger = pickup.GetComponent<PickupTrigger>();
                if (pickupTrigger == null)
                {
                    pickupTrigger = pickup.AddComponent<PickupTrigger>();
                }
                UnityEvent onPickupEvent = new UnityEvent();
                UnityAction<string> onPickupAction = questClearer.CompleteQuestObjective;
                UnityEventTools.AddStringPersistentListener(onPickupEvent, onPickupAction, objective.objective_reference);
                pickupTrigger.AddTrigger(onPickupEvent);
            }
            else if (objective.objective_type.Contains("Destroy"))
            {
                BaseStats baseStats = pickup.AddComponent<BaseStats>();
                Progression progression = Resources.Load<Progression>("Progression");
                baseStats.Progression = progression;
                baseStats.Class = CharacterClass.Item;
                Health health = pickup.AddComponent<Health>();
                UnityAction<string> onDestroyAction = questClearer.CompleteQuestObjective;
                UnityEventTools.AddStringPersistentListener(health.OnDeath, onDestroyAction, objective.objective_reference);
            }
        }
    }

    /// <summary>
    /// Serializable class representing the data structure for an item.
    /// </summary>
    [System.Serializable]
    public class ItemData
    {
        public ItemSerializedData item;
        public string item_type;
        public List<ItemObjective> objectives;
    }

    /// <summary>
    /// Serializable class representing the serialized data of an item.
    /// </summary>
    [System.Serializable]
    public class ItemSerializedData
    {
        public string name;
        public string description;
        public bool stackable;
        public bool consumable; // Specific to ActionItem
        public string allowedEquipLocation; // Specific to EquipableItem
        public List<ItemStatBonus> additiveBonuses; // Specific to StatEquipableItem
        public List<ItemStatBonus> multiplicativeBonuses; // Specific to StatEquipableItem
    }

    /// <summary>
    /// Serializable class representing an objective associated with an item.
    /// </summary>
    [System.Serializable]
    public class ItemObjective
    {
        public string objective_reference;
        public string objective_type;
    }

    /// <summary>
    /// Serializable class representing a stat bonus for an item.
    /// </summary>
    [System.Serializable]
    public class ItemStatBonus
    {
        public string stat;
        public int value;
    }
}
