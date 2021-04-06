using RPG.Control;
using RPG.Core;
using RPG.Attributes;
using UnityEngine;

namespace RPG.Combat
{
    [RequireComponent(typeof(Health))]
    public class CombatTarget : MonoBehaviour, IRaycastable
    {
        Collider _collider;

        private void Awake()
        {
            _collider = GetComponent<Collider>();
        }

        public bool IsRaycastHit(out CursorType cursorType, out RaycastableType raycastableType)
        {
            cursorType = CursorType.Combat;
            raycastableType = RaycastableType.Enemy;
            return enabled;
        }

        private void Kill()
        {
            _collider.enabled = false;
        }

        public Transform GetTransform()
        {
            return transform;
        }
    }
}