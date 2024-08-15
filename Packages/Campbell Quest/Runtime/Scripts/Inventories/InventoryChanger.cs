using UnityEngine;

namespace Campbell.InventorySystem
{
    public class InventoryChanger : MonoBehaviour
    {
        [SerializeField] private InventoryItem _item;
        [SerializeField] private bool _remove;
        [SerializeField] private int _number;

#if UNITY_EDITOR
        public InventoryItem Item { set => _item = value; }

        public bool Remove { set => _remove = value; }

        public int Number { set => _number = value; }
#endif

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