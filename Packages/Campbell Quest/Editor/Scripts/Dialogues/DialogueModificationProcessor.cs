using System.IO;
using Campbell.Dialogues;
using UnityEditor;

namespace Campbell.Editor.Dialogues
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
