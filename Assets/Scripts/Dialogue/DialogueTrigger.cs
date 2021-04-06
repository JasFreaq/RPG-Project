using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace RPG.Dialogue
{ 
    [RequireComponent(typeof(AIConversationHandler))]
    public class DialogueTrigger : MonoBehaviour
    {
        [System.Serializable]
        struct TriggerElement
        {
            public List<DialogueAction> dialogueActions;
            public UnityEvent onTrigger;

            public bool IsEqual(IReadOnlyList<DialogueAction> actions)
            {
                if (dialogueActions.Count != actions.Count)
                    return false;

                for (int i = 0, n = dialogueActions.Count; i < n; i++)
                {
                    if (dialogueActions[i] != actions[i])
                        return false;
                }

                return true;
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
