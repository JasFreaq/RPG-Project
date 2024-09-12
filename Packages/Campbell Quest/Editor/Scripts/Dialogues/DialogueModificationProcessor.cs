using System.IO;
using Campbell.Dialogues;
using UnityEditor;

namespace Campbell.Editor.Dialogues
{
    /// <summary>
    /// A class that handles the modification of dialogue assets when they are moved within the Unity Editor.
    /// </summary>
    public class DialogueModificationProcessor : UnityEditor.AssetModificationProcessor
    {
        /// <summary>
        /// Called by Unity when an asset is about to be moved.
        /// </summary>
        /// <param name="sourcePath">The original path of the asset being moved.</param>
        /// <param name="destinationPath">The new path to which the asset is being moved.</param>
        /// <returns>An AssetMoveResult indicating the outcome of the asset move operation.</returns>
        private static AssetMoveResult OnWillMoveAsset(string sourcePath, string destinationPath)
        {
            // Load the dialogue asset from the source path.
            Dialogue dialogue = AssetDatabase.LoadAssetAtPath<Dialogue>(sourcePath);

            // If the dialogue asset exists and is being moved within the same directory,
            // update the dialogue's name to match the new filename.
            if (dialogue)
            {
                if (Path.GetDirectoryName(sourcePath) == Path.GetDirectoryName(destinationPath))
                {
                    dialogue.name = Path.GetFileNameWithoutExtension(destinationPath);
                }
            }

            // Indicate that the asset was not moved by this processor.
            return AssetMoveResult.DidNotMove;
        }
    }
}