using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Campbell.Quests
{
    public class ProximityTrigger : MonoBehaviour
    {
        [SerializeField] private UnityEvent _trigger;
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player")
            {
                _trigger.Invoke();
            }
        }
    }
}