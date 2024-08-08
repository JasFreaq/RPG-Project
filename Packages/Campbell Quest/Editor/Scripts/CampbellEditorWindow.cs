using System.Collections;
using Python.Runtime;
using System.IO;
using Campbell.Editor.Utility;
using UnityEditor;
using UnityEditor.Scripting.Python;
using UnityEngine;
using RPG.Quests.Editor;
using System.Collections.Generic;

namespace Campbell.Editor
{
    public class CampbellEditorWindow : EditorWindow
    {
        int _selectedTab = 0;
        string[] _initialTabNames = { "Context Editor", "Quest Editor" };

        private const string _sampleFilesPath = "Packages/Campbell Quest/Context Samples";
        private const string _questAssetSavePath = "Assets/Campbell Generated Quests/Resources";
        
        private string _formattedQuest;
        private string _formattedQuestWithRewards;
        private List<string> _formattedDialogues = new List<string>();
        private string _generatedQuestName;

        private string _questPrompt;
        private string _objectiveInformation;
        private string _locationInformation;
        private string _characterInformation;
        private string _rewardInformation;

        private Vector2 _contextScrollPosition;
        private Vector2 _questScrollPosition;
        private Vector2 _dialoguesScrollPosition;

        private Vector2 _questPromptScrollPosition;
        private Vector2 _objectiveScrollPosition;
        private Vector2 _locationScrollPosition;
        private Vector2 _characterScrollPosition;
        private Vector2 _rewardScrollPosition;

        [MenuItem("Campbell/Campbell Editor Window")]
        public static void ShowWindow()
        {
            GetWindow<CampbellEditorWindow>("Campbell Editor");
        }

        private void OnGUI()
        {
            string[] tabNames = _initialTabNames;
            if (_formattedDialogues.Count > 0)
            {
                tabNames = new string[_initialTabNames.Length + 1];
                _initialTabNames.CopyTo(tabNames, 0);
                tabNames[_initialTabNames.Length] = "Dialogue Editor";
            }

            _selectedTab = GUILayout.Toolbar(_selectedTab, tabNames);

            if (_selectedTab == 0)
            {
                FormatContextWindow();
            }
            else if (_selectedTab == 1)
            {
                FormatQuestWindow();
            }
            else if (_selectedTab == 2)
            {
                FormatDialogueWindow();
            }
        }

        private void FormatContextWindow()
        {
            _contextScrollPosition = EditorGUILayout.BeginScrollView(_contextScrollPosition, GUILayout.ExpandHeight(true));
            
            EditorGUILayout.Space();

            PopulateFromSamples();

            EditorGUILayout.Space();

            DisplayQuestPrompt();
            
            EditorGUILayout.Space();

            DisplayObjectiveInformation();

            EditorGUILayout.Space();

            DisplayLocationInformation();

            EditorGUILayout.Space();

            DisplayCharacterInformation();

            EditorGUILayout.Space();

            DisplayRewardInformation();

            EditorGUILayout.Space();

            GenerateQuest();

            EditorGUILayout.EndScrollView();
        }

        private void FormatQuestWindow()
        {
            if (!(string.IsNullOrEmpty(_formattedQuestWithRewards) || string.IsNullOrWhiteSpace(_formattedQuestWithRewards)))
            {
                ClearQuest();
            }
            else
            {
                GenerateQuest();
            }

            EditorGUILayout.Space();

            DisplayGeneratedQuest();

            if (!(string.IsNullOrEmpty(_formattedQuestWithRewards) || string.IsNullOrWhiteSpace(_formattedQuestWithRewards)))
            {
                EditorGUILayout.Space();

                EditorGUILayout.BeginHorizontal();

                CreateQuestAssets();

                EditorGUILayout.Space();

                GenerateDialogues();

                EditorGUILayout.EndHorizontal();
            }
        }
        
        private void FormatDialogueWindow()
        {
            if (_formattedDialogues.Count > 0)
            {
                ClearDialogues();
            }
            else
            {
                GenerateDialogues();
            }

            EditorGUILayout.Space();

            DisplayGeneratedDialogues();

            if (_formattedDialogues.Count > 0)
            {
                EditorGUILayout.Space();
                
                CreateDialogueAssets();
            }
        }

#region Context Window

