using System.Collections.Generic;
using System.IO;
using System.Linq;
using Campbell.Editor.QuestGeneration.Utility;
using Campbell.Quests;
using Unity.Plastic.Newtonsoft.Json;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using static Campbell.Quests.Quest;
using static Campbell.Quests.Quest.QuestMetadata;

namespace Campbell.Editor.QuestGeneration
{
    public class QuestProcessor 
    {
        public string questPrompt;
        public string objectiveInformation;
        public string locationInformation;
        public string characterInformation;
        public string rewardInformation;

        public string formattedQuest;
        public string formattedQuestWithRewards;

        private Vector2 _questPromptScrollPosition;
        private Vector2 _objectiveScrollPosition;
        private Vector2 _locationScrollPosition;
        private Vector2 _characterScrollPosition;
        private Vector2 _rewardScrollPosition;

        private ReorderableList _objectivesList;
        private ReorderableList _rewardsList;
        
        public void PopulateFromSamples()
        {
            if (GUILayout.Button("Populate With Samples"))
            {
                questPrompt = Resources.Load<TextAsset>("campbellSamplePrompt").text;
                objectiveInformation = Resources.Load<TextAsset>("campbellSampleObjectives").text;
                locationInformation = Resources.Load<TextAsset>("campbellSampleLocations").text;
                characterInformation = Resources.Load<TextAsset>("campbellSampleCharacters").text;
                rewardInformation = Resources.Load<TextAsset>("campbellSampleRewards").text;
            }
        }
        
        public void DisplayQuestPrompt()
        {
            DisplayTextArea("Quest Prompt", ref questPrompt, ref _questPromptScrollPosition);
        }

        public void DisplayObjectiveInformation()
        {
            DisplayTextArea("Objective Information", ref objectiveInformation, ref _objectiveScrollPosition);
        }

        public void DisplayLocationInformation()
        {
            DisplayTextArea("Location Information", ref locationInformation, ref _locationScrollPosition);
        }

        public void DisplayCharacterInformation()
        {
            DisplayTextArea("Character Information", ref characterInformation, ref _characterScrollPosition);
        }

        public void DisplayRewardInformation()
        {
            DisplayTextArea("Reward Information", ref rewardInformation, ref _rewardScrollPosition);
        }

        private void DisplayTextArea(string label, ref string content, ref Vector2 scrollPosition)
        {
            GUIStyle textStyle = new GUIStyle(EditorStyles.textField)
            {
                padding = new RectOffset(5, 5, 5, 5),
                wordWrap = true
            };

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField(label);

            if (GUILayout.Button("Browse"))
            {
                string path = EditorUtility.OpenFilePanel($"Browse {label}", Application.dataPath, "png,txt,json");

                if (path.Length != 0)
                {
                    content = File.ReadAllText(path);
                }
            }

            EditorGUILayout.EndHorizontal();

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(150));

            content = EditorGUILayout.TextArea(content, textStyle, GUILayout.MinWidth(100),
                GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));

