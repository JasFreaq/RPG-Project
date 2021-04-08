using System.Collections;
using System.Collections.Generic;
using GameDevTV.Inventories;
using RPG.Stats;
using UnityEngine;

namespace RPG.InventorySystem
{
    [CreateAssetMenu(menuName =("RPG/Inventory/Equipable Item"))]
    public class StatsEquipableItem : EquipableItem, IModifier
    {
        [SerializeField] Modifier[] _additiveBonuses;
        [SerializeField] Modifier[] _multiplicativeBonuses;

        public IEnumerable<float> GetAdditive(Stat stat)
        {
            foreach (Modifier modifier in _additiveBonuses) 
            {
                if (modifier.stat == stat)
                    yield return modifier.value;
            }
        }

        public IEnumerable<float> GetMultiplicative(Stat stat)
        {
            foreach(Modifier modifier in _multiplicativeBonuses)
            {
                if (modifier.stat == stat)
                    yield return modifier.value;
            }
        }
    }
}