using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace RPG.Combat
{
    [SelectionBase]
    public class Health : MonoBehaviour
    {
        [SerializeField] float _healthPoints = 100;
        bool _isAlive = true;

        Animator _animator;

        private void Start()
        {
            _animator = GetComponent<Animator>();
        }

        public void SetDamage(float damage)
        {
            _healthPoints = Mathf.Max(_healthPoints - damage, 0);

            if (_isAlive && _healthPoints <= Mathf.Epsilon)
            {
                _animator.SetTrigger("death");
                _isAlive = false;

                BroadcastMessage("Kill", 0.1f);
            }
        }

        public bool IsAlive()
        {
            return _isAlive;
        }
    }
}