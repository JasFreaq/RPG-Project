using System.IO;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

namespace Campbell.Editor.QuestGeneration
{
    [InitializeOnLoad]
    public class CampbellPackageChecker
    {
        private const string _packageName = "com.jasfreaq.campbell-quest";

        private const string _requirementsFilePath = "ProjectSettings/requirements.txt";
        private static readonly string _stringToAppend = "campbell_quest";

        static CampbellPackageChecker()
        {
            if (!CheckRequirement())
            {
                CheckPackage();
            }
        }

        private static bool CheckRequirement()
        {
            if (!File.Exists(_requirementsFilePath))
            {
                return false;
            }

            using (StreamReader reader = new StreamReader(_requirementsFilePath))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (line.Contains(_stringToAppend))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private static void CheckPackage()
        {
            ListRequest listRequest = Client.List();

            EditorApplication.CallbackFunction updateDelegate = null;

            updateDelegate = () =>
            {
                if (listRequest.IsCompleted)
                {
                    if (listRequest.Status == StatusCode.Success)
                    {
                        bool packageInstalled = false;
                        foreach (var package in listRequest.Result)
                        {
                            if (package.name == _packageName)
                            {
                                packageInstalled = true;
                                break;
                            }
                        }

                        if (packageInstalled)
                        {
                            Debug.Log($"Package {_packageName} is installed.");

                            ProcessRequirementsUpdate();
                        }
                        else
                        {
                            Debug.LogWarning($"Package {_packageName} is not installed.");
                        }
                    }
                    else if (listRequest.Status >= StatusCode.Failure)
                    {
                        Debug.LogError($"Failed to list packages: {listRequest.Error.message}");
                    }

                    // Clean up the delegate reference
                    EditorApplication.update -= updateDelegate;
                }
            };

            // Add the delegate to EditorApplication.update
            EditorApplication.update += updateDelegate;
        }

        private static void ProcessRequirementsUpdate()
        {
            UpdateRequirementsFile();

            RestartEditorPrompt.ShowWindow();
        }

        private static void UpdateRequirementsFile()
        {
            if (!File.Exists(_requirementsFilePath))
            {
                // Create the file and write the initial string
                File.WriteAllText(_requirementsFilePath, _stringToAppend + "\n");
                Debug.Log($"Created {_requirementsFilePath} and added initial content.");
            }
            else
            {
                // Append the string to the file
                File.AppendAllText(_requirementsFilePath, _stringToAppend + "\n");
                Debug.Log($"Appended to {_requirementsFilePath}.");
            }

            // Refresh the AssetDatabase to reflect changes in the Unity editor
            AssetDatabase.Refresh();
        }
    }
}