            EditorGUILayout.EndScrollView();
        }

        private bool IsAllContextValid()
        {
            string helpText = "";
            Dictionary<string, string> contextFields = new Dictionary<string, string>()
            {
                { "Quest Prompt", questPrompt },
                { "Objective Information", objectiveInformation },
                { "Location Information", locationInformation },
                { "Character Information", characterInformation },
                { "Reward Information", rewardInformation },
            };

            foreach (var field in contextFields)
            {
                if (string.IsNullOrWhiteSpace(field.Value))
                {
                    helpText += $"{field.Key} is missing.\n";
                }
            }

            if (!string.IsNullOrWhiteSpace(helpText))
            {
                EditorGUILayout.HelpBox(helpText, MessageType.Info);
            }

            return contextFields.Values.All(x => !string.IsNullOrWhiteSpace(x));
        }

        public bool GenerateQuest()
        {
            if (IsAllContextValid())
            {
                if (GUILayout.Button("Generate Quest"))
                {
                    if (!string.IsNullOrWhiteSpace(formattedQuestWithRewards))
                    {
                        formattedQuestWithRewards = null;
                    }

                    string prompt = UtilityLibrary.FormatStringForPython(questPrompt);
                    string objectives = UtilityLibrary.FormatStringForPython(objectiveInformation);
                    string locations = UtilityLibrary.FormatStringForPython(locationInformation);
                    string characters = UtilityLibrary.FormatStringForPython(characterInformation);
                    string rewards = UtilityLibrary.FormatStringForPython(rewardInformation);

                    string questSchema = UtilityLibrary.FormatStringForPython(Resources.Load<TextAsset>("campbellQuestSchema").text);
                    string questWithRewardsSchema = UtilityLibrary.FormatStringForPython(Resources.Load<TextAsset>("campbellQuestWithRewardsSchema").text);

                    string initialQuestScript = "import UnityEngine;\n" +
                                          "from campbell_quest import quest_generator\n" +
                                          "\n" +
                                          $"prompt = \"{prompt}\"\n" +
                                          $"objectives = \"{objectives}\"\n" +
                                          $"locations = \"{locations}\"\n" +
                                          $"characters = \"{characters}\"\n" +
                                          "initial_generated_quest = quest_generator.generate_initial_quest(prompt, objectives, locations, characters)\n" +
                                          "print(initial_generated_quest)\n";

                    string initialQuest = UtilityLibrary.FormatStringForPython(UtilityLibrary.RunPythonScript(initialQuestScript));

                    string questWithObjectivesScript = "import UnityEngine;\n" +
                                         "from campbell_quest import quest_generator\n" +
                                         "\n" +
                                         $"initial_generated_quest = \"{initialQuest}\"\n" +
                                         $"locations = \"{locations}\"\n" +
                                         $"characters = \"{characters}\"\n" +
                                         "quest_with_objectives = quest_generator.generate_quest_with_objectives(initial_generated_quest, locations, characters)\n" +
                                         "print(quest_with_objectives)\n";

                    string questWithObjectives = UtilityLibrary.FormatStringForPython(UtilityLibrary.RunPythonScript(questWithObjectivesScript));

                    string questRewardScript = "import UnityEngine;\n" +
                                         "from campbell_quest import quest_generator\n" +
                                         "\n" +
                                         $"initial_generated_quest = \"{initialQuest}\"\n" +
                                         $"rewards = \"{rewards}\"\n" +
                                         "quest_reward = quest_generator.generate_quest_reward(initial_generated_quest, rewards)\n" +
                                         "print(quest_reward)\n";

                    string questReward = UtilityLibrary.FormatStringForPython(UtilityLibrary.RunPythonScript(questRewardScript));

                    string formatQuestScript = "import UnityEngine;\n" +
                                          "from campbell_quest import quest_generator\n" +
                                          "\n" +
                                          $"quest_with_objectives = \"{questWithObjectives}\"\n" +
                                          $"quest_schema = \"{questSchema}\"\n" +
                                          "formatted_quest = quest_generator.get_formatted_quest(quest_with_objectives, quest_schema)\n" +
                                          "print(formatted_quest)\n";

                    formattedQuest = UtilityLibrary.RunPythonScript(formatQuestScript);

                    string formatQuestWithRewardsScript = "import UnityEngine;\n" +
                                          "from campbell_quest import quest_generator\n" +
                                          "\n" +
                                          $"formatted_quest = \"{UtilityLibrary.FormatStringForPython(formattedQuest)}\"\n" +
                                          $"quest_reward = \"{questReward}\"\n" +
                                          $"quest_schema_with_rewards = \"{questWithRewardsSchema}\"\n" +
                                          "formatted_quest_with_rewards = quest_generator.get_formatted_quest_with_rewards(formatted_quest, quest_reward, quest_schema_with_rewards)\n" +
                                          "print(formatted_quest_with_rewards)\n";

                    formattedQuestWithRewards = UtilityLibrary.RunPythonScript(formatQuestWithRewardsScript);

                    if (!string.IsNullOrWhiteSpace(formattedQuestWithRewards))
                    {
                        InitializeObjectivesList();
                        InitializeRewardsList();
                        return true;
                    }

                    Debug.LogWarning("No quest generated.");
                    return false;
                }

                return false;
            }

            return false;
        }

        private void InitializeObjectivesList()
        {
            QuestData questData = UtilityLibrary.DeserializeJson<QuestData>(formattedQuestWithRewards);
            
            _objectivesList = new ReorderableList(questData.objectives, typeof(ObjectiveData), true, true, true, true)
            {
                drawHeaderCallback = (Rect rect) => { EditorGUI.LabelField(rect, "Objectives"); },
                elementHeight = EditorGUIUtility.singleLineHeight * 2 + 5f,
                drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    ObjectiveData objective = questData.objectives[index];
                    rect.y += 2.5f;

                    objective.reference = EditorGUI.TextField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), "Reference",
                            objective.reference);
                    objective.reference = (index + 1).ToString();

                    rect.y += EditorGUIUtility.singleLineHeight + 2.5f;

                    objective.description =
                        EditorGUI.TextField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), "Description",
                            objective.description);
                }
            };
        }
        
        private void InitializeRewardsList()
        {
            QuestData questData = UtilityLibrary.DeserializeJson<QuestData>(formattedQuestWithRewards);
            
            _rewardsList = new ReorderableList(questData.rewards, typeof(RewardData), true, true, true, true)
            {
                drawHeaderCallback = (Rect rect) => { EditorGUI.LabelField(rect, "Rewards"); },
                elementHeight = EditorGUIUtility.singleLineHeight * 2 + 5f,
                drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    RewardData reward = questData.rewards[index];
                    rect.y += 2.5f;

                    reward.number =
                        EditorGUI.IntField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), "Number",
                            reward.number);
                    rect.y += EditorGUIUtility.singleLineHeight + 2.5f;

                    reward.item =
                        EditorGUI.TextField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), "Item",
                            reward.item);
                }
            };
        }

        public void DisplayQuestInformation()
        {
            QuestData questData = UtilityLibrary.DeserializeJson<QuestData>(formattedQuestWithRewards);
            if (questData == null)
            {
                EditorGUILayout.HelpBox("Ollama response does not match required Schema. Try to generate quest again.", MessageType.Warning);
            }
            else
            {
                questData.name = EditorGUILayout.TextField("Quest Name", questData.name);

                EditorGUILayout.Space();

                questData.description = EditorGUILayout.TextField("Description", questData.description);

                EditorGUILayout.Space();

                questData.goal = EditorGUILayout.TextField("Goal", questData.goal);

                EditorGUILayout.Space();

                if (_objectivesList == null)
                {
                   InitializeObjectivesList();
                }
                _objectivesList.DoLayoutList();
                questData.objectives = new List<ObjectiveData>(_objectivesList.list.Cast<ObjectiveData>());
                
                EditorGUILayout.Space();

                if (_rewardsList == null)
                {
                    InitializeRewardsList();
                }
                _rewardsList.DoLayoutList();
                questData.rewards = new List<RewardData>(_rewardsList.list.Cast<RewardData>());

                QuestMetadataFormat questMetadataFormat = UtilityLibrary.DeserializeJson<QuestMetadataFormat>(formattedQuest);
                questMetadataFormat.title = "Quest";
                questMetadataFormat.name = questData.name;
                questMetadataFormat.description = questData.description;
                questMetadataFormat.goal = questData.goal;
                List<ObjectiveData> objectives = new List<ObjectiveData>();
                foreach (ObjectiveData objective in questData.objectives)
                {
                    objectives.Add(new ObjectiveData { reference = objective.reference, description = objective.description });
                }
                questMetadataFormat.objectives = objectives;

                formattedQuest = JsonConvert.SerializeObject(questMetadataFormat);

                formattedQuestWithRewards = JsonConvert.SerializeObject(questData);
            }
        }

        public bool ClearQuest()
        {
            if (GUILayout.Button("Clear Quest"))
            {
                formattedQuestWithRewards = null;
                return true;
            }

            return false;
        }

        public void CreateQuestAsset(Quest.QuestMetadata metadata)
        {
            if (GUILayout.Button("Create Quest Assets"))
            {
                QuestGenerator.CreateQuestFromJson(formattedQuestWithRewards, metadata);
            }
        }
        
        public void RecreateQuestAsset(Quest.QuestMetadata metadata)
        {
            if (GUILayout.Button("Recreate Quest Assets"))
            {
                QuestGenerator.CreateQuestFromJson(formattedQuestWithRewards, metadata);
            }
        }
    }
}