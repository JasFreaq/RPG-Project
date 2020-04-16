using UnityEngine;
using RPG.Stats;
using RPG.Saving;
using GameDevTV.Utils;

namespace RPG.Resources
{
    [SelectionBase]
    public class Health : MonoBehaviour, ISaveable
    {
        LazyValue<float> _healthPoints;
        bool _isAlive = true;

        Animator _animator;
        BaseStats _baseStats;
        Experience _experience;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _baseStats = GetComponent<BaseStats>();
            _healthPoints = new LazyValue<float>(GetInitialHealth);
        }

        private void OnEnable()
        {
            if (_baseStats)
            {
                _baseStats.OnLevelUp += LevelUpUpdate;
            }
        }

        private void Start()
        {
            _healthPoints.ForceInit();
        }

        private void OnDisable()
        {
            if (_baseStats)
            {
                _baseStats.OnLevelUp -= LevelUpUpdate;
            }
        }

        private float GetInitialHealth()
        {
            if (_baseStats)
                return _baseStats.GetStat(Stat.Health);

            Debug.LogError("BaseStats is missing -> " + name);
            return 0;
        }

        public void SetDamage(float damage, GameObject instigator)
        {
            _healthPoints.value = Mathf.Max(_healthPoints.value - damage, 0);

            if (_isAlive && _healthPoints.value <= Mathf.Epsilon)
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

        private void Death(Animator animator)
        {
            animator.SetTrigger("death");
            _isAlive = false;

            BroadcastMessage("Kill");
        }

        //Getter(s)
        public float GetHealth()
        {
            return _healthPoints.value;
        }

        public bool IsAlive()
        {
            return _isAlive;
        }

        //Levelling Up
        private void LevelUpUpdate()
        {
            _healthPoints.value = _baseStats.GetStat(Stat.Health);
        }

        //Save System
        public object CaptureState()
        {
            return _healthPoints.value;
        }

        public void RestoreState(object state)
        {
            _healthPoints.value = (float)state;

            if (_healthPoints.value <= Mathf.Epsilon)
                Death(GetComponent<Animator>());
        }
    }
}