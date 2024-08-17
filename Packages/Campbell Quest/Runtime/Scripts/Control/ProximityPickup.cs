using Campbell.Combat;
using Campbell.Core;
using Campbell.InventorySystem;
using UnityEngine;

namespace Campbell.Control
{
    [SelectionBase] [RequireComponent(typeof(Pickup))]
    public class ProximityPickup : MonoBehaviour, IRaycastable
    {
        Pickup _pickup;
        CombatTarget _combatTarget;

        private void Awake()
        {
            _pickup = GetComponent<Pickup>();
            _combatTarget = GetComponent<CombatTarget>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player")
            {
                _pickup.PickupItem();
            }
        }

        public bool IsRaycastHit(out CursorType cursorType, out RaycastableType raycastableType)
        {
            if (_combatTarget != null &&
                _combatTarget.IsRaycastHit(out cursorType, out raycastableType))
            {
                return true;
            }

            cursorType = _pickup.CanBePickedUp() ? CursorType.Pickup : CursorType.FullPickup;

            raycastableType = RaycastableType.ProximityPickup;
            return true;
        }

        public Transform GetTransform()
        {
            return transform;
        }
    }
}