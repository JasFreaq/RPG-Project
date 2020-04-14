using UnityEngine;
using RPG.Movement;
using RPG.Core;
using RPG.Resources;
using RPG.Saving;

namespace RPG.Combat
{
    public class Fighter : MonoBehaviour, IAction, ISaveable
    {
        Health _target;
        Mover _mover;
        Animator _animator;
        ActionScheduler _scheduler;
                
        Weapon.WeaponProperties _weaponProperties;
        [Header("Weapon System")]

        [Tooltip("Place left-hand transform at index 0, and right-hand transform at index 1.")]
        [SerializeField] Transform[] _handTransforms = new Transform[2];
        [SerializeField] Weapon _defaultWeapon;

        Weapon _currentWeapon = null;
        float _timeSinceLastAttack = Mathf.Infinity;

        private void Awake()
        {
            _mover = GetComponent<Mover>();
            _animator = GetComponent<Animator>();
            _scheduler = GetComponent<ActionScheduler>();
        }

        private void Start()
        {
            if(_currentWeapon == null)
                EquipWeapon(_defaultWeapon);
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

        public void EquipWeapon(Weapon weapon)
        {
            if (_currentWeapon)
                _currentWeapon.DestroyWeapon();

            _currentWeapon = weapon;
            _weaponProperties = weapon.Spawn(_handTransforms, _animator);
        }

        private bool GetIsInRange()
        {
            return Vector3.Distance(transform.position, _target.transform.position) - _weaponProperties.weaponsRange <= Mathf.Epsilon;
        }

        private void AttackBehaviour()
        {
            transform.LookAt(_target.transform);

            if (_timeSinceLastAttack - _weaponProperties.timeBetweenAttacks >= Mathf.Epsilon)
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
            this.enabled = false;
        }

        public float GetWeaponsRange()
        {
            return _weaponProperties.weaponsRange;
        }
        //Animation Events
        void Hit()
        {
            if (_target) 
                _target.SetDamage(_weaponProperties.weaponsDamage);
        }

        void Shoot()
        {
            if(_target)
                _currentWeapon.SpawnProjectile(_target);
        }

        public object CaptureState()
        {
            return _currentWeapon.name;
        }

        public void RestoreState(object state)
        {
            string weaponName = (string)state;
            Weapon weapon = UnityEngine.Resources.Load<Weapon>(weaponName);
            EquipWeapon(weapon);
        }
    }
}