using System.Collections;
using System.Collections.Generic;
using GameDevTV.Inventories;
using RPG.Stats;
using UnityEngine;

namespace RPG.InventorySystem
{
    public class StatsEquipment : Equipment, IModifier
    {
        IEnumerable<float> IModifier.GetAdditive(Stat stat)
        {
            foreach(var slot in GetAllPopulatedSlots())
            {
                IModifier item = GetItemInSlot(slot) as IModifier;
                if (item != null) 
                {
                    foreach(float modifier in item.GetAdditive(stat))
                    {
                        yield return modifier;
                    }
                }
            }
        }

        IEnumerable<float> IModifier.GetMultiplicative(Stat stat)
        {
            foreach(EquipLocation slot in GetAllPopulatedSlots())
            {
                IModifier item = GetItemInSlot(slot) as IModifier;
                if (item != null)
                {
                    foreach(float modifier in item.GetMultiplicative(stat))
                    {
                        yield return modifier;
                    }
                }
            }
        }
    }
}