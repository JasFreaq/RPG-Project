using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

namespace RPG.Core.SceneManagement
{
    public class Portal : MonoBehaviour
    {
        [Header("Connections")]
        [SerializeField] int _sceneToLoad = -1;
        [SerializeField] Transform _spawnPoint;
        [SerializeField] PortalIdentifier _identifier;

        [Header("Transition")]
        [SerializeField] float _fadeOutTime=2f;
        [SerializeField] float _fadeInTime = 1f, _fadeWaitTime = 0.5f;
        Fader _fader;

        NavMeshAgent _playerAgent;

        private void Start()
        {
            _fader = FindObjectOfType<Fader>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player")
            {
                DontDestroyOnLoad(gameObject);
                StartCoroutine(TransitionRoutine());
            }
        }

        IEnumerator TransitionRoutine()
        {
            yield return _fader.FadeOutRoutine(_fadeOutTime);
            
            yield return SceneManager.LoadSceneAsync(_sceneToLoad);

            Portal[] portals = FindObjectsOfType<Portal>();
            foreach(Portal portal in portals)
            {
                if (portal == this) continue;

                if (portal.GetIdentifier() == _identifier)
                {
                    HandlePlayer(portal);

                    yield return null;

                    _playerAgent.enabled = false;
                }
            }

            yield return new WaitForSeconds(_fadeWaitTime);
            yield return _fader.FadeInRoutine(_fadeInTime);

            _playerAgent.enabled = true;

            Destroy(gameObject);
        }

        private void HandlePlayer(Portal portal)
        {
            Transform player = GameObject.FindGameObjectWithTag("Player").transform;
            Transform spawnPoint = portal.GetSpawnPoint();

            _playerAgent = player.GetComponent<NavMeshAgent>();

            _playerAgent.Warp(spawnPoint.position);
            player.rotation = spawnPoint.rotation;
        }

        public Transform GetSpawnPoint()
        {
            return _spawnPoint;
        }

        public PortalIdentifier GetIdentifier()
        {
            return _identifier;
        }
    }
}