using System.Collections;
using System.Collections.Generic;
using System.Reflection;
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

            [HideInInspector] public string parameter;
        }

        [SerializeField] private List<TriggerElement> _triggers = new();
        
        public void AddTrigger(DialogueAction action, UnityEvent onTrigger, string parameter = null)
        {
            TriggerElement element = new TriggerElement
            {
                dialogueAction = action,
                onTrigger = onTrigger,
                parameter = parameter
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
                        if (string.IsNullOrWhiteSpace(element.parameter) || element.parameter == actionData.parameter)
                        {
                            element.onTrigger?.Invoke();
                        }
                    }
                }
            }
        }
    }
}