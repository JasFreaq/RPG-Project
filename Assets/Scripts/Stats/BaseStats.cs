using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Saving;
using UnityEngine;

namespace RPG.Stats
{
    public class BaseStats : MonoBehaviour, ISaveable
    {
        [SerializeField] Progression _progression = null;
        [SerializeField] CharacterClass _class;

        [SerializeField] [Range(1, 100)] int _startingLevel = 1;
        [Range(1, 100)] int _level = 0;
        float _levelingXP = -1;

        public event Action OnLevelUp;
        [SerializeField] GameObject _levelUpEffectPrefab = null;
        [SerializeField] bool _shouldUseModifiers = false;

        private void Awake()
        {
            if (_level == 0) 
                _level = _startingLevel;
        }

        //Getter(s)
        public float GetStat(Stat stat)
        {
            return (_progression.GetStats(_class, stat, _level) + GetAdditive(stat)) * GetMultiplicative(stat);
        }

        private float GetAdditive(Stat stat)
        {
            float total = 0;

            if (_shouldUseModifiers)
            {
                foreach (IModifier modifier in GetComponents<IModifier>())
                {
                    foreach (float modifierValue in modifier.GetAdditive(stat))
                    {
                        total += modifierValue;
                    }
                }
            }

            return total;
        }

        private float GetMultiplicative(Stat stat)
        {
            float total = 0;

            if (_shouldUseModifiers)
            {
                foreach (IModifier modifier in GetComponents<IModifier>())
                {
                    foreach (float modifierValue in modifier.GetMultiplicative(stat))
                    {
                        total += modifierValue;
                    }
                }
            }

            return 1 + total / 100;
        }

        public int GetLevel()
        {
            return _level;
        }

        //Levelling Up
        public IEnumerator SetLevelRoutine(float currentXP)
        {
            while ((_levelingXP = _progression.GetStats(_class, Stat.ExperienceToLevelUp, _level)) != 0 && currentXP >= _levelingXP)
            {
                _level++;

                if (OnLevelUp != null)
                {
                    OnLevelUp();
                    LevelUpEffect();
                }

                yield return null;
            }
        }

        private void LevelUpEffect()
        {
            GameObject effect = Instantiate(_levelUpEffectPrefab, transform);
            Destroy(effect, 5);
        }

        //Save System
        public object CaptureState()
        {
            return _level;
        }

        public void RestoreState(object state)
        {
            _level = (int)state;
        }
    }
}