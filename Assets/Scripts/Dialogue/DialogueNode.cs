using System.Collections;
using System.Collections.Generic;
using RPG.Core;
using UnityEditor;
using UnityEngine;

namespace RPG.Dialogues
{
    public class DialogueNode : ScriptableObject
    {
        public static float MIN_WIDTH = 300;
        public static float MIN_HEIGHT = 225;

        [SerializeField] private bool _isPlayerSpeech = false;
        
        [SerializeField] private string _text;

        [SerializeField] private List<string> _childrenIds = new List<string>();

        [SerializeField] private List<DialogueAction> _onEnterActions = new List<DialogueAction>();

        [SerializeField] private List<DialogueAction> _onExitActions = new List<DialogueAction>();

        [SerializeField] private Condition _condition;

        #region Editor Specific
#if UNITY_EDITOR

        [SerializeField] [HideInInspector]
        private Rect _positionRect = new Rect(100, 100, MIN_WIDTH, MIN_HEIGHT);

        private bool _editingOnEnterActions = false;

        private bool _editingOnExitActions = false;

        public void SetIsPlayer(bool value)
        {
            Undo.RecordObject(this, "Dialogue Speaker Edit");
            _isPlayerSpeech = value;
        }
        
        public void SetText(string value)
        {
            if (_text != value)
            {
                Undo.RecordObject(this, "Dialogue Text Edit");
                _text = value;
                EditorUtility.SetDirty(this);
            }
        }

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

        public bool EditingOnEnterActions
        {
            get { return _editingOnEnterActions; }
            set { _editingOnEnterActions = value; }
        }

        public bool EditingOnExitActions
        {
            get { return _editingOnExitActions; }
            set { _editingOnExitActions = value; }
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

        public void ModifyOnEnterAction(DialogueAction action, bool add)
        {
            if (add)
            {
                if (!_onEnterActions.Contains(action))
                {
                    Undo.RecordObject(this, "Dialogue Added OnEnter Action");
                    _onEnterActions.Add(action);
                    EditorUtility.SetDirty(this);
                }
            }
            else
            {
                Undo.RecordObject(this, "Dialogue Removed OnEnter Action");
                _onEnterActions.Remove(action);
                EditorUtility.SetDirty(this);
            }
        }

        public bool OnEnterActionsContain(DialogueAction action)
        {
            return _onEnterActions.Contains(action);
        }

        public void ModifyOnExitAction(DialogueAction action, bool add)
        {
            if (add)
            {
                if (!_onExitActions.Contains(action))
                {
                    Undo.RecordObject(this, "Dialogue Added OnExit Action");
                    _onExitActions.Add(action);
                    EditorUtility.SetDirty(this);
                }
            }
            else
            {
                Undo.RecordObject(this, "Dialogue Removed OnExit Action");
                _onExitActions.Remove(action);
                EditorUtility.SetDirty(this);
            }
        }

        public bool OnExitActionsContain(DialogueAction action)
        {
            return _onExitActions.Contains(action);
        }

        public Condition Condition { set => _condition = value; }

#endif
        #endregion

        public bool IsPlayerSpeech
        {
            get { return _isPlayerSpeech; }
        }

        public string Text
        {
            get { return _text; }
        }

        public IReadOnlyList<string> ChildrenIDs
        {
            get { return _childrenIds; }
        }

        public IReadOnlyList<DialogueAction> OnEnterActions
        {
            get { return _onEnterActions; }
        }
        
        public IReadOnlyList<DialogueAction> OnExitActions
        {
            get { return _onExitActions; }
        }

        public bool EvaluateCondition(IEnumerable<IPredicateEvaluable> evaluables)
        {
            return _condition.Evaluate(evaluables);
        }
    }
}