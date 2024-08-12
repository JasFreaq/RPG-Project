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
        struct TriggerElement
        {
            public DialogueAction dialogueAction;
            public UnityEvent onTrigger;

            public bool IsEqual(IReadOnlyList<DialogueAction> actions)
            {
                for (int i = 0, n = actions.Count; i < n; i++)
                {
                    if (dialogueAction == actions[i])
                        return true;
                }

                return false;
            }
        }

        [SerializeField] private List<TriggerElement> _triggers;

        public void Trigger(IReadOnlyList<DialogueAction> actions)
        {
            foreach (TriggerElement element in _triggers)
            {
                if (element.IsEqual(actions)) 
                {
                    element.onTrigger?.Invoke();
                }
            }
        }
    }
}
