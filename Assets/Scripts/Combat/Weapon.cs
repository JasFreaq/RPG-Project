using RPG.Resources;
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
            public float weaponsDamageModifier;
        }

        [SerializeField] WeaponProperties _properties;

        [SerializeField] GameObject _weaponPrefab;
        [SerializeField] AnimatorOverrideController _weaponsOverrideController;
        [SerializeField] Projectile _projectile = null;

        [Tooltip("0 is for left-handed, 1 is for right-handed.")]
        [SerializeField] [Range(0, 1)] int _handIndex = -1;

        Transform[] _handTransforms = new Transform[2];

        public WeaponProperties Spawn(Transform[] handTransforms, Animator animator)
        {
            _handTransforms = handTransforms;

            if (_weaponPrefab)
            {
                Instantiate(_weaponPrefab, _handTransforms[_handIndex]);
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

        public void SpawnProjectile(Health target, float damage, GameObject instigator)
        {
            if(_projectile)
            {
                Projectile projectile = Instantiate(_projectile, _handTransforms[_handIndex].position, _handTransforms[_handIndex].rotation);
                projectile.InitiateTarget(target, damage, instigator);
            }
        }

        public void DestroyWeapon()
        {
            //Debug.Log($"{name} is Destroying old weapons");
            for (int i = 0; i < 2; i++)
            {
                //Debug.Log($"{name} checking {_handTransforms[i]}");
                foreach (Transform transform in _handTransforms[i])
                {
                    //Debug.Log($"{ name} -> { transform.name}/{ transform.tag}");
                    if (transform.tag == "Weapon Prefab")
                    {
                        Destroy(transform.gameObject);                     
                    }
                }
            }
        }
    }
}