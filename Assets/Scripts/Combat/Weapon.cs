using System.Collections;
using System.Collections.Generic;
using RPG.Core;
using UnityEngine;

namespace RPG.Combat
{
    [CreateAssetMenu(fileName = "Weapon", menuName = "Weapon", order = 1)]
    public class Weapon : ScriptableObject
    {
        [System.Serializable]
        public struct WeaponProperties
        {
            public float weaponsRange;
            public float timeBetweenAttacks;
            public float weaponsDamage;
        }

        [SerializeField] WeaponProperties _properties;

        [SerializeField] GameObject _weaponPrefab;
        [SerializeField] AnimatorOverrideController _weaponsOverrideController;
        [SerializeField] Projectile _projectile = null;

        [Tooltip("0 is for left-handed, 1 is for right-handed.")]
        [SerializeField] [Range(0, 1)] int _handIndex = -1;

        Transform[] _handTransforms = new Transform[2];
        GameObject _weaponInstance;

        public WeaponProperties Spawn(Transform[] handTransforms, Animator animator)
        {
            if (_weaponPrefab)
            {
                _handTransforms = handTransforms;

                _weaponInstance = Instantiate(_weaponPrefab, _handTransforms[_handIndex].position, _handTransforms[_handIndex].rotation);
                _weaponInstance.transform.SetParent(_handTransforms[_handIndex]);
            }

            if (_weaponsOverrideController) 
            { 
                animator.runtimeAnimatorController = _weaponsOverrideController;
            }

            return _properties;
        }

        public void SpawnProjectile(Health target)
        {
            if(_projectile)
            {
                Projectile projectile = Instantiate(_projectile, _handTransforms[_handIndex].position, _handTransforms[_handIndex].rotation);
                projectile.InitiateTarget(target, _properties.weaponsDamage);
            }
        }

        public void DestroyWeapon()
        {
            Destroy(_weaponInstance);
        }
    }
}