using UnityEngine;
using UnityEngine.AI;
using RPG.Core;
using RPG.Saving;

namespace RPG.Movement
{
    public class Mover : MonoBehaviour, IAction, ISaveable
    {
        NavMeshAgent _navMeshAgent;
        Animator _animator;
        ActionScheduler _scheduler;

        [System.Serializable]
        struct Orientation
        {
            public SerializableVector3 position;
            public SerializableVector3 rotation;
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

        private void UpdateAnimator()
        {
            Vector3 velocity = _navMeshAgent.velocity;
            Vector3 localVelocity = transform.InverseTransformDirection(velocity);

            float speed = localVelocity.z;
            _animator.SetFloat("forwardSpeed", speed);
        }

        //Setter(s)
        public void SetSpeed(float speed)
        {
            _navMeshAgent.speed = speed;
        }

        //Disabler(s)
        public void Cancel()
        {
            _navMeshAgent.isStopped = true;
        }

        private void Kill()
        {
            GetComponent<NavMeshAgent>().isStopped = true;
            this.enabled = false;
        }

        //Save System
        public object CaptureState()
        {
            Orientation orientation = new Orientation();

            orientation.position = new SerializableVector3(transform.position);
            orientation.rotation = new SerializableVector3(transform.eulerAngles);

            return orientation;
        }

        public void RestoreState(object state)
        {
            Orientation orientation = (Orientation)state;

            GetComponent<NavMeshAgent>().enabled = false;
            
            transform.position = orientation.position.ToVector3();
            transform.eulerAngles = orientation.rotation.ToVector3();

            GetComponent<NavMeshAgent>().enabled = true;
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }
    }
}