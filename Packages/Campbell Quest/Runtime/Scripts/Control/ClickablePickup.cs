using Campbell.Combat;
using Campbell.Core;
using Campbell.Dialogues;
using Campbell.InventorySystem;
using UnityEngine;
namespace Campbell.Control
{
    [SelectionBase]
    [RequireComponent(typeof(Pickup))]
    public class ClickablePickup : MonoBehaviour, IRaycastable
    {
        Pickup _pickup;
        CombatTarget _combatTarget;

        private void Awake()
        {
            _pickup = GetComponent<Pickup>();
            _combatTarget = GetComponent<CombatTarget>();
        }

        public bool IsRaycastHit(out CursorType cursorType, out RaycastableType raycastableType)
        {
            if (_combatTarget != null &&
                _combatTarget.IsRaycastHit(out cursorType, out raycastableType))
            {
                return true;
            }

            cursorType = _pickup.CanBePickedUp() ? CursorType.Pickup : CursorType.FullPickup;

            raycastableType = RaycastableType.ClickablePickup;

            if (Input.GetMouseButtonDown(0))
                _pickup.PickupItem();

            return true;
        }

        public Transform GetTransform()
        {
            return transform;
        }
    }
}