using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Campbell.UI
{
    public class ShowHideUI : MonoBehaviour
    {
        [SerializeField] KeyCode _toggleKey = KeyCode.Escape;
        [SerializeField] GameObject _uIContainer = null;
        [SerializeField] private bool _activeOnStart;

        // Start is called before the first frame update
        void Start()
        {
            _uIContainer.SetActive(_activeOnStart);
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(_toggleKey))
            {
                ToggleActiveState();
            }
        }

        public void ToggleActiveState()
        {
            _uIContainer.SetActive(!_uIContainer.activeSelf);
        }
    }
}