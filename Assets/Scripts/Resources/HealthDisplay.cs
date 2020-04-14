using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Resources
{
    public class HealthDisplay : MonoBehaviour
    {
        Health _health;
        [SerializeField] Text _healthDisplay;

        private void Awake()
        {
            _health = GameObject.FindGameObjectWithTag("Player").GetComponent<Health>();
        }

        // Update is called once per frame
        void Update()
        {
            _healthDisplay.text = "Health: " + _health.GetHealth();
        }
    }
}