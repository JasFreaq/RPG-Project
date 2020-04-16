using System.Collections;
using System.Collections.Generic;
using RPG.Saving;
using UnityEngine;

namespace RPG.SceneManagement
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

            StartCoroutine(LoadLastSceneRoutine());
        }

        private IEnumerator LoadLastSceneRoutine()
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
            else if (Input.GetKeyDown(KeyCode.Delete))
                Delete();
        }

        public void Save()
        {
            _savingSystem.Save(_DefaultSaveFile);
        }

        public void Load()
        {
            _savingSystem.Load(_DefaultSaveFile);
        }

        public void Delete()
        {
            _savingSystem.Delete(_DefaultSaveFile);
        }
    }
}