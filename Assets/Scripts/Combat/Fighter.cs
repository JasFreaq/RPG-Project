using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Movement;
using RPG.Core;

namespace RPG.Combat
{
    public class Fighter : MonoBehaviour, IAction
    {
        [SerializeField] float _weaponsRange = 2f;
        [SerializeField] float _timeBetweenAttacks = 1f;
        [SerializeField] float _weaponsDamage = 2f;

        Health _target;
        Mover _mover;
        Animator _animator;
        ActionScheduler _scheduler;

        float _timeSinceLastAttack = Mathf.Infinity;

        private void Start()
        {
            _mover = GetComponent<Mover>();
            _animator = GetComponent<Animator>();
            _scheduler = GetComponent<ActionScheduler>();
        }

        private void Update()
        {
            _timeSinceLastAttack += Time.deltaTime;

            if (_target && GetIsInRange())
            {
                _mover.Cancel();

                if (_target.IsAlive())
                {
                    AttackBehaviour();
                }
                else
                {
                    Cancel();
                }
            }
        }
        
        private bool GetIsInRange()
        {
            return Vector3.Distance(transform.position, _target.transform.position) - _weaponsRange <= Mathf.Epsilon;
        }

        private void AttackBehaviour()
        {
            transform.LookAt(_target.transform);

            if (_timeSinceLastAttack - _timeBetweenAttacks >= Mathf.Epsilon)
            {
                _animator.ResetTrigger("stopAttack");
                _animator.SetTrigger("attack");

                _timeSinceLastAttack = 0f;
            }
        }

        public void Attack(GameObject combatTarget)
        {
            _scheduler.StartAction(this);

            _target = combatTarget.GetComponent<Health>();

            if (_target)
            {
                _mover.MoveTo(_target.transform.position);
            }
        }

        public void Cancel()
        {
            _target = null;

            _animator.ResetTrigger("attack");
            _animator.SetTrigger("stopAttack");
        }

        private void Kill()
        {
            Cancel();
            this.enabled = false;
        }

        public float GetWeaponsRange()
        {
            return _weaponsRange;
        }
        //Animation Event(s)
        public void Hit()
        {
            if (_target != null) 
                _target.SetDamage(_weaponsDamage);
        }
    }
}