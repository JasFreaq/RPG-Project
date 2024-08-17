using Python.Runtime;
using System.IO;
using System.Linq;
using System.Text;
using Unity.Plastic.Newtonsoft.Json;
using UnityEditor.Scripting.Python;
using UnityEngine;

namespace Campbell.Editor.QuestGeneration.Utility
{
    public class UtilityLibrary
    {
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

        public static bool RemoveInvalidFileNameChars(ref string fileName)
        {
            char[] invalidChars = Path.GetInvalidFileNameChars();
            
            bool foundInvalidChars = fileName.Any(c => invalidChars.Contains(c));
            
            if (foundInvalidChars)
            {
                fileName = new string(fileName.Where(c => !invalidChars.Contains(c)).ToArray());
            }

            return foundInvalidChars;
        }

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