using Campbell.Attributes;
using Campbell.Combat;
using Campbell.Movement;
using Campbell.Utils;
using UnityEngine;

namespace Campbell.Control
{
    public class AIController : MonoBehaviour
    {
        Mover _mover;
        Fighter _fighter;
        Health _player;

        //Basic
        [SerializeField] float _chaseRange = 5f, _shoutRange = 5f;
        LazyValue<Vector3> _guardLocation;
        float _weaponsRange;
        
        [Header("Patrolling")]
        [SerializeField] PatrolPath _path = null;
        [SerializeField] float _pathTolerance = 1f;
        int _pathIndex = 0;
        [SerializeField] float _chaseSpeed, _patrolSpeed;


        [Header("Timers")]
        [SerializeField] float _aggravationTime = 2f;
        [SerializeField]float _timeSinceAggravated = Mathf.Infinity;
        [SerializeField] float _suspicionTime = 2f;
        float _timeSinceLastSawPlayer = Mathf.Infinity;
        [SerializeField] float _dwellingTime = 2f;
        float _timeSinceLastPatrolled = Mathf.Infinity;

        void Awake()
        {
            _mover = GetComponent<Mover>();
            _fighter = GetComponent<Fighter>();

            _player = GameObject.FindGameObjectWithTag("Player").GetComponent<Health>();

            _guardLocation = new LazyValue<Vector3>(GetInitialLocation);
            _weaponsRange = _fighter.GetWeaponsRange();
        }

        private void Start()
        {
            _guardLocation.ForceInit();
        }

        void Update()
        {
            _timeSinceLastSawPlayer += Time.deltaTime;
            _timeSinceLastPatrolled += Time.deltaTime;
            _timeSinceAggravated += Time.deltaTime;

            if (_player.IsAlive() && _fighter.enabled && ShouldAttack())
            {
                if (Vector3.Distance(transform.position, _player.transform.position) - _weaponsRange >= Mathf.Epsilon)
                {
                    _fighter.Attack(_player.gameObject);
                    _mover.SetSpeed(_chaseSpeed);
                }

                _timeSinceLastSawPlayer = 0;
            }
            else if (_timeSinceLastSawPlayer - _suspicionTime <= Mathf.Epsilon)
            {
                _mover.Cancel();
            }
            else
            {
                PatrolBehaviour();
            }
        }

        private bool ShouldAttack()
        {
            return Vector3.Distance(transform.position, _player.transform.position) - _chaseRange <= Mathf.Epsilon || 
                _timeSinceAggravated - _aggravationTime <= Mathf.Epsilon;
        }

        private Vector3 GetInitialLocation()
        {
            return transform.position;
        }

        private void PatrolBehaviour()
        {
            Vector3 pos = Vector3.zero;
            _mover.SetSpeed(_patrolSpeed);

            if (_path != null)
            {
                if (AtWaypoint())
                {
                    _timeSinceLastPatrolled = 0;
                    _pathIndex = _path.GetJ(_pathIndex);
                }

                if (_timeSinceLastPatrolled - _dwellingTime >= Mathf.Epsilon)
                    pos = _path.GetWaypoint(_pathIndex);
            }
            else
                pos = _guardLocation.value;
            
            _mover.MoveToLocation(pos);
        }

        private bool AtWaypoint()
        {
            float dist = Vector3.Distance(transform.position, _path.GetWaypoint(_pathIndex));

            return dist - _pathTolerance <= Mathf.Epsilon;
        }
                
        public void Aggravate()
        {
            _timeSinceAggravated = 0;
            if (!_fighter.enabled)
                _fighter.enabled = true;
        }

        public void AggravateNearbyEnemies()
        {
            RaycastHit[] hits = Physics.SphereCastAll(transform.position, _shoutRange, Vector3.up);

            foreach(RaycastHit hit in hits)
            {
                AIController enemy = hit.transform.GetComponent<AIController>();

                if (enemy)
                {
                    enemy.Aggravate();
                }
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, _chaseRange);
        }

        private void Kill()
        {
            enabled = false;
        }
    }
}