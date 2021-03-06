using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Control;
using UnityEngine;
using RPG.Core;

namespace RPG.Dialogue
{
    public class AIConversationHandler : MonoBehaviour, IRaycastable
    {
        [SerializeField] private Dialogue _dialogue;

        private DialogueTrigger _trigger;

        private void Awake()
        {
            _trigger = GetComponent<DialogueTrigger>();
        }

        public Dialogue Dialogue
        {
            get { return _dialogue; }
        }

        public void TriggerDialogueAction(IReadOnlyList<DialogueAction> actions)
        {
            if (_trigger)
                _trigger.Trigger(actions);
        }

        public bool IsRaycastHit(out CursorType cursorType, out RaycastableType raycastableType)
        {
            cursorType = CursorType.Dialogue;
            raycastableType = RaycastableType.Dialogue;
            return enabled;
        }

        public Transform GetTransform()
        {
            return transform;
        }
    }
}