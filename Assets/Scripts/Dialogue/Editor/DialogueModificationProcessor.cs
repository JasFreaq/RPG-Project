using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace RPG.Dialogues.Editor
{
    public class DialogueModificationProcessor : UnityEditor.AssetModificationProcessor
    {
        private static AssetMoveResult OnWillMoveAsset(string sourcePath, string destinationPath)
        {
            Dialogue dialogue = AssetDatabase.LoadAssetAtPath<Dialogue>(sourcePath);
            if (dialogue)
            {
                if (Path.GetDirectoryName(sourcePath) == Path.GetDirectoryName(destinationPath))
                {
                    dialogue.name = Path.GetFileNameWithoutExtension(destinationPath);
                }
            }

            return AssetMoveResult.DidNotMove;
        }
    }
}
