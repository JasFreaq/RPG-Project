using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using RPG.Control;
using RPG.Core;

namespace RPG.Cinematics
{
    public class CinematicControlRemover : MonoBehaviour
    {
        GameObject _player;

        // Start is called before the first frame update
        void Start()
        {
            PlayableDirector playableDirector = GetComponent<PlayableDirector>();

            playableDirector.played += DisableControl;
            playableDirector.stopped += EnableControl;


        }

        void DisableControl(PlayableDirector director)
        {
            _player = GameObject.FindGameObjectWithTag("Player");

            _player.GetComponent<ActionScheduler>().CancelCurrentAction();
            _player.GetComponent<PlayerController>().enabled = false;
        }

        void EnableControl(PlayableDirector director)
        {
            _player.GetComponent<PlayerController>().enabled = true;
        }
    }
}