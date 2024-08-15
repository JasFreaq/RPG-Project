using System.Collections.Generic;
using Campbell.Control;
using Campbell.Core;
using UnityEngine;

namespace Campbell.Dialogues
{
    public class AIConversationHandler : MonoBehaviour, IRaycastable
    {
        [SerializeField] private string _conversantName;
        [SerializeField] private Dialogue _dialogue;

        private DialogueTrigger _trigger;

        private void Awake()
        {
            _trigger = GetComponent<DialogueTrigger>();
        }

        public string ConversantName
        {
            get { return _conversantName; }
            set { _conversantName = value; }
        }

        public Dialogue Dialogue
        {
            get { return _dialogue; }
            set { _dialogue = value; }
        }

        public void TriggerDialogueAction(IReadOnlyList<DialogueActionData> actions)
        {
            if (_trigger)
            {
                _trigger.Trigger(actions);
            }
        }

        public bool IsRaycastHit(out CursorType cursorType, out RaycastableType raycastableType)
        {
            cursorType = CursorType.Dialogue;
            raycastableType = RaycastableType.Dialogue;
            return true;
        }

        public Transform GetTransform()
        {
            return transform;
        }
    }
}