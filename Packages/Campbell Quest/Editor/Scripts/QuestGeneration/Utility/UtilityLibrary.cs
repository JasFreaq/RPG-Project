using Python.Runtime;
using System.IO;
using System.Text;
using Unity.Plastic.Newtonsoft.Json;
using UnityEditor.Scripting.Python;
using UnityEngine;

namespace Campbell.Editor.QuestGeneration.Utility
{
    /// <summary>
    /// Utility library providing helper methods for model string conversion, 
    /// string formatting for Python, running Python scripts, and JSON deserialization.
    /// </summary>
    public class UtilityLibrary
    {
        /// <summary>
        /// Converts a given model type to its corresponding string representation.
        /// </summary>
        /// <param name="model">The model type to convert.</param>
        /// <returns>The string representation of the model type.</returns>
        public static string GetModelString(ModelType model)
        {
            return model switch
            {
                ModelType.Llama3_1 => "llama3.1",
                ModelType.Gemma2 => "gemma2",
                ModelType.MistralNemo => "mistral-nemo"
            };
        }

        /// <summary>
        /// Formats a given string for safe execution in Python by escaping special characters.
        /// </summary>
        /// <param name="input">The input string to format.</param>
        /// <returns>The formatted string with special characters escaped.</returns>
        public static string FormatStringForPython(string input)
        {
            if (input == null)
            {
                Debug.LogWarning("Input string is null.");
                return null;
            }

            StringBuilder formattedString = new StringBuilder();
            foreach (char c in input)
            {
                switch (c)
                {
                    case '\'':
                        formattedString.Append("\\'");
                        break;
                    case '\"':
                        formattedString.Append("\\\"");
                        break;
                    case '\\':
                        formattedString.Append("\\\\");
                        break;
                    case '\n':
                        formattedString.Append("\\n");
                        break;
                    case '\r':
                        formattedString.Append("\\r");
                        break;
                    case '\t':
                        formattedString.Append("\\t");
                        break;
                    default:
                        formattedString.Append(c);
                        break;
                }
            }
            return formattedString.ToString();
        }

        /// <summary>
        /// Executes a given Python script and captures the standard output.
        /// </summary>
        /// <param name="script">The Python script to execute.</param>
        /// <returns>The standard output captured from the script execution.</returns>
        public static string RunPythonScript(string script)
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

        /// <summary>
        /// Deserializes a JSON string into an object of type T.
        /// </summary>
        /// <typeparam name="T">The type of object to deserialize into.</typeparam>
        /// <param name="json">The JSON string to deserialize.</param>
        /// <returns>The deserialized object of type T, or default if deserialization fails.</returns>
        public static T DeserializeJson<T>(string json)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(json);
            }
            catch (JsonException e)
            {
                Debug.LogError("Failed to deserialize JSON: " + e.Message);
                return default;
            }
        }
    }
}
