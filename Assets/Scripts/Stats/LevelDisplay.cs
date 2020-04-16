using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Stats
{
    public class LevelDisplay : MonoBehaviour
    {
        BaseStats _baseStats;
        [SerializeField] Text _levelDisplay;

        private void Awake()
        {
            _baseStats = GameObject.FindGameObjectWithTag("Player").GetComponent<BaseStats>();
        }

        // Update is called once per frame
        void Update()
        {
            _levelDisplay.text = "Level: " + _baseStats.GetLevel();
        }
    }
}