using System.Collections.Generic;
using Campbell.Editor.QuestGeneration.Utility;
using Unity.Plastic.Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace Campbell.Editor.QuestGeneration
{
    public class DialogueProcessor
    {
        private bool _canNpcFight = false;

        public bool GenerateDialogues(string formattedQuest, string locationInformation, string characterInformation, ref List<string> formattedDialogues)
        {
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
                return true;
            }

            return false;
        }

        public string DisplayDialogueInformation(string dialogueJson)
        {
            DialogueData dialogueData = JsonConvert.DeserializeObject<DialogueData>(dialogueJson);

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

            //GUIStyle textStyle = new GUIStyle(EditorStyles.textField)
            //{
            //    padding = new RectOffset(5, 5, 5, 5),
            //    wordWrap = true
            //};

            //dialogue = EditorGUILayout.TextArea(dialogue, textStyle, GUILayout.MinWidth(100),
            //    GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));

            //return dialogue;
        }

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

        public bool ClearDialogues(ref List<string> formattedDialogues)
        {
            if (GUILayout.Button("Clear Dialogues"))
            {
                formattedDialogues.Clear();
                return true;
            }

            return false;
        }

        public void CreateDialogueAsset(string dialogue, string questAssetSavePath, string generatedQuestName)
        {
            if (GUILayout.Button("Create Dialogue Assets"))
            {
                string dialogueSavePath = questAssetSavePath + "/" + generatedQuestName;
                DialogueGenerator.CreateDialogueFromJson(dialogue, dialogueSavePath);
            }
        }

        public void RecreateDialogueAsset(string dialogue, string questAssetSavePath, string generatedQuestName)
        {
            if (GUILayout.Button("Recreate Dialogue Assets"))
            {
                string dialogueSavePath = questAssetSavePath + "/" + generatedQuestName;
                DialogueGenerator.CreateDialogueFromJson(dialogue, dialogueSavePath);
            }
        }

        public void CreateNpcAsset(string dialogue, string questAssetSavePath, string generatedQuestName)
        {
            if (GUILayout.Button("Create Npc Asset"))
            {
                string dialogueSavePath = questAssetSavePath + "/" + generatedQuestName;
                //CreateDialogueFromJson(dialogue, dialogueSavePath);
            }
        }
    }
}