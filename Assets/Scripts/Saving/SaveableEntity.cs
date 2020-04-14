using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace RPG.Saving
{
    [ExecuteAlways]
    public class SaveableEntity : MonoBehaviour
    {
        [SerializeField] string _uniqueIdentifier = "";
        static Dictionary<string, SaveableEntity> _globalLookup = new Dictionary<string, SaveableEntity>();

#if UNITY_EDITOR
        private void Update()
        {
            if (!string.IsNullOrEmpty(gameObject.scene.path) && !Application.IsPlaying(gameObject))
            {
                SerializedObject serializedObject = new SerializedObject(this);
                SerializedProperty serializedProperty = serializedObject.FindProperty("_uniqueIdentifier");
                
                if (string.IsNullOrEmpty(serializedProperty.stringValue) || !IsUnique(serializedProperty.stringValue))
                {
                    serializedProperty.stringValue = System.Guid.NewGuid().ToString();
                    serializedObject.ApplyModifiedProperties();
                }

                _globalLookup[serializedProperty.stringValue] = this;
            }
        }

        private bool IsUnique(string stringValue)
        {
            if (_globalLookup.ContainsKey(stringValue))
            {
                if (_globalLookup[stringValue] != this)
                {
                    if (_globalLookup[stringValue])
                    {
                        if (_globalLookup[stringValue].GetIdentifier() == stringValue)
                        {
                            return false;
                        }
                        else
                            _globalLookup.Remove(stringValue);
                    }
                    else
                        _globalLookup.Remove(stringValue);
                }
            }
            return true;
        }
#endif

        public object CaptureState()
        {
            Dictionary<string, object> stateDict = new Dictionary<string, object>();
            foreach(ISaveable saveable in GetComponents<ISaveable>())
            {
                stateDict[saveable.GetType().ToString()] = saveable.CaptureState();
            }
            return stateDict;
        }

        public void RestoreState(object state)
        {
            Dictionary<string, object> stateDict = (Dictionary<string, object>)state;
            foreach (ISaveable saveable in GetComponents<ISaveable>())
            {
                string iD = saveable.GetType().ToString();

                if (stateDict.ContainsKey(iD))
                    saveable.RestoreState(stateDict[iD]);
            }
        }

        public string GetIdentifier()
        {
            return _uniqueIdentifier;
        }
    }
}