using System.Collections;
using System.Collections.Generic;
using RPG.Core.Saving;
using UnityEngine;

namespace RPG.Core.SceneManagement
{
    public class SavingWrapper : MonoBehaviour
    {
        const string _DefaultSaveFile = "save";
        SavingSystem _savingSystem;

        Fader _fader;
        [SerializeField] float _loadStartSceneTime = 0.75f;

        private void Awake()
        {
            _savingSystem = GetComponent<SavingSystem>();
            _fader = FindObjectOfType<Fader>();
        }

        private IEnumerator Start()
        {
            _fader.FadeOutImmediate();
            yield return _savingSystem.LoadLastSceneRoutine(_DefaultSaveFile);
            yield return _fader.FadeInRoutine(_loadStartSceneTime);
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.S))
                Save();
            else if (Input.GetKeyDown(KeyCode.L))
                Load();
        }

        public void Save()
        {
            _savingSystem.Save(_DefaultSaveFile);
        }

        public void Load()
        {
            _savingSystem.Load(_DefaultSaveFile);
        }
    }
}