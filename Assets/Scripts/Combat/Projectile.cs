using System.Collections;
using System.Collections.Generic;
using RPG.Core;
using UnityEngine;

namespace RPG.Combat
{
    public class Projectile : MonoBehaviour
    {
        Health _target;
        [SerializeField] float _speed = 3f;
        float _damage;
        [SerializeField] bool _isHoming = false;

        CapsuleCollider _collider;

        // Start is called before the first frame update
        public void InitiateTarget(Health target, float damage)
        {
            _target = target;
            _damage = damage;

            if(!_isHoming)
                SetAimLocation();
        }

        // Update is called once per frame
        void Update()
        {
            if (_isHoming)
            {
                if (!_target.IsAlive())
                    Destroy(gameObject);

                SetAimLocation(); 
            }

            if (_target)
                transform.Translate(Vector3.forward * _speed * Time.deltaTime);
        }

        private void SetAimLocation()
        {
            if (_target) 
            {
                _collider = _target.GetComponent<CapsuleCollider>();

                if (_collider)
                    transform.LookAt(_target.transform.position + Vector3.up * _collider.height / 2);
                else
                {
                    transform.LookAt(_target.transform.position);
                    Debug.LogWarning("Target is missing CapsuleCollider.");
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if(_target)
            {
                _target.SetDamage(_damage);
            }
            Destroy(gameObject);
        }
    }
}