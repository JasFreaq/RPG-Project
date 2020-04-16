using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Stats
{
    [CreateAssetMenu(fileName = "Progression", menuName = "Progression", order = 1)]
    public class Progression : ScriptableObject
    {
        [System.Serializable]
        class ProgressionStats
        {
            public Stat _stat;
            public float[] _levels;
        }

        [System.Serializable]
        class ProgressionCharacter
        {
            public CharacterClass _class;
            public ProgressionStats[] _stats;
        }

        [SerializeField] ProgressionCharacter[] _progressionCharacters = null;

        Dictionary<CharacterClass, Dictionary<Stat, float[]>> _lookupTable = null;

        public float GetStats(CharacterClass characterClass, Stat stat, int level)
        {
            BuildTable();

            float[] levels = _lookupTable[characterClass][stat];

            if (levels.Length >= level)
                return levels[level - 1];

            if (stat == Stat.ExperienceToLevelUp)
                Debug.LogWarning("Highest level reached.");
            else
                Debug.LogError("Stat value not found");

            return 0;
        }

        private void BuildTable()
        {
            if (_lookupTable == null)
            {
                _lookupTable = new Dictionary<CharacterClass, Dictionary<Stat, float[]>>();

                foreach (ProgressionCharacter progressionCharacter in _progressionCharacters)
                {
                    Dictionary<Stat, float[]> statLookupTable = new Dictionary<Stat, float[]>();

                    foreach (ProgressionStats progressionStat in progressionCharacter._stats)
                    {
                        statLookupTable[progressionStat._stat] = progressionStat._levels;
                    }

                    _lookupTable[progressionCharacter._class] = statLookupTable;
                }
            }
        }
    }
}