        private void PopulateFromSamples()
        {
            if (GUILayout.Button("Populate With Samples")) // Create a button
            {
                string promptSamplePath = $"{_sampleFilesPath}/example_prompt.txt";
                if (File.Exists(promptSamplePath))
                {
                    string fileContent = File.ReadAllText(promptSamplePath);
                    _questPrompt = fileContent;
                }
                else
                {
                    Debug.LogError("Schema sample file not found");
                }

                string objectiveSamplePath = $"{_sampleFilesPath}/example_objectives.json";
                if (File.Exists(objectiveSamplePath))
                {
                    string fileContent = File.ReadAllText(objectiveSamplePath);
                    _objectiveInformation = fileContent;
                }
                else
                {
                    Debug.LogError("Objective sample file not found");
                }

                string locationSamplePath = $"{_sampleFilesPath}/example_locations.json";
                if (File.Exists(locationSamplePath))
                {
                    string fileContent = File.ReadAllText(locationSamplePath);
                    _locationInformation = fileContent;
                }
                else
                {
                    Debug.LogError("Location sample file not found");
                }
                
                string characterSamplePath = $"{_sampleFilesPath}/example_characters.json";
                if (File.Exists(characterSamplePath))
                {
                    string fileContent = File.ReadAllText(characterSamplePath);
                    _characterInformation = fileContent;
                }
                else
                {
                    Debug.LogError("Character sample file not found");
                }
                
                string rewardSamplePath = $"{_sampleFilesPath}/example_rewards.json";
                if (File.Exists(rewardSamplePath))
                {
                    string fileContent = File.ReadAllText(rewardSamplePath);
                    _rewardInformation = fileContent;
                }
                else
                {
                    Debug.LogError("Rewards sample file not found");
                }
            }
        }

        private void DisplayQuestPrompt()
        {
            GUIStyle textStyle = new GUIStyle(EditorStyles.textField);
            textStyle.padding = new RectOffset(5, 5, 5, 5);
            textStyle.wordWrap = true;

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField("Quest Prompt");

            if (GUILayout.Button("Browse")) // Create a button
            {
                string path = EditorUtility.OpenFilePanel("Browse Quest Prompt", Application.dataPath, "png,txt,json");

                if (path.Length != 0)
                {
                    string fileContent = File.ReadAllText(path);
                    _questPrompt = fileContent;
                }
            }

            EditorGUILayout.EndHorizontal();
            
            // Begin a scroll view
            _questPromptScrollPosition = EditorGUILayout.BeginScrollView(_questPromptScrollPosition, GUILayout.Height(75));

            // Text area with flexible height
            _questPrompt = EditorGUILayout.TextArea(_questPrompt, textStyle, GUILayout.MinWidth(100),
                GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));

            // End the scroll view
            EditorGUILayout.EndScrollView();
        }

        private void DisplayObjectiveInformation()
        {
            GUIStyle textStyle = new GUIStyle(EditorStyles.textField);
            textStyle.padding = new RectOffset(5, 5, 5, 5);
            textStyle.wordWrap = true;

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField("Objective Information");

            if (GUILayout.Button("Browse")) // Create a button
            {
                string path =
                    EditorUtility.OpenFilePanel("Browse Objective File", Application.dataPath, "png,txt,json");

                if (path.Length != 0)
                {
                    string fileContent = File.ReadAllText(path);
                    _objectiveInformation = fileContent;
                }
            }

            EditorGUILayout.EndHorizontal();

            // Begin a scroll view
            _objectiveScrollPosition = EditorGUILayout.BeginScrollView(_objectiveScrollPosition, GUILayout.Height(150));

            // Text area with flexible height
            _objectiveInformation = EditorGUILayout.TextArea(_objectiveInformation, textStyle, GUILayout.MinWidth(100),
                GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));

