using UnityEngine;
using RPG.Stats;
using RPG.Saving;

namespace RPG.Resources
{
    [SelectionBase]
    public class Health : MonoBehaviour, ISaveable
    {
        float _healthPoints = 100;
        bool _isAlive = true;

        Animator _animator;
        BaseStats _baseStats;
        Experience _experience;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _baseStats = GetComponent<BaseStats>();
            _healthPoints = _baseStats.GetStat(Stat.Health);
        }


        public void SetDamage(float damage, GameObject instigator)
        {
            _healthPoints = Mathf.Max(_healthPoints - damage, 0);

            if (_isAlive && _healthPoints <= Mathf.Epsilon)
            {
                _experience = instigator.GetComponent<Experience>();
                if (_experience) 
                    _experience.GainXP(_baseStats.GetStat(Stat.ExperienceReward));

                Death();
            }
        }

        private void Death()
        {
            _animator.SetTrigger("death");
            _isAlive = false;

            BroadcastMessage("Kill");
        }

        public float GetHealth()
        {
            return _healthPoints;
        }

        public bool IsAlive()
        {
            return _isAlive;
        }

        public object CaptureState()
        {
            return _healthPoints;
        }

        public void RestoreState(object state)
        {
            _healthPoints = (float)state;

            if (_healthPoints <= Mathf.Epsilon)
                Death();
        }
    }
}