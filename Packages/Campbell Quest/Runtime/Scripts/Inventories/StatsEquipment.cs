using System.Collections.Generic;
using Campbell.Stats;

namespace Campbell.InventorySystem
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