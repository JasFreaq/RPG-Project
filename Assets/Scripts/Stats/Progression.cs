using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Stats
{
    [CreateAssetMenu(fileName = "Progression", menuName = "Progression", order = 1)]
    public class Progression : ScriptableObject
    {
        [System.Serializable]
        class ProgressionCharacter
        {
            [SerializeField] CharacterClass _class;
            [SerializeField] float[] _health;

            public CharacterClass GetClass()
            {
                return _class;
            }

            public float GetHealth(int level)
            {
                return _health[level];
            }
        }

        [SerializeField] ProgressionCharacter[] _progressionCharacters = null;

        public float GetHealth(CharacterClass characterClass, int level)
        {
            foreach(ProgressionCharacter progressionCharacter in _progressionCharacters)
            {
                if (characterClass == progressionCharacter.GetClass()) 
                {
                    return progressionCharacter.GetHealth(level);
                }
            }

            Debug.LogError("Health not found.");
            return 0;
        }
    }
}