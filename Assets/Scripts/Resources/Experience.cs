using System.Collections;
using System.Collections.Generic;
using RPG.Saving;
using UnityEngine;

namespace RPG.Resources
{
    public class Experience : MonoBehaviour, ISaveable
    {
        [SerializeField] float _xPPoints = 0;

        public void GainXP(float xP)
        {
            _xPPoints += xP;
        }

        public object CaptureState()
        {
            return _xPPoints;
        }

        public void RestoreState(object state)
        {
            _xPPoints = (float)state;
        }
    }
}