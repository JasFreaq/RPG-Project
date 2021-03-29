using GameDevTV.Inventories;
using RPG.Combat;
using RPG.Core;
using UnityEngine;

namespace RPG.Control
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