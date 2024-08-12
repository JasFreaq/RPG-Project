using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Campbell.InventorySystem.Pickups
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