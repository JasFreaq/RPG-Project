using System.Collections.Generic;
using Campbell.Editor.QuestGeneration.Utility;
using Unity.Plastic.Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace Campbell.Editor.QuestGeneration
{
    /// <summary>
    /// The DialogueProcessor class is responsible for generating and managing dialogues within the quest generation editor.
    /// </summary>
    public class DialogueProcessor
    {
        /// <summary>
        /// The currently selected model type for dialogue generation.
        /// </summary>
        public ModelType selectedModel = 0;

        /// <summary>
        /// Generates dialogues based on the provided quest, location, and character information.
        /// </summary>
        /// <param name="formattedQuest">The formatted quest string to be used in dialogue generation.</param>
        /// <param name="locationInformation">Information about the locations involved in the quest.</param>
        /// <param name="characterInformation">Information about the characters involved in the quest.</param>
        /// <param name="formattedDialogues">A reference to a list where the generated dialogues will be stored.</param>
        public void GenerateDialogues(string formattedQuest, string locationInformation, string characterInformation, ref List<string> formattedDialogues)
        {
            selectedModel = (ModelType)EditorGUILayout.EnumPopup("Model", selectedModel);

            if (GUILayout.Button("Generate Dialogues"))
            {
                if (formattedDialogues.Count > 0)
                {
                    formattedDialogues.Clear();
                }

                string quest = UtilityLibrary.FormatStringForPython(formattedQuest);
                string locations = UtilityLibrary.FormatStringForPython(locationInformation);
                string characters = UtilityLibrary.FormatStringForPython(characterInformation);
                string requiredDialoguesSchema = UtilityLibrary.FormatStringForPython(Resources.Load<TextAsset>("campbellRequiredDialoguesSchema").text);
                string dialogueTemplate = UtilityLibrary.FormatStringForPython(Resources.Load<TextAsset>("campbellDialogueTemplate").text);
                string model = UtilityLibrary.GetModelString(selectedModel);

                string dialogueScript = "import UnityEngine;\n" +
                                        "from campbell_quest import dialogue_generator\n" +
                                        "\n" +
                                        $"formatted_quest = \"{quest}\"\n" +
                                        $"required_dialogues_schema = \"{requiredDialoguesSchema}\"\n" +
                                        $"example_dialogue = \"{dialogueTemplate}\"\n" +
                                        $"locations = \"{locations}\"\n" +
                                        $"characters = \"{characters}\"\n" +
                                        $"model = \"{model}\"\n" +
                                        "dialogues = dialogue_generator.get_dialogues(formatted_quest, required_dialogues_schema, example_dialogue, locations, characters, model)\n" +
                                        "for dialogue in dialogues:\n" +
                                        "\tprint(dialogue)\n" +
                                        "\tprint(\"@\")\n";

                string dialogues = UtilityLibrary.RunPythonScript(dialogueScript);

                List<string> dialoguesBuilder = new List<string>();
                formattedDialogues.AddRange(dialogues.Split('@'));

                foreach (string dialogue in formattedDialogues)
                {
                    if (!string.IsNullOrWhiteSpace(dialogue))
                    {
                        dialoguesBuilder.Add(dialogue);
                    }
                }

                formattedDialogues = dialoguesBuilder;

                if (formattedDialogues.Count == 0)
                {
                    Debug.LogWarning("No dialogues generated.");
                }
            }
        }

        /// <summary>
        /// Displays information about a dialogue in a custom inspector.
        /// </summary>
        /// <param name="dialogueJson">The JSON string containing dialogue data.</param>
        /// <returns>The modified dialogue JSON string.</returns>
        public string DisplayDialogueInformation(string dialogueJson)
        {
            DialogueData dialogueData = UtilityLibrary.DeserializeJson<DialogueData>(dialogueJson);

            // Start custom inspector
            EditorGUILayout.LabelField("NPC Name", EditorStyles.boldLabel);
            dialogueData.npc_name = EditorGUILayout.TextField(dialogueData.npc_name);
            EditorGUILayout.LabelField("NPC Dialogue", EditorStyles.boldLabel);
            dialogueData.npc_dialogue = EditorGUILayout.TextField(dialogueData.npc_dialogue);
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Choices", EditorStyles.boldLabel);

            if (dialogueData.choices != null)
            {
                DrawChoices(dialogueData.choices, 0);
            }

            // Apply changes
            if (GUI.changed)
            {
                dialogueJson = JsonConvert.SerializeObject(dialogueData);
            }

            return dialogueJson;
        }

        /// <summary>
        /// Recursively draws the choices in the dialogue editor.
        /// </summary>
        /// <param name="choices">A list of choices to be drawn.</param>
        /// <param name="indentLevel">The current indentation level for nested choices.</param>
        private void DrawChoices(List<ChoiceData> choices, int indentLevel)
        {
            EditorGUI.indentLevel = indentLevel;
            for (int i = 0; i < choices.Count; i++)
            {
                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.LabelField($"Choice {i + 1}", EditorStyles.boldLabel);
                choices[i].condition = EditorGUILayout.TextField("Condition", choices[i].condition);
                choices[i].player_dialogue = EditorGUILayout.TextField("Player Dialogue", choices[i].player_dialogue);
                choices[i].npc_dialogue = EditorGUILayout.TextField("NPC Dialogue", choices[i].npc_dialogue);
                choices[i].result = EditorGUILayout.TextField("Result", choices[i].result);

                if (choices[i].choices != null && choices[i].choices.Count > 0)
                {
                    EditorGUILayout.LabelField("Nested Choices", EditorStyles.boldLabel);
                    DrawChoices(choices[i].choices, indentLevel + 1);
                }

                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Add Nested Choice"))
                {
                    if (choices[i].choices == null)
                    {
                        choices[i].choices = new List<ChoiceData>();
                    }
                    choices[i].choices.Add(new ChoiceData());
                }
                if (GUILayout.Button("Remove Choice"))
                {
                    choices.RemoveAt(i);
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();
            }

            if (GUILayout.Button("Add Choice"))
            {
                choices.Add(new ChoiceData());
            }
        }

        /// <summary>
        /// Clears the list of formatted dialogues.
        /// </summary>
        /// <param name="formattedDialogues">A reference to the list of dialogues to be cleared.</param>
        /// <returns>True if dialogues were cleared, otherwise false.</returns>
        public bool ClearDialogues(ref List<string> formattedDialogues)
        {
            if (GUILayout.Button("Clear Dialogues"))
            {
                formattedDialogues.Clear();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Creates a dialogue asset from a JSON string at the specified save path.
        /// </summary>
        /// <param name="dialogue">The JSON string representing the dialogue.</param>
        /// <param name="dialogueAssetSavePath">The path where the dialogue asset will be saved.</param>
        public void CreateDialogueAsset(string dialogue, string dialogueAssetSavePath)
        {
            if (GUILayout.Button("Create Dialogue Assets"))
            {
                DialogueGenerator.CreateDialogueFromJson(dialogue, dialogueAssetSavePath);
            }
        }

        /// <summary>
        /// Recreates an existing dialogue asset from a JSON string at the specified save path.
        /// </summary>
        /// <param name="dialogue">The JSON string representing the dialogue.</param>
        /// <param name="dialogueAssetSavePath">The path where the dialogue asset will be saved.</param>
        public void RecreateDialogueAsset(string dialogue, string dialogueAssetSavePath)
        {
            if (GUILayout.Button("Recreate Dialogue Assets"))
            {
                DialogueGenerator.CreateDialogueFromJson(dialogue, dialogueAssetSavePath);
            }
        }
    }
}
