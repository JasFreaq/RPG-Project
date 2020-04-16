﻿using UnityEngine;
using RPG.Movement;
using RPG.Combat;
using RPG.Resources;
using GameDevTV.Utils;

namespace RPG.Control
{
    public class AIController : MonoBehaviour
    {
        Mover _mover;
        Fighter _fighter;
        Health _player;

        //Basic
        [SerializeField] float _chaseRange = 5f;
        LazyValue<Vector3> _guardLocation;
        float _weaponsRange;
        
        [Header("Patrolling")]
        [SerializeField] PatrolPath _path = null;
        [SerializeField] float _pathTolerance = 1f;
        int _pathIndex = 0;
        [SerializeField] float _chaseSpeed, _patrolSpeed;
        

        [Header("Timers")]
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

            if (_player.IsAlive() && Vector3.Distance(transform.position, _player.transform.position) - _chaseRange <= Mathf.Epsilon)
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
                PatrolBehaviour();
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
            
            _mover.MoveTo(pos);
        }

        private bool AtWaypoint()
        {
            float dist = Vector3.Distance(transform.position, _path.GetWaypoint(_pathIndex));

            return dist - _pathTolerance <= Mathf.Epsilon;
        }
                
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, _chaseRange);
        }

        private void Kill()
        {
            this.enabled = false;
        }
    }
}