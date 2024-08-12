using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Campbell.Core
{
    public class PersistentObjectSpawner : MonoBehaviour
    {
        [SerializeField] GameObject _persistentPrefab;

        static bool _hasSpawned = false;

        private void Awake()
        {
            if(!_hasSpawned)
            {
                SpawnPersistentObjects();
                _hasSpawned = true;
            }
        }

        private void SpawnPersistentObjects()
        {
            GameObject persistent = Instantiate(_persistentPrefab, transform.position, Quaternion.identity);
            DontDestroyOnLoad(persistent);
        }
    }
}