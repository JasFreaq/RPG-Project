using RPG.Resources;
using UnityEngine;

namespace RPG.Combat
{
    public class Projectile : MonoBehaviour
    {
        Health _target;
        float _damage;

        [SerializeField] float _speed = 3f;
        [SerializeField] bool _isHoming = false;
        [SerializeField] float _lifetime = 10f;
        float _timer = 0;

        [Header("On Impact")]
        [SerializeField] GameObject _hitEffects = null;
        [SerializeField] GameObject[] _destroyImmediate;
        [SerializeField] float _destroyDelay = 0.25f;

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
            if (_target)
                transform.Translate(Vector3.forward * _speed * Time.deltaTime);

            if (!_target.IsAlive() || _timer > _lifetime) 
                Destroy(gameObject);

            if (_isHoming)
                SetAimLocation();

            _timer += Time.deltaTime;
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
            if (other.GetComponent<Health>() == _target) 
            {
                _target.SetDamage(_damage);

                if (_hitEffects)
                {
                    GameObject effects = Instantiate(_hitEffects, transform.position, Quaternion.identity);
                    Destroy(effects, 3f);
                }

                foreach (GameObject gameObject in _destroyImmediate)
                    Destroy(gameObject);
                
                Destroy(gameObject, _destroyDelay);
            }
        }
    }
}