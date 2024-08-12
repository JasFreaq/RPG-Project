using System.Collections.Generic;
using Campbell.Stats;
using UnityEngine;

namespace Campbell.InventorySystem
{
    [CreateAssetMenu(menuName =("Campbell/Inventory/Stats Equipable Item"))]
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