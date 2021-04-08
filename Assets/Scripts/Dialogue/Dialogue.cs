using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace RPG.Dialogue
{
    [CreateAssetMenu(fileName = "New Dialogue", menuName = "RPG/Dialogue")]
    public class Dialogue : ScriptableObject, ISerializationCallbackReceiver
    {
        [SerializeField] private List<DialogueNode> _dialogueNodes = new List<DialogueNode>();

        private Dictionary<string, DialogueNode> _nodeLookup = new Dictionary<string, DialogueNode>();

        #region Editor Specific
#if UNITY_EDITOR

        private Vector2 _editorScrollPosition = Vector2.zero;
        public Vector2 EditorScrollPosition
        {
            get { return _editorScrollPosition; }
            set { _editorScrollPosition = value; }
        }

        public void CreateRootNode()
        {
            if (_dialogueNodes.Count == 0)
            {
                DialogueNode rootNode = CreateInstance<DialogueNode>();
                rootNode.name = Guid.NewGuid().ToString();
                _dialogueNodes.Add(rootNode);
            }
        }

        public void CreateNode(DialogueNode parentNode)
        {
            DialogueNode node = CreateInstance<DialogueNode>();
            node.name = Guid.NewGuid().ToString();
            Undo.RegisterCreatedObjectUndo(node, "Dialogue Object Created");

            node.SetIsPlayer(!parentNode.IsPlayerSpeech);
            Rect tempRect = new Rect(node.PositionRect);
            tempRect.center = new Vector2(parentNode.PositionRect.center.x + 1.5f * parentNode.PositionRect.width,
                parentNode.PositionRect.center.y);
            node.PositionRect = tempRect;
            parentNode.AddChild(node.name);

            Undo.RecordObject(this, "Dialogue Node Added");
            _dialogueNodes.Add(node);
            if (!_nodeLookup.ContainsKey(node.name))
                _nodeLookup[node.name] = node;
        }

        public void DeleteNode(DialogueNode node)
        {
            Undo.RecordObject(this, "Dialogue Node Deleted");
            _dialogueNodes.Remove(node);

            if (_nodeLookup.ContainsKey(node.name))
                _nodeLookup.Remove(node.name);
            foreach (DialogueNode dialogueNode in _dialogueNodes)
            {
                dialogueNode.RemoveChild(node.name);
            }

            Undo.DestroyObjectImmediate(node);

            CreateRootNode();
        }
#endif
        #endregion

        private void Awake()
        {
#if UNITY_EDITOR
            CreateRootNode();
#endif

            OnValidate();
        }

        private void OnValidate()
        {
            _nodeLookup.Clear();
            foreach (DialogueNode node in _dialogueNodes)
            {
                _nodeLookup[node.name] = node;
            }
        }

        public IReadOnlyList<DialogueNode> DialogueNodes
        {
            get { return _dialogueNodes; }
        }
        
        public IReadOnlyList<DialogueNode> GetChildrenOfNode(DialogueNode node)
        {
            List<DialogueNode> children = new List<DialogueNode>();
            foreach (string iD in node.ChildrenIDs)
            {
                if (_nodeLookup.ContainsKey(iD)) 
                    children.Add(_nodeLookup[iD]);
            }

            return children;
        }
        
        public IReadOnlyList<DialogueNode> GetPlayerChildrenOfNode(DialogueNode node)
        {
            List<DialogueNode> children = new List<DialogueNode>();
            foreach (string iD in node.ChildrenIDs)
            {
                if (_nodeLookup.ContainsKey(iD) && _nodeLookup[iD].IsPlayerSpeech) 
                    children.Add(_nodeLookup[iD]);
            }

            return children;
        }
        
        public void OnBeforeSerialize()
        {
#if UNITY_EDITOR
            if (!string.IsNullOrEmpty(AssetDatabase.GetAssetPath(this)))
            {
                foreach (DialogueNode node in _dialogueNodes)
                {
                    if (string.IsNullOrEmpty(AssetDatabase.GetAssetPath(node)))
                    {
                        AssetDatabase.AddObjectToAsset(node, this);
                    }
                }
            }
#endif
        }

        public void OnAfterDeserialize() { }
    }
}
