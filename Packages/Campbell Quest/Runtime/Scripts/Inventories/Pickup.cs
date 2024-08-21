using Campbell.InventorySystem.Pickups;
using UnityEngine;

namespace Campbell.InventorySystem
{
    /// <summary>
    /// To be placed at the root of a Pickup prefab. Contains the data about the
    /// pickup such as the type of item and the number.
    /// </summary>
    public class Pickup : MonoBehaviour
    {
        // STATE
        private InventoryItem _item;
        private int _number = 1;

        // CACHED REFERENCE
        private Inventory _inventory;
        private PickupTrigger _trigger;

        // LIFECYCLE METHODS

        private void Awake()
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            _inventory = player.GetComponent<Inventory>();

            _trigger = GetComponent<PickupTrigger>();
        }

        // PUBLIC

        /// <summary>
        /// Set the vital data after creating the prefab.
        /// </summary>
        /// <param name="item">The type of item this prefab represents.</param>
        /// <param name="number">The number of items represented.</param>
        public void Setup(InventoryItem item, int number)
        {
            _item = item;
            if (!item.IsStackable())
            {
                number = 1;
            }
            _number = number;
        }

        public InventoryItem GetItem()
        {
            return _item;
        }

        public int GetNumber()
        {
            return _number;
        }

        public void PickupItem()
        {
            bool foundSlot = _inventory.AddToFirstEmptySlot(_item, _number);
            if (foundSlot)
            {
                if (_trigger != null)
                {
                    _trigger.Trigger();
                }

                Destroy(gameObject);
            }
        }

        public bool CanBePickedUp()
        {
            return _inventory.HasSpaceFor(_item);
        }
    }
}