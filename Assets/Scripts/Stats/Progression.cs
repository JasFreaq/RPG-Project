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

        public float GetStats(Stat stat, CharacterClass characterClass, int level)
        {
            foreach (ProgressionCharacter progressionCharacter in _progressionCharacters) 
            {
                if (characterClass == progressionCharacter._class) 
                {
                    foreach (ProgressionStats progressionStat in progressionCharacter._stats) 
                    {
                        if (stat == progressionStat._stat)
                        {
                            if (progressionStat._levels.Length >= level)
                                return progressionStat._levels[level - 1];
                        }
                    }
                }
            }

            Debug.LogError(stat + " not found.");
            return 0;
        }
    }
}