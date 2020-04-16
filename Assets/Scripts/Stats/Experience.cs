using System.Collections;
using System.Collections.Generic;
using RPG.Saving;
using UnityEngine;

namespace RPG.Stats
{
    public class Experience : MonoBehaviour, ISaveable
    {
        float _xPPoints = 0;
        BaseStats _baseStats;

        private void Awake()
        {
            _baseStats = GetComponent<BaseStats>();
        }

        public void GainXP(float xP)
        {
            _xPPoints += xP;
            StartCoroutine(_baseStats.SetLevelRoutine(_xPPoints));
        }

        public float GetXP()
        {
            return _xPPoints;
        }

        public object CaptureState()
        {
            return _xPPoints;
        }

        public void RestoreState(object state)
        {
            _xPPoints = (float)state;
            StartCoroutine(GetComponent<BaseStats>().SetLevelRoutine(_xPPoints));
        }
    }
}