using Campbell.Quests;
using System.Collections.Generic;
using Campbell.Editor.QuestGeneration;
using Unity.Plastic.Newtonsoft.Json;
using UnityEditor;
using static Campbell.Quests.Quest;

namespace Campbell.Editor.Quests
{
    /// <summary>
    /// Custom editor class for the Quest object.
    /// Provides a custom inspector interface for editing Quest properties in the Unity Editor.
    /// </summary>
    [CustomEditor(typeof(Quest))]
    public class QuestEditor : UnityEditor.Editor
    {
        private SerializedProperty _objectivesProperty;
        private SerializedProperty _rewardsProperty;

        /// <summary>
        /// Called when the editor is enabled.
        /// Initializes serialized properties for objectives and rewards.
        /// </summary>
        private void OnEnable()
        {
            _objectivesProperty = serializedObject.FindProperty("_objectives");
            _rewardsProperty = serializedObject.FindProperty("_rewards");
        }

        /// <summary>
        /// Overrides the default inspector GUI to add custom property fields.
        /// Handles updating the quest's metadata when objectives are modified.
        /// </summary>
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            // Draw all properties except objectives and rewards
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

        /// <summary>
        /// Updates the metadata of the given quest based on its current properties.
        /// </summary>
        /// <param name="quest">The quest whose metadata is to be updated.</param>
        private void UpdateQuestMetadata(Quest quest)
        {
            QuestMetadataFormat questMetadataFormat = new QuestMetadataFormat
            {
                title = "Quest",
                name = quest.name,
                description = quest.QuestDescription,
                goal = quest.QuestGoal
            };

            List<ObjectiveData> objectives = new List<ObjectiveData>();
            foreach (Objective objective in quest.Objectives)
            {
                objectives.Add(new ObjectiveData
                {
                    reference = objective.reference,
                    description = objective.description
                });
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
