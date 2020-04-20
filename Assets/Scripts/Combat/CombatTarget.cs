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

        public bool HandleRaycast(PlayerController callingController)
        {
            if (Input.GetMouseButtonDown(0))
            {
                callingController.GetComponent<Fighter>().Attack(gameObject);
            }

            return true;
        }

        public CursorType GetCursorType()
        {
            return CursorType.Combat;
        }

        public bool IsMovementRequired()
        {
            return false;
        }

        private void Kill()
        {
            Destroy(_collider);
        }

        public Vector3 GetTransform()
        {
            return transform.position;
        }
    }
}