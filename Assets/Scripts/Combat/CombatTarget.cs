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
            return true;
        }

        private void Kill()
        {
            Destroy(_collider);
        }

        public Transform GetTransform()
        {
            return transform;
        }
    }
}