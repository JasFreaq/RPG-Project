using System.Collections.Generic;
using Campbell.Editor.QuestGeneration.Utility;
using Campbell.Quests;
using Unity.Plastic.Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace Campbell.Editor.QuestGeneration
{
    /// <summary>
    /// Class responsible for processing and managing items related to quest generation.
    /// </summary>
    public class ItemProcessor
    {
        /// <summary>
        /// The selected model type for item generation.
        /// </summary>
        public ModelType selectedModel = 0;

        /// <summary>
        /// Generates items based on the provided quest and updates the formatted items list.
        /// </summary>
        /// <param name="formattedQuest">The formatted quest string used for item generation.</param>
        /// <param name="formattedItems">Reference to the list where generated items will be stored.</param>
        public void GenerateItems(string formattedQuest, ref List<string> formattedItems)
        {
            selectedModel = (ModelType)EditorGUILayout.EnumPopup("Model", selectedModel);
            if (GUILayout.Button("Generate Items"))
            {
                if (formattedItems.Count > 0)
                {
                    formattedItems.Clear();
                }

                string quest = UtilityLibrary.FormatStringForPython(formattedQuest);
                string requiredItemsSchema = UtilityLibrary.FormatStringForPython(Resources.Load<TextAsset>("campbellRequiredItemsSchema").text);
                string actionItemTemplate = UtilityLibrary.FormatStringForPython(Resources.Load<TextAsset>("campbellActionItemTemplate").text);
                string equipmentTemplate = UtilityLibrary.FormatStringForPython(Resources.Load<TextAsset>("campbellEquipmentTemplate").text);
                string statEquipmentTemplate = UtilityLibrary.FormatStringForPython(Resources.Load<TextAsset>("campbellStatEquipmentTemplate").text);
                string model = UtilityLibrary.GetModelString(selectedModel);

                string itemScript = "import UnityEngine;\n" +
                                    "from campbell_quest import item_generator\n" +
                                    "\n" +
                                    $"formatted_quest = \"{quest}\"\n" +
                                    $"required_items_schema = \"{requiredItemsSchema}\"\n" +
                                    "item_templates = {}\n" +
                                    $"item_templates[\"Action Item\"] = \"{actionItemTemplate}\"\n" +
                                    $"item_templates[\"Equipment\"] = \"{equipmentTemplate}\"\n" +
                                    $"item_templates[\"Stat-Boosting Equipment\"] = \"{statEquipmentTemplate}\"\n" +
                                    $"model = \"{model}\"\n" +
                                    "items = item_generator.get_items(formatted_quest, required_items_schema, item_templates, model)\n" +
                                    "for item in items:\n" +
                                    "\tprint(item)\n" +
                                    "\tprint(\"@\")\n";

                string items = UtilityLibrary.RunPythonScript(itemScript);
                List<string> itemsBuilder = new List<string>();
                formattedItems.AddRange(items.Split('@'));

                foreach (string item in formattedItems)
                {
                    if (!string.IsNullOrWhiteSpace(item))
                    {
                        itemsBuilder.Add(item);
                    }
                }

                formattedItems = itemsBuilder;

                if (formattedItems.Count == 0)
                {
                    Debug.LogWarning("No items generated.");
                }
            }
        }

        /// <summary>
        /// Displays and edits item information from a JSON string.
        /// </summary>
        /// <param name="itemJson">The JSON string containing item data.</param>
        /// <returns>The updated JSON string after editing.</returns>
        public string DisplayItemInformation(string itemJson)
        {
            ItemData itemData = UtilityLibrary.DeserializeJson<ItemData>(itemJson);
            EditorGUILayout.LabelField("Item Name", EditorStyles.boldLabel);
            itemData.item.name = EditorGUILayout.TextField(itemData.item.name);
            EditorGUILayout.LabelField("Item Description", EditorStyles.boldLabel);
            itemData.item.description = EditorGUILayout.TextField(itemData.item.description);
            EditorGUILayout.LabelField("Item Type", EditorStyles.boldLabel);
            itemData.item_type = EditorGUILayout.TextField(itemData.item_type);
            EditorGUILayout.Space();
            itemData.item.stackable = EditorGUILayout.Toggle("Stackable", itemData.item.stackable);

            if (itemData.item_type == "Action Item")
            {
                itemData.item.consumable = EditorGUILayout.Toggle("Consumable", itemData.item.consumable);
            }
            else if (itemData.item_type == "Equipment")
            {
                itemData.item.allowedEquipLocation = EditorGUILayout.TextField("Allowed Equip Location", itemData.item.allowedEquipLocation);
            }
            else if (itemData.item_type == "Stat-Boosting Equipment")
            {
                itemData.item.allowedEquipLocation = EditorGUILayout.TextField("Allowed Equip Location", itemData.item.allowedEquipLocation);
                EditorGUILayout.LabelField("Additive Bonuses", EditorStyles.boldLabel);

                if (itemData.item.additiveBonuses != null)
                {
                    for (int i = 0; i < itemData.item.additiveBonuses.Count; i++)
                    {
                        itemData.item.additiveBonuses[i].stat = EditorGUILayout.TextField("Stat", itemData.item.additiveBonuses[i].stat);
                        itemData.item.additiveBonuses[i].value = EditorGUILayout.IntField("Value", itemData.item.additiveBonuses[i].value);
                    }
                }

                EditorGUILayout.LabelField("Multiplicative Bonuses", EditorStyles.boldLabel);

                if (itemData.item.multiplicativeBonuses != null)
                {
                    for (int i = 0; i < itemData.item.multiplicativeBonuses.Count; i++)
                    {
                        itemData.item.multiplicativeBonuses[i].stat = EditorGUILayout.TextField("Stat", itemData.item.multiplicativeBonuses[i].stat);
                        itemData.item.multiplicativeBonuses[i].value = EditorGUILayout.IntField("Value", itemData.item.multiplicativeBonuses[i].value);
                    }
                }
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Objectives", EditorStyles.boldLabel);

            if (itemData.objectives != null)
            {
                for (int i = 0; i < itemData.objectives.Count; i++)
                {
                    itemData.objectives[i].objective_reference = EditorGUILayout.TextField("Objective Reference", itemData.objectives[i].objective_reference);
                    itemData.objectives[i].objective_type = EditorGUILayout.TextField("Objective Type", itemData.objectives[i].objective_type);
                }
            }

            if (GUI.changed)
            {
                itemJson = JsonConvert.SerializeObject(itemData);
            }

            return itemJson;
        }

        /// <summary>
        /// Clears the list of formatted items.
        /// </summary>
        /// <param name="formattedItems">Reference to the list of formatted items to be cleared.</param>
        /// <returns>Returns true if items were cleared, otherwise false.</returns>
        public bool ClearItems(ref List<string> formattedItems)
        {
            if (GUILayout.Button("Clear Items"))
            {
                formattedItems.Clear();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Creates an item asset from a JSON string and saves it at the specified path.
        /// </summary>
        /// <param name="item">The JSON string containing item data.</param>
        /// <param name="questAsset">The quest asset associated with the item.</param>
        /// <param name="itemAssetSavePath">The path where the item asset will be saved.</param>
        public void CreateItemAsset(string item, Quest questAsset, string itemAssetSavePath)
        {
            if (GUILayout.Button("Create Item Assets"))
            {
                ItemGenerator.CreateItemFromJson(item, questAsset, itemAssetSavePath);
            }
        }

        /// <summary>
        /// Recreates an item asset from a JSON string and saves it at the specified path.
        /// </summary>
        /// <param name="item">The JSON string containing item data.</param>
        /// <param name="questAsset">The quest asset associated with the item.</param>
        /// <param name="itemAssetSavePath">The path where the item asset will be saved.</param>
        public void RecreateItemAsset(string item, Quest questAsset, string itemAssetSavePath)
        {
            if (GUILayout.Button("Recreate Item Assets"))
            {
                ItemGenerator.CreateItemFromJson(item, questAsset, itemAssetSavePath);
            }
        }
    }
}
