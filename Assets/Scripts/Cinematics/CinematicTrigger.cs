using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace RPG.Cinematics
{
    public class CinematicTrigger : MonoBehaviour
    {
        bool _shouldPlay = true;

        private void OnTriggerEnter(Collider other)
        {
            if(_shouldPlay && other.tag=="Player")
            {
                GetComponent<PlayableDirector>().Play();
                _shouldPlay = false;
            }
        }
    }
}