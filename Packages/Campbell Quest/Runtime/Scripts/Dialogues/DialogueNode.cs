using System.Collections.Generic;
using Campbell.Core;
using UnityEditor;
using UnityEngine;

namespace Campbell.Dialogues
{
    public class DialogueNode : ScriptableObject
    {
        public static float MIN_WIDTH = 300;
        public static float MIN_HEIGHT = 225;

        [SerializeField] private bool _isPlayerSpeech = false;
        
        [SerializeField] private string _text;

        [SerializeField] private List<string> _childrenIds = new List<string>();

        [SerializeField] private List<DialogueActionData> _dialogueActions = new List<DialogueActionData>();

        [SerializeField] private Condition _condition;

        #region Editor Specific
#if UNITY_EDITOR

        [SerializeField] [HideInInspector]
        private Rect _positionRect = new Rect(100, 100, MIN_WIDTH, MIN_HEIGHT);

        private bool _editingDialogueActions = false;

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

        public List<DialogueActionData> DialogueActions
        {
            get { return _dialogueActions; }
        }

        public bool EditingDialogueActions
        {
            get { return _editingDialogueActions; }
            set { _editingDialogueActions = value; }
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

        public bool DialogueActionsContain(DialogueAction action, out string parameter)
        {
            bool contains = false;
            parameter = null;
            foreach (DialogueActionData data in _dialogueActions)
            {
                if (data.action == action)
                {
                    contains = true;
                    parameter = data.parameter;
                    break;
                }
            }

            return contains;
        }

        public void ModifyDialogueAction(DialogueAction action, string parameter, bool add)
        {
            if (add)
            {
                if (!DialogueActionsContain(action, out string _))
                {
                    Undo.RecordObject(this, "Dialogue Added OnEnter Action");
                    AddDialogueAction(action, parameter);
                    EditorUtility.SetDirty(this);
                }
            }
            else
            {
                if (DialogueActionsContain(action, out string _))
                {
                    Undo.RecordObject(this, "Dialogue Removed OnEnter Action");
                    RemoveDialogueAction(action);
                    EditorUtility.SetDirty(this);
                }
            }
        }

        private void AddDialogueAction(DialogueAction action, string parameter)
        {
            DialogueActionData data = new DialogueActionData();
            data.action = action;
            switch (data.action)
            {
                case DialogueAction.CompleteQuest:
                case DialogueAction.EditInventory:
                    data.parameter = parameter;
                    break;
            }

            _dialogueActions.Add(data);
        }

        private void RemoveDialogueAction(DialogueAction action)
        {
            foreach (DialogueActionData data in _dialogueActions)
            {
                if (data.action == action)
                {
                    _dialogueActions.Remove(data);
                    break;
                }
            }
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

        public bool EvaluateCondition(IEnumerable<IPredicateEvaluable> evaluables)
        {
            return _condition.Evaluate(evaluables);
        }
    }
}