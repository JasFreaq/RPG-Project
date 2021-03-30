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

        private Dictionary<string, DialogueNode> _nodeLookup = new Dictionary<string, DialogueNode>();

        private void Awake()
        {
#if UNITY_EDITOR
            if (_dialogueNodes.Count == 0)
            {
                DialogueNode rootNode = new DialogueNode();
                rootNode.NodeID = Guid.NewGuid().ToString();
                _dialogueNodes.Add(rootNode);
            }
#endif
            OnValidate();
        }

        private void OnValidate()
        {
            _nodeLookup.Clear();
            foreach (DialogueNode node in _dialogueNodes)
            {
                _nodeLookup[node.NodeID] = node;
            }
        }

        public IEnumerable<DialogueNode> DialogueNodes
        {
            get { return _dialogueNodes; }
        }

        public void CreateNode(DialogueNode parentNode)
        {
            DialogueNode node = new DialogueNode();
            node.NodeID = Guid.NewGuid().ToString();
            node.positionRect.center = new Vector2(parentNode.positionRect.center.x + 1.5f * parentNode.positionRect.width,
                parentNode.positionRect.center.y);
            parentNode.ChildrenIDs.Add(node.NodeID);
            _dialogueNodes.Add(node);

            OnValidate();
        }

        public IEnumerable<DialogueNode> GetChildren(DialogueNode node)
        {
            List<DialogueNode> children = new List<DialogueNode>();
            foreach (string iD in node.ChildrenIDs)
            {
                if (_nodeLookup.ContainsKey(iD)) 
                    children.Add(_nodeLookup[iD]);
            }

            return children;
        }
    }
}
