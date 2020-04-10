using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using RPG.Core;
using RPG.Core.Saving;

namespace RPG.Movement
{
    public class Mover : MonoBehaviour, IAction, ISaveable
    {
        NavMeshAgent _navMeshAgent;
        Animator _animator;
        ActionScheduler _scheduler;

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
            MoveTo(destination);
        }

        public void MoveTo(Vector3 destination)
        {
            if (_navMeshAgent.enabled)
            {
                _navMeshAgent.destination = destination;
                _navMeshAgent.isStopped = false;
            }
        }

        public void Cancel()
        {
            _navMeshAgent.isStopped = true;
        }

        private void UpdateAnimator()
        {
            Vector3 velocity = _navMeshAgent.velocity;
            Vector3 localVelocity = transform.InverseTransformDirection(velocity);

            float speed = localVelocity.z;
            _animator.SetFloat("forwardSpeed", speed);
        }

        //Speed
        public void SetSpeed(float speed)
        {
            _navMeshAgent.speed = speed;
        }

        private void Kill()
        {
            Cancel();
            this.enabled = false;
        }

        public object CaptureState()
        {
            return new SerializableVector3(transform.position);
        }

        public void RestoreState(object state)
        {
            SerializableVector3 serializableVector3 = state as SerializableVector3;

            _navMeshAgent.enabled = false;
            
            transform.position = serializableVector3.ToVector3();

            _navMeshAgent.enabled = true;
            _scheduler.CancelCurrentAction();
        }
    }
}