using System.Collections;
using System.Collections.Generic;
using GameDevTV.Inventories;
using RPG.Core;
using UnityEngine;
namespace RPG.Control
{
    [SelectionBase]
    [RequireComponent(typeof(Pickup))]
    public class ClickablePickup : MonoBehaviour, IRaycastable
    {
        Pickup _pickup;

        private void Awake()
        {
            _pickup = GetComponent<Pickup>();
        }

        public bool IsRaycastHit(out CursorType cursorType, out RaycastableType raycastableType)
        {
            if (_pickup.CanBePickedUp())
                cursorType = CursorType.Pickup;
            else
                cursorType = CursorType.FullPickup;

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