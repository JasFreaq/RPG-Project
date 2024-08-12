using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Campbell.Audio
{
    public class AudioPlayerCharacter : RandomizedAudioPlayer
    {
        [SerializeField] AudioClip[] _deathAudioClips;

        public void PlayDeath()
        {
            if (_audioSource.isPlaying)
            {
                _audioSource.Stop();
            }

            _audioSource.clip = _deathAudioClips[Random.Range(0, _deathAudioClips.Length)];
            _audioSource.Play();
        }
    }
}