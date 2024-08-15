using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Campbell.Dialogues
{
    [RequireComponent(typeof(AIConversationHandler))]
    public class DialogueTrigger : MonoBehaviour
    {
        [System.Serializable]
        public class TriggerElement
        {
            public DialogueAction dialogueAction;
            public UnityEvent onTrigger;
        }

        [SerializeField] private List<TriggerElement> _triggers = new();
        
        public void AddTrigger(DialogueAction action, UnityEvent onTrigger)
        {
            TriggerElement element = new TriggerElement
            {
                dialogueAction = action,
                onTrigger = onTrigger
            };

            _triggers.Add(element);
        }
        
        public void Trigger(IReadOnlyList<DialogueActionData> actions)
        {
            foreach (TriggerElement element in _triggers)
            {
                foreach (DialogueActionData actionData in actions)
                {
                    if (element.dialogueAction == actionData.action)
                    {
                        element.onTrigger?.Invoke();
                    }
                }
            }
        }
    }
}