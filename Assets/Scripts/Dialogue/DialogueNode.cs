using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace RPG.Dialogue
{
    public class DialogueNode : ScriptableObject
    {
        public static float MIN_WIDTH = 200;
        public static float MIN_HEIGHT = 100;

        [SerializeField] private bool _isPlayerSpeech = false;

        [SerializeField] private string _text;

        [SerializeField] private List<string> _childrenIds = new List<string>();

#if UNITY_EDITOR
        [SerializeField] [HideInInspector]
        private Rect _positionRect = new Rect(100, 100, MIN_WIDTH, MIN_HEIGHT);
#endif

        public bool IsPlayerSpeech
        {
            get { return _isPlayerSpeech; }

#if UNITY_EDITOR
            set
            {
                Undo.RecordObject(this, "Dialogue Speaker Edit");
                _isPlayerSpeech = value;
            }
#endif
        }

        public string Text
        {
            get { return _text; }
#if UNITY_EDITOR
            set
            {
                if (_text != value) 
                {
                    Undo.RecordObject(this, "Dialogue Text Edit");
                    _text = value;
                    EditorUtility.SetDirty(this);
                }
            }
#endif
        }

        public IReadOnlyList<string> ChildrenIDs
        {
            get { return _childrenIds; }
        }

#if UNITY_EDITOR
        public Rect PositionRect
        {
            get { return _positionRect; }

            set
            {
                Undo.RecordObject(this, "Dialogue Node Rect Edit");
                _positionRect = value;
                EditorUtility.SetDirty(this);
            }
        }

        public void AddChild(string childNodeID)
        {
            Undo.RecordObject(this, "Dialogue Linked Node");
            _childrenIds.Add(childNodeID);
            EditorUtility.SetDirty(this);
        }
        
        public void RemoveChild(string childNodeID)
        {
            Undo.RecordObject(this, "Dialogue Unlinked Node");
            _childrenIds.Remove(childNodeID);
            EditorUtility.SetDirty(this);
        }
        
        public bool ContainsChild(string childNodeID)
        {
            return _childrenIds.Contains(childNodeID);
        }
#endif
    }
}