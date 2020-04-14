using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Stats
{
    public class BaseStats : MonoBehaviour
    {
        [SerializeField] Progression _progression = null;

        [SerializeField] [Range(1, 100)] int _startingLevel = 1;

        [SerializeField] CharacterClass _class;

        public float GetHealth()
        {
            return _progression.GetHealth(_class, _startingLevel - 1);
        }
    }
}