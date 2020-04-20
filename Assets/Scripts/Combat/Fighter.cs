using UnityEngine;
using RPG.Movement;
using RPG.Core;
using RPG.Attributes;
using RPG.Saving;
using RPG.Stats;
using GameDevTV.Utils;

namespace RPG.Combat
{
    public class Fighter : MonoBehaviour, IAction, ISaveable
    {
        Health _target;
        Mover _mover;

        Animator _animator;
        ActionScheduler _scheduler;

        BaseStats _baseStats;
        LazyValue<float> _damage;
                
        WeaponConfig.WeaponProperties _weaponProperties;
        [Header("Weapon System")]

        [Tooltip("Place left-hand transform at index 0, and right-hand transform at index 1.")]
        [SerializeField] Transform[] _handTransforms = new Transform[2];
        [SerializeField] WeaponConfig _defaultWeaponConfig;

        LazyValue<WeaponConfig> _currentWeaponConfig;
        Weapon _currentWeapon;

        float _timeSinceLastAttack = Mathf.Infinity;

        private void Awake()
        {
            _mover = GetComponent<Mover>();
            _animator = GetComponent<Animator>();
            _scheduler = GetComponent<ActionScheduler>();
            _baseStats = GetComponent<BaseStats>();

            _damage = new LazyValue<float>(GetInitialDamage);
            _currentWeaponConfig = new LazyValue<WeaponConfig>(SetDefaultWeapon);
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
            if (_currentWeaponConfig == null) 
                EquipWeapon(_defaultWeaponConfig);

            _damage.ForceInit();
            _currentWeaponConfig.ForceInit();
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

            if (_currentWeaponConfig != null)
            {
                if (_weaponProperties.weaponsDamageModifier < 1)
                    _weaponProperties.weaponsDamageModifier = 1;
            }
        }

        private void OnDisable()
        {
            if (_baseStats)
            {
                _baseStats.OnLevelUp -= LevelUpUpdate;
            }
        }

        //Weapons System
        private WeaponConfig SetDefaultWeapon()
        {
            AttachWeapon(_defaultWeaponConfig);
            return (_defaultWeaponConfig);
        }

        public void EquipWeapon(WeaponConfig weapon)
        {
            if (_currentWeaponConfig.value)
                _currentWeaponConfig.value.DestroyWeapon();

            _currentWeaponConfig.value = weapon;
            AttachWeapon(weapon);
        }
        public void EquipWeapon(WeaponConfig weapon, Animator animator)
        {
            if (_currentWeaponConfig.value)
                _currentWeaponConfig.value.DestroyWeapon();

            _currentWeaponConfig.value = weapon;
            AttachWeapon(weapon, animator);
        }

        private void AttachWeapon(WeaponConfig weapon)
        {
            _weaponProperties = weapon.Spawn(_handTransforms, _animator, out _currentWeapon);
        }

        private void AttachWeapon(WeaponConfig weapon, Animator animator)
        {
            _weaponProperties = weapon.Spawn(_handTransforms, animator, out _currentWeapon);
        }
        
        //Attacking
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

        //Disablers
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

        //Getter(s)
        private float GetInitialDamage()
        {
            return _baseStats.GetStat(Stat.Damage);
        }

        private bool GetIsInRange()
        {
            return Vector3.Distance(transform.position, _target.transform.position) - _weaponProperties.weaponsRange <= Mathf.Epsilon;
        }

        public float GetWeaponsRange()
        {
            return _weaponProperties.weaponsRange;
        }

        public Health GetTarget()
        {
            return _target;
        }

        //Animation Event(s)
        void Hit()
        {
            if (_target)
            {
                if (gameObject.tag == "Player")
                    _target.SetDamage((_damage.value + _weaponProperties.weaponsDamage) * _weaponProperties.weaponsDamageModifier, gameObject);
                else
                    _target.SetDamage(_damage.value, gameObject);

                if (_currentWeapon)
                    _currentWeapon.OnHit();
            }
        }

        void Shoot()
        {
            if (_target)
            {
                if (gameObject.tag == "Player")
                    _currentWeaponConfig.value.SpawnProjectile(_target, 
                        (_damage.value + _weaponProperties.weaponsDamage) * _weaponProperties.weaponsDamageModifier, gameObject);
                else
                    _currentWeaponConfig.value.SpawnProjectile(_target, _damage.value, gameObject);
            }
        }
        //Levelling Up
        private void LevelUpUpdate()
        {
            _damage.value = _baseStats.GetStat(Stat.Damage);
        }

        //Save System
        public object CaptureState()
        {
            return _currentWeaponConfig.value.name;
        }

        public void RestoreState(object state)
        {
            string weaponName = (string)state;
            WeaponConfig weapon = Resources.Load<WeaponConfig>(weaponName);
            EquipWeapon(weapon, GetComponent<Animator>());
        }
    }
}