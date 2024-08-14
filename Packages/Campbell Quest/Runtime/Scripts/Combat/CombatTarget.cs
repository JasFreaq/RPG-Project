using Campbell.Attributes;
using Campbell.Control;
using Campbell.Core;
using Campbell.Dialogues;
using UnityEngine;

namespace Campbell.Combat
{
    [RequireComponent(typeof(Health))]
    public class CombatTarget : MonoBehaviour, IRaycastable
    {
        Fighter _fighter;
        AIConversationHandler _conversationHandler;

        private void Awake()
        {
            _fighter = GetComponent<Fighter>();
            _conversationHandler = GetComponent<AIConversationHandler>();
        }

        public bool IsRaycastHit(out CursorType cursorType, out RaycastableType raycastableType)
        {
            if (_fighter != null && _fighter.enabled)
            {
                cursorType = CursorType.Combat;
                raycastableType = RaycastableType.Enemy;
                return true;
            }

            if (_conversationHandler != null &&
                _conversationHandler.IsRaycastHit(out cursorType, out raycastableType))
            {
                return true;
            }

            cursorType = CursorType.Combat;
            raycastableType = RaycastableType.Enemy;
            return true;
        }

        public Transform GetTransform()
        {
            return transform;
        }
    }
}