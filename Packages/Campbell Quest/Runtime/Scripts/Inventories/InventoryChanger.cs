using UnityEngine;

namespace Campbell.InventorySystem
{
    public class InventoryChanger : MonoBehaviour
    {
        [SerializeField] private InventoryItem _item;
        [SerializeField] private bool _remove;
        [SerializeField] private int _number;

        public void EditInventory()
        {
            Inventory playerInventory = Inventory.GetPlayerInventory();
            if (_remove)
            {
                if (playerInventory.HasItem(_item, out int slot))
                {
                    playerInventory.RemoveFromSlot(slot, _number);
                }
            }
            else
            {
                playerInventory.AddToFirstEmptySlot(_item, _number);
            }
        }
    }
}