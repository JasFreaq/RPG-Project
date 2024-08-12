using System;
using System.Collections;
using Campbell.Core;
using Campbell.Saving;
using UnityEngine;
using UnityEngine.AI;

namespace Campbell.Movement
{
    public class Mover : MonoBehaviour, IAction, ISaveable
    {
        [SerializeField] private float _objectStoppingDistance = 1.5f;

        NavMeshAgent _navMeshAgent;
        Animator _animator;
        ActionScheduler _scheduler;

        private Action _onDestinationReached;
        private Coroutine _checkDestinationReachedCoroutine = null;

        [System.Serializable]
        struct Orientation
        {
            public SerializableVector3 position;
            public SerializableVector3 rotation;

            public Orientation(Transform transform)
            {
                position = new SerializableVector3(transform.position);
                rotation = new SerializableVector3(transform.rotation.eulerAngles);
            }
        }

        void Awake()
        {
            _navMeshAgent = GetComponent<NavMeshAgent>();
            _animator = GetComponent<Animator>();
            _scheduler = GetComponent<ActionScheduler>();
        }

        void Update()
        {
            UpdateAnimator();
        }

        public void MoveToCursor(Vector3 destination)
        {
            _scheduler.StartAction(this);
            MoveToLocation(destination);
        }

        public void MoveToLocation(Vector3 destination)
        {
            if (_navMeshAgent.enabled)
            {
                _navMeshAgent.stoppingDistance = 0;
                _navMeshAgent.destination = destination;
                _navMeshAgent.isStopped = false;
                
                CheckDestinationReached();
            }
        }
        
        public void MoveToObject(Transform destination)
        {
            if (_navMeshAgent.enabled)
            {
                _navMeshAgent.stoppingDistance = _objectStoppingDistance;
                _navMeshAgent.destination = destination.position;
                _navMeshAgent.isStopped = false;

                CheckDestinationReached();
            }
        }
        
        private void UpdateAnimator()
        {
            Vector3 velocity = _navMeshAgent.velocity;
            Vector3 localVelocity = transform.InverseTransformDirection(velocity);

            float speed = localVelocity.z;
            _animator.SetFloat("forwardSpeed", speed);
        }

        public void RegisterOnDestinationReached(Action action)
        {
            _onDestinationReached += action;
        }
        
        public void DeregisterOnDestinationReached(Action action)
        {
            _onDestinationReached -= action;
        }

        private void CheckDestinationReached()
        {
            if (_checkDestinationReachedCoroutine != null)
            {
                StopCoroutine(_checkDestinationReachedCoroutine);
            }

            _checkDestinationReachedCoroutine = StartCoroutine(CheckDestinationReachedRoutine());
        }

        private IEnumerator CheckDestinationReachedRoutine()
        {
            while (true)
            {
                yield return new WaitForEndOfFrame();

                if (!_navMeshAgent.pathPending)
                {
                    if (_navMeshAgent.remainingDistance <= _navMeshAgent.stoppingDistance)
                    {
                        if (!_navMeshAgent.hasPath || _navMeshAgent.velocity.sqrMagnitude == 0f)
                        {
                            _onDestinationReached?.Invoke();
                            _checkDestinationReachedCoroutine = null;
                            break;
                        }
                    }
                }
            }
        }

        public void SetSpeed(float speed)
        {
            _navMeshAgent.speed = speed;
        }

        public void Cancel()
        {
            _navMeshAgent.isStopped = true;
        }

        private void Kill()
        {
            GetComponent<NavMeshAgent>().isStopped = true;
            enabled = false;
        }

        //Save System
        public object CaptureState()
        {
            return new Orientation(transform);
        }

        public void RestoreState(object state)
        {
            Orientation orientation = (Orientation) state;

            GetComponent<NavMeshAgent>().enabled = false;
            
            transform.position = orientation.position.ToVector3();
            transform.eulerAngles = orientation.rotation.ToVector3();

            GetComponent<NavMeshAgent>().enabled = true;
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }
    }
}