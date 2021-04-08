using System.Collections;
using System.Collections.Generic;
using GameDevTV.Inventories;
using RPG.Stats;
using UnityEngine;

namespace RPG.InventorySystem
{
    [RequireComponent(typeof(BaseStats))]
    public class EnemyRandomDropper : RandomDropper
    {
        //Config Data
        [SerializeField] DropLibrary _dropLibrary;

        public void RandomDrop()
        {
            foreach (DropLibrary.Dropped drop in _dropLibrary.GetRandomDrops(GetComponent<BaseStats>().GetLevel() - 1))
            {
                DropItem(drop.item, drop.number);
            }
        }
    }
}