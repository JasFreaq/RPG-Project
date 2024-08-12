using Campbell.Core;
using Campbell.InventorySystem;
using UnityEngine;

namespace Campbell.Control
{
    [SelectionBase] [RequireComponent(typeof(Pickup))]
    public class ProximityPickup : MonoBehaviour, IRaycastable
    {
        Pickup _pickup;

        private void Awake()
        {
            _pickup = GetComponent<Pickup>();
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
            if (_pickup.CanBePickedUp())
                cursorType = CursorType.Pickup;
            else
                cursorType = CursorType.FullPickup;

            raycastableType = RaycastableType.ProximityPickup;
            return true;
        }

        public Transform GetTransform()
        {
            return transform;
        }
    }
}