            // End the scroll view
            EditorGUILayout.EndScrollView();
        }

        private void DisplayLocationInformation()
        {
            GUIStyle textStyle = new GUIStyle(EditorStyles.textField);
            textStyle.padding = new RectOffset(5, 5, 5, 5);
            textStyle.wordWrap = true;

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField("Location Information");

            if (GUILayout.Button("Browse")) // Create a button
            {
                string path = EditorUtility.OpenFilePanel("Browse Location File", Application.dataPath, "png,txt,json");

                if (path.Length != 0)
                {
                    string fileContent = File.ReadAllText(path);
                    _locationInformation = fileContent;
                }
            }

            EditorGUILayout.EndHorizontal();

            // Begin a scroll view
            _locationScrollPosition = EditorGUILayout.BeginScrollView(_locationScrollPosition, GUILayout.Height(150));

            // Text area with flexible height
            _locationInformation = EditorGUILayout.TextArea(_locationInformation, textStyle, GUILayout.MinWidth(100),
                GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));

            // End the scroll view
            EditorGUILayout.EndScrollView();
        }
        
        private void DisplayCharacterInformation()
        {
            GUIStyle textStyle = new GUIStyle(EditorStyles.textField);
            textStyle.padding = new RectOffset(5, 5, 5, 5);
            textStyle.wordWrap = true;

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField("Character Information");

            if (GUILayout.Button("Browse")) // Create a button
            {
                string path = EditorUtility.OpenFilePanel("Browse Character File", Application.dataPath, "png,txt,json");

                if (path.Length != 0)
                {
                    string fileContent = File.ReadAllText(path);
                    _characterInformation = fileContent;
                }
            }

            EditorGUILayout.EndHorizontal();

            // Begin a scroll view
            _characterScrollPosition = EditorGUILayout.BeginScrollView(_characterScrollPosition, GUILayout.Height(150));

            // Text area with flexible height
            _characterInformation = EditorGUILayout.TextArea(_characterInformation, textStyle, GUILayout.MinWidth(100),
                GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));

            // End the scroll view
            EditorGUILayout.EndScrollView();
        }
        
        private void DisplayRewardInformation()
        {
            GUIStyle textStyle = new GUIStyle(EditorStyles.textField);
            textStyle.padding = new RectOffset(5, 5, 5, 5);
            textStyle.wordWrap = true;

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField("Reward Information");

            if (GUILayout.Button("Browse")) // Create a button
            {
                string path = EditorUtility.OpenFilePanel("Browse Reward File", Application.dataPath, "png,txt,json");

                if (path.Length != 0)
                {
                    string fileContent = File.ReadAllText(path);
                    _rewardInformation = fileContent;
                }
            }

            EditorGUILayout.EndHorizontal();

            // Begin a scroll view
            _rewardScrollPosition = EditorGUILayout.BeginScrollView(_rewardScrollPosition, GUILayout.Height(150));

            // Text area with flexible height
            _rewardInformation = EditorGUILayout.TextArea(_rewardInformation, textStyle, GUILayout.MinWidth(100),
                GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));

            // End the scroll view
            EditorGUILayout.EndScrollView();
        }

