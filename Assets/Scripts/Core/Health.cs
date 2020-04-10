using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Core.Saving;


namespace RPG.Core
{
    [SelectionBase]
    public class Health : MonoBehaviour, ISaveable
    {
        [SerializeField] float _healthPoints = 100;
        bool _isAlive = true;

        Animator _animator;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        public void SetDamage(float damage)
        {
            _healthPoints = Mathf.Max(_healthPoints - damage, 0);

            if (_isAlive && _healthPoints <= Mathf.Epsilon)
            {
                Death();
            }
        }

        private void Death()
        {
            _animator.SetTrigger("death");
            _isAlive = false;

            BroadcastMessage("Kill");
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