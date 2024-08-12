using Campbell.Attributes;
using UnityEngine;
using UnityEngine.UI;

namespace Campbell.Combat
{
    public class EnemyHealthDisplay : MonoBehaviour
    {
        Health _health;
        Fighter _player;
        [SerializeField] Text _healthDisplay;

        private void Awake()
        {
            _player = GameObject.FindGameObjectWithTag("Player").GetComponent<Fighter>();
        }

        // Update is called once per frame
        void Update()
        {
            _health = _player.GetTarget();

            if (_health)
                _healthDisplay.text = "Enemy: " + _health.GetHealth();
            else
                _healthDisplay.text = "Enemy: N/A";
        }
    }
}