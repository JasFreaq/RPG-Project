using Campbell.Stats;
using UnityEngine;

namespace Campbell.InventorySystem
{
    [RequireComponent(typeof(BaseStats))]
    public class EnemyRandomDropper : RandomDropper
    {
        //Config Data
        [SerializeField] private DropLibrary _dropLibrary;

#if UNITY_EDITOR

        public DropLibrary DropLibrary
        {
            set { _dropLibrary = value; }
        }

#endif

        public void RandomDrop()
        {
            foreach (DropLibrary.Dropped drop in _dropLibrary.GetRandomDrops(GetComponent<BaseStats>().GetLevel() - 1))
            {
                DropItem(drop.item, drop.number);
            }
        }
    }
}