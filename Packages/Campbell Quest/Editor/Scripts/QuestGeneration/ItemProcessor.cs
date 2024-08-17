using System.Collections;
using System.Collections.Generic;
using Campbell.Editor.QuestGeneration.Utility;
using Campbell.Quests;
using Unity.Plastic.Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace Campbell.Editor.QuestGeneration
{
    public class ItemProcessor
    {
        public void GenerateItems(string formattedQuest, ref List<string> formattedItems)
        {
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

                string itemScript = "import UnityEngine;\n" +
                                    "from campbell_quest import item_generator\n" +
                                    "\n" +
                                    $"formatted_quest = \"{quest}\"\n" +
                                    $"required_items_schema = \"{requiredItemsSchema}\"\n" +
                                    "item_templates = {}\n" +
                                    $"item_templates[\"Action Item\"] = \"{actionItemTemplate}\"\n" +
                                    $"item_templates[\"Equipment\"] = \"{equipmentTemplate}\"\n" +
                                    $"item_templates[\"Stat-Boosting Equipment\"] = \"{statEquipmentTemplate}\"\n" +
                                    "items = item_generator.get_items(formatted_quest, required_items_schema, item_templates)\n" +
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

        public bool ClearItems(ref List<string> formattedItems)
        {
            if (GUILayout.Button("Clear Items"))
            {
                formattedItems.Clear();
                return true;
            }

            return false;
        }

        public void CreateItemAsset(string item, Quest questAsset, string itemAssetSavePath)
        {
            if (GUILayout.Button("Create Item Assets"))
            {
                ItemGenerator.CreateItemFromJson(item, questAsset, itemAssetSavePath);
            }
        }

        public void RecreateItemAsset(string item, Quest questAsset, string itemAssetSavePath)
        {
            if (GUILayout.Button("Recreate Item Assets"))
            {
                ItemGenerator.CreateItemFromJson(item, questAsset, itemAssetSavePath);
            }
        }
    }
}