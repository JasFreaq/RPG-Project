using Campbell.Editor.Utility;
using RPG.Quests.Editor;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Campbell.Editor
{
    public class QuestProcessor
    {
        private string _questPrompt;
        private string _objectiveInformation;
        private string _locationInformation;
        private string _characterInformation;
        private string _rewardInformation;

        private Vector2 _questPromptScrollPosition;
        private Vector2 _objectiveScrollPosition;
        private Vector2 _locationScrollPosition;
        private Vector2 _characterScrollPosition;
        private Vector2 _rewardScrollPosition;

        public string LocationInformation => _locationInformation;

        public string CharacterInformation => _characterInformation;

        public void PopulateFromSamples(string sampleFilesPath)
        {
            if (GUILayout.Button("Populate With Samples"))
            {
                _questPrompt = LoadSampleFile($"{sampleFilesPath}/example_prompt.txt");
                _objectiveInformation = LoadSampleFile($"{sampleFilesPath}/example_objectives.json");
                _locationInformation = LoadSampleFile($"{sampleFilesPath}/example_locations.json");
                _characterInformation = LoadSampleFile($"{sampleFilesPath}/example_characters.json");
                _rewardInformation = LoadSampleFile($"{sampleFilesPath}/example_rewards.json");
            }
        }

        private string LoadSampleFile(string path)
        {
            if (File.Exists(path))
            {
                return File.ReadAllText(path);
            }
            else
            {
                Debug.LogError($"Sample file not found at {path}");
                return string.Empty;
            }
        }

        public void DisplayQuestPrompt()
        {
            DisplayTextArea("Quest Prompt", ref _questPrompt, ref _questPromptScrollPosition);
        }

        public void DisplayObjectiveInformation()
        {
            DisplayTextArea("Objective Information", ref _objectiveInformation, ref _objectiveScrollPosition);
        }

        public void DisplayLocationInformation()
        {
            DisplayTextArea("Location Information", ref _locationInformation, ref _locationScrollPosition);
        }

        public void DisplayCharacterInformation()
        {
            DisplayTextArea("Character Information", ref _characterInformation, ref _characterScrollPosition);
        }

        public void DisplayRewardInformation()
        {
            DisplayTextArea("Reward Information", ref _rewardInformation, ref _rewardScrollPosition);
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
                { "Quest Prompt", _questPrompt },
                { "Objective Information", _objectiveInformation },
                { "Location Information", _locationInformation },
                { "Character Information", _characterInformation },
                { "Reward Information", _rewardInformation },
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


        public bool GenerateQuest(ref string formattedQuest, ref string formattedQuestWithRewards)
        {
            if (IsAllContextValid())
            {
                if (GUILayout.Button("Generate Quest"))
                {
                    if (!(string.IsNullOrEmpty(formattedQuestWithRewards) || string.IsNullOrWhiteSpace(formattedQuestWithRewards)))
                    {
                        formattedQuestWithRewards = null;
                    }

                    string prompt = UtilityLibrary.FormatStringForPython(_questPrompt);
                    string objectives = UtilityLibrary.FormatStringForPython(_objectiveInformation);
                    string locations = UtilityLibrary.FormatStringForPython(_locationInformation);
                    string characters = UtilityLibrary.FormatStringForPython(_characterInformation);
                    string rewards = UtilityLibrary.FormatStringForPython(_rewardInformation);

                    string questSchema = UtilityLibrary.LoadSchema("quest");
                    string questWithRewardsSchema = UtilityLibrary.LoadSchema("questWithRewards");

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

                    string questWithRewardScript = "import UnityEngine;\n" +
                                         "from campbell_quest import quest_generator\n" +
                                         "\n" +
                                         $"initial_generated_quest = \"{initialQuest}\"\n" +
                                         $"rewards = \"{rewards}\"\n" +
                                         "quest_reward = quest_generator.generate_quest_reward(initial_generated_quest, rewards)\n" +
                                         "print(quest_reward)\n";

                    string questReward = UtilityLibrary.FormatStringForPython(UtilityLibrary.RunPythonScript(questWithRewardScript));

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
                    return true;
                }

                return false;
            }

            return false;
        }

        public void ClearQuest(ref string formattedQuestWithRewards)
        {
            if (GUILayout.Button("Clear Quest"))
            {
                formattedQuestWithRewards = null;
            }
        }

        public void CreateQuestAssets(string formattedQuestWithRewards, string questAssetSavePath, ref string generatedQuestName)
        {
            if (GUILayout.Button("Create Quest Assets"))
            {
                generatedQuestName =
                    AssetGenerator.CreateQuestFromJson(formattedQuestWithRewards, questAssetSavePath);
            }
        }
        
        public void RecreateQuestAssets(string formattedQuestWithRewards, string questAssetSavePath, ref string generatedQuestName)
        {
            if (GUILayout.Button("Recreate Quest Assets"))
            {
                generatedQuestName =
                    AssetGenerator.CreateQuestFromJson(formattedQuestWithRewards, questAssetSavePath);
            }
        }
    }
}
