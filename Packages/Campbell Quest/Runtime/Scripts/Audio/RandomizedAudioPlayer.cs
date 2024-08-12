using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Campbell.Audio
{
    public class RandomizedAudioPlayer : MonoBehaviour
    {
        protected AudioSource _audioSource;
        [SerializeField] protected AudioClip[] _audioClips;

        protected void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        public void Play()
        {
            _audioSource.clip = _audioClips[Random.Range(0, _audioClips.Length)];
            _audioSource.Play();
        }
    }
}