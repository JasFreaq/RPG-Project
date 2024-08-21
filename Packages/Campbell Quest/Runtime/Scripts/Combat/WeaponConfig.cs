﻿using System.Collections.Generic;
using Campbell.Attributes;
using Campbell.InventorySystem;
using Campbell.Stats;
using UnityEngine;

namespace Campbell.Combat
{
    [CreateAssetMenu(fileName = "Weapon Config", menuName = "Campbell/Weapon Configuration", order = 1)]
    public class WeaponConfig : EquipableItem, IModifier
    {
        [System.Serializable]
        public struct WeaponProperties
        {
            public float weaponsRange;
            public float timeBetweenAttacks;
            public float weaponsDamage;
            public bool isProjectile;
        }

        [SerializeField] WeaponProperties _properties;

        [SerializeField] Weapon _weaponPrefab;
        [SerializeField] AnimatorOverrideController _weaponsOverrideController;
        [SerializeField] Projectile _projectile = null;

        [Tooltip("0 is for left-handed, 1 is for right-handed.")]
        [SerializeField] [Range(0, 1)] int _handIndex = -1;
        Transform[] _handTransforms = new Transform[2];

        [SerializeField] Modifier[] _additiveBonuses;
        [SerializeField] Modifier[] _multiplicativeBonuses;

#if UNITY_EDITOR

        public Modifier[] AdditiveBonuses { set => _additiveBonuses = value; }

        public Modifier[] MultiplicativeBonuses { set => _multiplicativeBonuses = value; }

#endif

        public WeaponProperties Spawn(Transform[] handTransforms, Animator animator, out Weapon weapon)
        {
            _handTransforms = handTransforms;

            if (_weaponPrefab)
            {
                weapon = Instantiate(_weaponPrefab, handTransforms[_handIndex]);
            }
            else
            {
                weapon = null;
            }

            AnimatorOverrideController overrideController = animator.runtimeAnimatorController as AnimatorOverrideController;
            if (_weaponsOverrideController)
            {
                animator.runtimeAnimatorController = _weaponsOverrideController;
            }
            else if (overrideController) 
            {
                animator.runtimeAnimatorController = overrideController.runtimeAnimatorController;
            }

            return _properties;
        }

        public void SpawnProjectile(Health target, float damage, GameObject instigator, Transform[] handTransforms)
        {
            if(_projectile)
            {
                Projectile projectile = Instantiate(_projectile, handTransforms[_handIndex].position, handTransforms[_handIndex].rotation);
                projectile.InitiateTarget(target, damage, instigator);
            }
        }

        public void DestroyWeapon()
        {
            for (int i = 0; i < 2; i++)
            {
                foreach (Transform transform in _handTransforms[i])
                {
                    if (transform.tag == "Weapon Prefab")
                    {
                        Destroy(transform.gameObject);                     
                    }
                }
            }
        }

        public IEnumerable<float> GetAdditive(Stat stat)
        {
            if (_additiveBonuses.Length > 0)
            {
                foreach (Modifier modifier in _additiveBonuses)
                {
                    if (stat == Stat.Damage)
                    {
                        if (modifier.stat == stat)
                            yield return _properties.weaponsDamage + modifier.value;
                        else
                            yield return _properties.weaponsDamage;
                    }
                    else
                    {
                        if (modifier.stat == stat)
                            yield return modifier.value;
                    }
                }
            }
            else if (stat == Stat.Damage)
                yield return _properties.weaponsDamage;
        }

        public IEnumerable<float> GetMultiplicative(Stat stat)
        {
            foreach (Modifier modifier in _multiplicativeBonuses)
            {
                if (modifier.stat == stat)
                    yield return modifier.value;
            }
        }
    }
}