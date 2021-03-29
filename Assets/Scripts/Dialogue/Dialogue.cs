using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Dialogue
{
    [CreateAssetMenu(fileName = "New Dialogue", menuName = "RPG/Dialogue")]
    public class Dialogue : ScriptableObject
    {
        [SerializeField] private List<DialogueNode> _dialogueNodes = new List<DialogueNode>();

        public IEnumerable<DialogueNode> DialogueNodes
        {
            get { return _dialogueNodes; }
        }

#if UNITY_EDITOR
        private void Awake()
        {
            if (_dialogueNodes.Count == 0)
                _dialogueNodes.Add(new DialogueNode());
        }
#endif
    }
}
