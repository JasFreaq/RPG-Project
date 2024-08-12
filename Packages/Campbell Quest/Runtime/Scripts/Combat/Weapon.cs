using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Campbell.Combat
{
    public class Weapon : MonoBehaviour
    {
        [SerializeField] UnityEvent _onHit;

        public void OnHit()
        {
            _onHit.Invoke();
        }
    }
}