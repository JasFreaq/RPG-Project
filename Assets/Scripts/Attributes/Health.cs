﻿using UnityEngine;
using RPG.Stats;
using RPG.Saving;
using GameDevTV.Utils;
using UnityEngine.Events;

namespace RPG.Attributes
{
    [SelectionBase]
    public class Health : MonoBehaviour, ISaveable
    {
        [System.Serializable]
        struct HealthPointSystem
        {
            public float healthPoints;
            public float totalHealthPoints;
        }

        public LazyValue<float> _healthPoints;
        HealthPointSystem _healthPointSystem;
        bool _isAlive = true;

        Animator _animator;
        BaseStats _baseStats;
        Experience _experience;

        [System.Serializable] public class OnTakeDamage : UnityEvent<float> { }
        [SerializeField] OnTakeDamage _onTakeDamage;

        [SerializeField] UnityEvent _onDeath;


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

            if(_healthPointSystem.totalHealthPoints < _healthPoints.value)
                _healthPointSystem.totalHealthPoints = _healthPoints.value;
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
            _onTakeDamage.Invoke(damage);

            if (_isAlive && _healthPoints.value <= Mathf.Epsilon)
            {
                _onDeath.Invoke();

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

        public float GetTotalHealth()
        {
            return _healthPointSystem.totalHealthPoints;
        }

        public bool IsAlive()
        {
            return _isAlive;
        }

        //Levelling Up
        private void LevelUpUpdate()
        {
            _healthPoints.value = _baseStats.GetStat(Stat.Health);
            _healthPointSystem.totalHealthPoints = _healthPoints.value;
        }

        //Save System
        public object CaptureState()
        {
            _healthPointSystem.healthPoints = _healthPoints.value;
            return _healthPointSystem;
        }

        public void RestoreState(object state)
        {
            HealthPointSystem healthPointSystem = (HealthPointSystem)state;

            _healthPoints.value = healthPointSystem.healthPoints;
            _healthPointSystem.totalHealthPoints = healthPointSystem.totalHealthPoints;

            if (_healthPoints.value <= Mathf.Epsilon)
                Death(GetComponent<Animator>());
        }
    }
}