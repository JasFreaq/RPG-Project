using System.Collections;
using System.Collections.Generic;
using GameDevTV.Inventories;
using RPG.Dialogue;
using UnityEngine;
using UnityEngine.Events;

namespace RPG.InventorySystem.Pickups
{
    [RequireComponent(typeof(Pickup))]
    public class PickupTrigger : MonoBehaviour
    {
        [SerializeField] private List<UnityEvent> _triggers;

        public void Trigger()
        {
            foreach (UnityEvent unityEvent in _triggers)
            {
                unityEvent?.Invoke();
            }
        }
    }
}