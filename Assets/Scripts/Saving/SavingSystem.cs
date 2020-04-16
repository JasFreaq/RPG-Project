using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RPG.Saving
{
    public class SavingSystem : MonoBehaviour
    {
        BinaryFormatter _formatter = new BinaryFormatter();

        public IEnumerator LoadLastSceneRoutine(string saveFile)
        {
            Dictionary<string, object> state = LoadFile(saveFile);
            int index = SceneManager.GetActiveScene().buildIndex;

            if (state.ContainsKey("lastScene"))
            {
                index = (int)state["lastScene"];
            }

            yield return SceneManager.LoadSceneAsync(index);
            RestoreState(LoadFile(saveFile));
        }

        public void Save(string saveFile)
        {
            Dictionary<string, object> state = LoadFile(saveFile);
            CaptureState(state);
            SaveFile(saveFile, state);
        }
                
        public void Load(string saveFile)
        {
            RestoreState(LoadFile(saveFile));
        }

        public void Delete(string defaultSaveFile)
        {
            File.Delete(GetSaveFilePath(defaultSaveFile));
        }

        private void SaveFile(string saveFile, object state)
        {
            string path = GetSaveFilePath(saveFile);

            using (FileStream stream = File.Open(path, FileMode.Create))
            {
                _formatter.Serialize(stream, state);
            }
        }

        private Dictionary<string, object> LoadFile(string saveFile)
        {
            string path = GetSaveFilePath(saveFile);

            if (!File.Exists(path))
            {
                return new Dictionary<string, object>();
            }

            using (FileStream stream = File.Open(path, FileMode.Open))
            {
                return (Dictionary<string, object>)_formatter.Deserialize(stream);
            }
        }

        private void CaptureState(Dictionary<string, object> state)
        {
            foreach (SaveableEntity saveableEntity in FindObjectsOfType<SaveableEntity>())
            {
                state[saveableEntity.GetIdentifier()] = saveableEntity.CaptureState();
            }

            SaveScene(state);
        }
        
        private void RestoreState(Dictionary<string, object> state)
        {
            foreach(SaveableEntity saveableEntity in FindObjectsOfType<SaveableEntity>())
            {
                string iD = saveableEntity.GetIdentifier();

                if (state.ContainsKey(iD))
                    saveableEntity.RestoreState(state[iD]);
            }
        }

        private static void SaveScene(Dictionary<string, object> state)
        {
            state["lastScene"] = SceneManager.GetActiveScene().buildIndex;
        }

        private string GetSaveFilePath(string saveFile)
        {
            return Path.Combine(Application.persistentDataPath, saveFile + ".sav");
        }
    }
}