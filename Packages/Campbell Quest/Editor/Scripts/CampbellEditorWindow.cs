using Python.Runtime;
using System;
using System.IO;
using System.Text;
using Campbell.Editor.Utility;
using UnityEditor;
using UnityEditor.Scripting.Python;
using UnityEngine;
using MacFsWatcher;
using RPG.Quests.Editor;
using UnityEngine.UIElements;

namespace Campbell.Editor
{
    public class CampbellEditorWindow : EditorWindow
    {
        int _selectedTab = 0;
        string[] _tabNames = { "Context Editor", "Quest Editor" };

        private const string _sampleFilesPath = "Packages/Campbell Quest/Context Samples";
        private const string _questAssetSavePath = "Assets/Campbell Generated Quests/Resources/Quests";

        private string _generatedQuest;
        private string _questPrompt;
        
        private string _objectiveInformation;
        private string _locationInformation;
        private string _characterInformation;
        private string _rewardInformation;

        private Vector2 _contextScrollPosition;
        private Vector2 _questScrollPosition;

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
            _selectedTab = GUILayout.Toolbar(_selectedTab, _tabNames);

            if (_selectedTab == 0)
            {
                FormatContextWindow();
            }
            else if (_selectedTab == 1)
            {
                FormatQuestWindow();
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

            EditorGUILayout.EndScrollView();
        }

        private void FormatQuestWindow()
        {
            if (_generatedQuest != null)
            {
                EditorGUILayout.BeginHorizontal();

                GenerateQuest();

                EditorGUILayout.Space();

                ClearQuest();

                EditorGUILayout.EndHorizontal();
            }
            else
            {
                GenerateQuest();
            }

            EditorGUILayout.Space();

            DisplayGeneratedQuest();

            if (_generatedQuest != null)
            {
                EditorGUILayout.Space();
                CreateQuestAssets();
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
                string prompt = UtilityLibrary.FormatStringForPython(_questPrompt);
                string objectives = UtilityLibrary.FormatStringForPython(_objectiveInformation);
                string locations = UtilityLibrary.FormatStringForPython(_locationInformation);
                string characters = UtilityLibrary.FormatStringForPython(_characterInformation);
                string rewards = UtilityLibrary.FormatStringForPython(_rewardInformation);

                string questSchema = UtilityLibrary.LoadSchema("quest");

                string pythonScript = "import UnityEngine;\n" +
                                      "from campbell_quest import quest_generator\n" +
                                      "\n" +
                                      $"prompt = \"{prompt}\"\n" +
                                      $"schema = \"{questSchema}\"\n" +
                                      $"objectives = \"{objectives}\"\n" +
                                      $"locations = \"{locations}\"\n" +
                                      $"characters = \"{characters}\"\n" +
                                      $"rewards = \"{rewards}\"\n" +
                                      "quest = quest_generator.generate_quest(prompt, schema, objectives, locations, characters, rewards)\n" +
                                      "print(quest)\n";

                using StringWriter stringWriter = new StringWriter();
                using (Py.GIL())
                {
                    dynamic sys = Py.Import("sys");
                    sys.stdout = new CampbellTextWriter(stringWriter);
                    PythonRunner.RunString(pythonScript);
                }
                
                _generatedQuest = stringWriter.ToString();
            }
        }
        
        private void ClearQuest()
        {
            if (GUILayout.Button("Clear Quest"))
            {
                _generatedQuest = null;
            }
        }

        private void DisplayGeneratedQuest()
        {
            GUIStyle textStyle = new GUIStyle(EditorStyles.textField);
            textStyle.padding = new RectOffset(5, 5, 5, 5);
            textStyle.wordWrap = true;

            _questScrollPosition = EditorGUILayout.BeginScrollView(_questScrollPosition, GUILayout.ExpandHeight(true));
            
            _generatedQuest = EditorGUILayout.TextArea(_generatedQuest, textStyle, GUILayout.MinWidth(100),
                GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));

            EditorGUILayout.EndScrollView();
        }
        
        private void CreateQuestAssets()
        {
            if (GUILayout.Button("Create Quest Assets"))
            {
                QuestGenerator.CreateQuestFromJson(_generatedQuest, _questAssetSavePath);
            }
        }
    }
}