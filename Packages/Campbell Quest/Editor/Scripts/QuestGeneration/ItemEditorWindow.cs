using Campbell.Editor.QuestGeneration.Utility;
using Campbell.Quests;
using System.Collections;
using System.Collections.Generic;
using Unity.Plastic.Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace Campbell.Editor.QuestGeneration
{
    public class ItemEditorWindow : EditorWindow
    {
        private List<string> _formattedItems = new List<string>();

        private int _itemTab = 0;

        private Vector2 _itemsScrollPosition;

        private ItemProcessor _itemProcessor;

        private Quest _quest;

        [MenuItem("Campbell/Item Editor Window", false, 2)]
        public static void ShowWindow()
        {
            GetWindow<ItemEditorWindow>("Campbell Item Editor");
        }

        private void OnEnable()
        {
            _itemProcessor = new ItemProcessor();
        }

        private void OnGUI()
        {
            Rect rect = EditorGUILayout.GetControlRect();
            _quest = (Quest)EditorGUI.ObjectField(rect, "Quest Asset", _quest, typeof(Quest), false);

            if (_quest == null)
            {
                if (_formattedItems.Count > 0)
                {
                    _formattedItems.Clear();
                }

                EditorGUILayout.HelpBox("Missing Quest Asset.", MessageType.Info);
            }
            else
            {
                Quest.QuestMetadata metadata = _quest.Metadata;
                _itemProcessor.GenerateItems(metadata.formattedQuest, ref _formattedItems);

                if (_formattedItems.Count > 0)
                {
                    FormatItemWindow();
                }
            }
        }

        private void FormatItemWindow()
        {
            if (_itemProcessor.ClearItems(ref _formattedItems))
            {
                _itemTab = 0;
                return;
            }

            EditorGUILayout.Space();

            string[] itemTabNames = new string[_formattedItems.Count];
            for (int i = 0; i < _formattedItems.Count; i++)
            {
                ItemData itemData = UtilityLibrary.DeserializeJson<ItemData>(_formattedItems[i]);
                itemTabNames[i] = $"{itemData.item.name}";
            }

            _itemTab = GUILayout.Toolbar(_itemTab, itemTabNames);

            DisplayGeneratedItems();

            EditorGUILayout.Space();

            Quest.QuestMetadata metadata = _quest.Metadata;
            string questName = UtilityLibrary.DeserializeJson<QuestData>(metadata.formattedQuest).name;
            string itemAssetSavePath = QuestEditorWindow.QuestAssetSavePath + "/" + questName;
            if (ItemGenerator.DoesItemAssetExist(_formattedItems[_itemTab], itemAssetSavePath))
            {
                _itemProcessor.RecreateItemAsset(_formattedItems[_itemTab], _quest, itemAssetSavePath);
            }
            else
            {
                _itemProcessor.CreateItemAsset(_formattedItems[_itemTab], _quest, itemAssetSavePath);
            }
        }

        private void DisplayGeneratedItems()
        {
            _itemsScrollPosition = EditorGUILayout.BeginScrollView(_itemsScrollPosition, GUILayout.ExpandHeight(true));

            _formattedItems[_itemTab] = _itemProcessor.DisplayItemInformation(_formattedItems[_itemTab]);

            EditorGUILayout.EndScrollView();
        }
    }
}