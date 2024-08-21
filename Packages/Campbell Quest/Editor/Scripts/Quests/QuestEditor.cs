using Campbell.Quests;
using System.Collections.Generic;
using Campbell.Editor.QuestGeneration;
using Unity.Plastic.Newtonsoft.Json;
using UnityEditor;
using UnityEngine;
using static Campbell.Quests.Quest.QuestMetadata;
using static Campbell.Quests.Quest;
using UnityEngine.Profiling.Memory.Experimental;

namespace Campbell.Editor.Quests
{
    [CustomEditor(typeof(Quest))]
    public class QuestEditor : UnityEditor.Editor
    {
        private SerializedProperty _objectivesProperty;
        private SerializedProperty _rewardsProperty;

        private void OnEnable()
        {
            _objectivesProperty = serializedObject.FindProperty("_objectives");
            _rewardsProperty = serializedObject.FindProperty("_rewards");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            DrawPropertiesExcluding(serializedObject, "_objectives", "_rewards");

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(_objectivesProperty, true);
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();

                Quest quest = (Quest)target;
                UpdateQuestMetadata(quest);

                EditorUtility.SetDirty(quest);
                AssetDatabase.SaveAssetIfDirty(quest);
            }

            EditorGUILayout.PropertyField(_rewardsProperty, true);
        }

        private void UpdateQuestMetadata(Quest quest)
        {
            QuestMetadataFormat questMetadataFormat = new QuestMetadataFormat();
            questMetadataFormat.title = "Quest";
            questMetadataFormat.name = quest.name;
            questMetadataFormat.description = quest.QuestDescription;
            questMetadataFormat.goal = quest.QuestGoal;
            List<ObjectiveData> objectives = new List<ObjectiveData>();
            foreach (Objective objective in quest.Objectives)
            {
                objectives.Add(new ObjectiveData
                    { reference = objective.reference, description = objective.description });
            }

            questMetadataFormat.objectives = objectives;

            QuestMetadata metadata = new QuestMetadata
            {
                formattedQuest = JsonConvert.SerializeObject(questMetadataFormat),
                characterInformation = quest.Metadata.characterInformation,
                locationInformation = quest.Metadata.locationInformation
            };

            quest.Metadata = metadata;
        }
    }
}