#endregion

        private void GenerateQuest()
        {
            if (GUILayout.Button("Generate Quest"))
            {
                if (!(string.IsNullOrEmpty(_formattedQuestWithRewards) || string.IsNullOrWhiteSpace(_formattedQuestWithRewards)))
                {
                    _formattedQuestWithRewards = null;
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

                string initialQuest = UtilityLibrary.FormatStringForPython(RunPythonScript(initialQuestScript));

                string questWithObjectivesScript = "import UnityEngine;\n" +
                                     "from campbell_quest import quest_generator\n" +
                                     "\n" +
                                     $"initial_generated_quest = \"{initialQuest}\"\n" +
                                     $"locations = \"{locations}\"\n" +
                                     $"characters = \"{characters}\"\n" +
                                     "quest_with_objectives = quest_generator.generate_quest_with_objectives(initial_generated_quest, locations, characters)\n" +
                                     "print(quest_with_objectives)\n";

                string questWithObjectives = UtilityLibrary.FormatStringForPython(RunPythonScript(questWithObjectivesScript));

                string questWithRewardScript = "import UnityEngine;\n" +
                                     "from campbell_quest import quest_generator\n" +
                                     "\n" +
                                     $"initial_generated_quest = \"{initialQuest}\"\n" +
                                     $"rewards = \"{rewards}\"\n" +
                                     "quest_reward = quest_generator.generate_quest_reward(initial_generated_quest, rewards)\n" +
                                     "print(quest_reward)\n";

                string questReward = UtilityLibrary.FormatStringForPython(RunPythonScript(questWithRewardScript));

                string formatQuestScript = "import UnityEngine;\n" +
                                      "from campbell_quest import quest_generator\n" +
                                      "\n" +
                                      $"quest_with_objectives = \"{questWithObjectives}\"\n" +
                                      $"quest_schema = \"{questSchema}\"\n" +
                                      "formatted_quest = quest_generator.get_formatted_quest(quest_with_objectives, quest_schema)\n" +
                                      "print(formatted_quest)\n";

                _formattedQuest = RunPythonScript(formatQuestScript);

                string formatQuestWithRewardsScript = "import UnityEngine;\n" +
                                      "from campbell_quest import quest_generator\n" +
                                      "\n" +
                                      $"formatted_quest = \"{UtilityLibrary.FormatStringForPython(_formattedQuest)}\"\n" +
                                      $"quest_reward = \"{questReward}\"\n" +
                                      $"quest_schema_with_rewards = \"{questWithRewardsSchema}\"\n" +
                                      "formatted_quest_with_rewards = quest_generator.get_formatted_quest_with_rewards(formatted_quest, quest_reward, quest_schema_with_rewards)\n" +
                                      "print(formatted_quest_with_rewards)\n";

                _formattedQuestWithRewards = RunPythonScript(formatQuestWithRewardsScript);
            }
        }

        private void GenerateDialogues()
        {
            if (GUILayout.Button("Generate Dialogues"))
            {
                if (_formattedDialogues.Count > 0)
                {
                    _formattedDialogues.Clear();
                }

                string quest = UtilityLibrary.FormatStringForPython(_formattedQuest);
                string locations = UtilityLibrary.FormatStringForPython(_locationInformation);
                string characters = UtilityLibrary.FormatStringForPython(_characterInformation);
                
                string requiredDialoguesSchema = UtilityLibrary.LoadSchema("requiredDialogues");
                string dialogueTemplate = UtilityLibrary.LoadTemplate("dialogue");

                string dialogueScript = "import UnityEngine;\n" +
                                      "from campbell_quest import dialogue_generator\n" +
                                      "\n" +
                                      $"formatted_quest = \"{quest}\"\n" +
                                      $"required_dialogues_schema = \"{requiredDialoguesSchema}\"\n" +
                                      $"example_dialogue = \"{dialogueTemplate}\"\n" +
                                      $"locations = \"{locations}\"\n" +
                                      $"characters = \"{characters}\"\n" +
                                      "dialogues = dialogue_generator.get_dialogues(formatted_quest, required_dialogues_schema, example_dialogue, locations, characters)\n" +
                                      "for dialogue in dialogues:\n" +
                                      "\tprint(dialogue)\n" +
                                      "\tprint(\"@\")\n";

                string dialogues = RunPythonScript(dialogueScript);
                List<string> dialoguesBuilder = new List<string>();
                _formattedDialogues.AddRange(dialogues.Split('@'));
                foreach (string dialogue in _formattedDialogues)
                {
                    if (!string.IsNullOrWhiteSpace(dialogue))
                    {
                        dialoguesBuilder.Add(dialogue);
                    }
                }
                _formattedDialogues = dialoguesBuilder;
            }
        }

        private string RunPythonScript(string script)
        {
            using StringWriter stringWriter = new StringWriter();
            using (Py.GIL())
            {
                dynamic sys = Py.Import("sys");
                sys.stdout = new CampbellTextWriter(stringWriter);
                PythonRunner.RunString(script);
            }

            return stringWriter.ToString();
        }

        private void ClearQuest()
        {
            if (GUILayout.Button("Clear Quest"))
            {
                _formattedQuestWithRewards = null;
            }
        }
        
        private void ClearDialogues()
        {
            if (GUILayout.Button("Clear Dialogues"))
            {
                _formattedDialogues.Clear();
            }
        }

        private void DisplayGeneratedQuest()
        {
            GUIStyle textStyle = new GUIStyle(EditorStyles.textField);
            textStyle.padding = new RectOffset(5, 5, 5, 5);
            textStyle.wordWrap = true;

            _questScrollPosition = EditorGUILayout.BeginScrollView(_questScrollPosition, GUILayout.ExpandHeight(true));
            
            _formattedQuestWithRewards = EditorGUILayout.TextArea(_formattedQuestWithRewards, textStyle, GUILayout.MinWidth(100),
                GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));

            EditorGUILayout.EndScrollView();
        }
        
        private void DisplayGeneratedDialogues()
        {
            GUIStyle textStyle = new GUIStyle(EditorStyles.textField);
            textStyle.padding = new RectOffset(5, 5, 5, 5);
            textStyle.wordWrap = true;

            _dialoguesScrollPosition = EditorGUILayout.BeginScrollView(_dialoguesScrollPosition, GUILayout.ExpandHeight(true));
            
            for (int i = 0; i < _formattedDialogues.Count; i++)
            {
                _formattedDialogues[i] = EditorGUILayout.TextArea(_formattedDialogues[i], textStyle, GUILayout.MinWidth(100),
                    GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));

                EditorGUILayout.Space();
            }

            EditorGUILayout.EndScrollView();
        }
        
        private void CreateQuestAssets()
        {
            if (GUILayout.Button("Create Quest Assets"))
            {
                _generatedQuestName = AssetGenerator.CreateQuestFromJson(_formattedQuestWithRewards, _questAssetSavePath);
            }
        }
        
        private void CreateDialogueAssets()
        {
            if (GUILayout.Button("Create Dialogue Assets"))
            {
                foreach (string dialogue in _formattedDialogues)
                {
                    string dialogueSavePath = _questAssetSavePath + "/" + _generatedQuestName;
                    AssetGenerator.CreateDialogueFromJson(dialogue, dialogueSavePath);
                }
            }
        }
    }
}