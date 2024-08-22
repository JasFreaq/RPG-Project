using System.Collections.Generic;
using UnityEngine;

namespace Campbell.InventorySystem
{
    [CreateAssetMenu(menuName =("Campbell/Inventory/Drop Library"))]
    public class DropLibrary : ScriptableObject
    {
        [System.Serializable]
        class PotentialDrop
        {
            public InventoryItem item;
            public float relativeChance;
            public int minNoItemsDropped, maxNoItemsDropped;

            public int GetRandomNumber()
            {
                if (item.IsStackable())
                {
                    return Random.Range(minNoItemsDropped, maxNoItemsDropped + 1);
                }

                return 1;
            }
        }

        [System.Serializable]
        struct DropLevel
        {
            public float dropChancePercentage;
            public int minNoOfDrops, maxNoOfDrops;
            public PotentialDrop[] potentialDrops;
        }

        [SerializeField] DropLevel[] _dropLevels;

        public struct Dropped
        {
            public InventoryItem item;
            public int number;
        }

        public IEnumerable<Dropped> GetRandomDrops(int level)
        {
            DropLevel drop = GetDropLevel(level);

            if (ShouldRandomDrop(drop)) 
            {
                for (int i = 0; i < GetRandomNoOfDrops(drop); i++)
                {
                    yield return GetRandomDrop(drop);
                }
            }
        }

        private DropLevel GetDropLevel(int level)
        {
            if (level >= _dropLevels.Length)
                return _dropLevels[_dropLevels.Length - 1];
            
            if (level < 0)
                return _dropLevels[0];

            return _dropLevels[level];
        }

        private bool ShouldRandomDrop(DropLevel drop)
        {
            float randomRoll = Random.Range(0, 100);
            if (randomRoll < drop.dropChancePercentage)
                return true;

            return false;
        }

        private int GetRandomNoOfDrops(DropLevel drop)
        {
            return Random.Range(drop.minNoOfDrops, drop.maxNoOfDrops + 1);
        }

        private Dropped GetRandomDrop(DropLevel drop)
        {
            Dropped dropped = new Dropped();
            PotentialDrop potentialDrop = SelectDrop(drop);

            dropped.item = potentialDrop.item;
            dropped.number = potentialDrop.GetRandomNumber();

            return dropped;
        }

        PotentialDrop SelectDrop(DropLevel drop)
        {
            float randomRoll = Random.Range(0, GetTotalChance(drop));
            float chanceTotal = 0;

            foreach (PotentialDrop potentialDrop in drop.potentialDrops) 
            {
                chanceTotal += potentialDrop.relativeChance;
                if (chanceTotal > randomRoll)
                    return potentialDrop;
            }

            return null;
        }

        private float GetTotalChance(DropLevel drop)
        {
            float total = 0;

            foreach(PotentialDrop potentialDrop in drop.potentialDrops)
            {
                total += potentialDrop.relativeChance;
            }

            return total;

        }
    }
}