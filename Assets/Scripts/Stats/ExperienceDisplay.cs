using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Stats
{
    public class ExperienceDisplay : MonoBehaviour
    {
        Experience _experience;
        [SerializeField] Text _xPDisplay;

        private void Awake()
        {
            _experience = GameObject.FindGameObjectWithTag("Player").GetComponent<Experience>();
        }

        // Update is called once per frame
        void Update()
        {
            _xPDisplay.text = "XP: " + _experience.GetXP();
        }
    }
}