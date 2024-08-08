using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Campbell.Editor.Utility
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

        public static string LoadSchema(string schemaName)
        {
            TextAsset schema = Resources.Load<TextAsset>($"Schemas/{schemaName}_schema");

            return FormatStringForPython(schema.text);
        }
        
        public static string LoadTemplate(string templateName)
        {
            TextAsset template = Resources.Load<TextAsset>($"Templates/{templateName}_template");

            return FormatStringForPython(template.text);
        }
    }
}