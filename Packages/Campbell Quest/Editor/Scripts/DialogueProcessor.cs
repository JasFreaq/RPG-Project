using System.Collections.Generic;
using Campbell.Editor.Utility;
using RPG.Quests.Editor;
using UnityEditor;
using UnityEngine;

namespace Campbell.Editor
{
    public class DialogueProcessor
    {
        public void GenerateDialogues(string formattedQuest, string locationInformation, string characterInformation, ref List<string> formattedDialogues)
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
            }
        }

        public void ClearDialogues(ref List<string> formattedDialogues)
        {
            if (GUILayout.Button("Clear Dialogues"))
            {
                formattedDialogues.Clear();
            }
        }

        public void CreateDialogueAssets(List<string> formattedDialogues, string questAssetSavePath, string generatedQuestName)
        {
            if (GUILayout.Button("Create Dialogue Assets"))
            {
                foreach (var dialogue in formattedDialogues)
                {
                    string dialogueSavePath = questAssetSavePath + "/" + generatedQuestName;
                    AssetGenerator.CreateDialogueFromJson(dialogue, dialogueSavePath);
                }
            }
        }
    }
}