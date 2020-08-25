using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Core
{
    public class GameHandler : MonoBehaviour
    {
        [SerializeField] GameObject _helpCanvas;

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
                Application.Quit();

            if (Input.GetKeyDown(KeyCode.H))
                _helpCanvas.SetActive(!_helpCanvas.activeInHierarchy);
        }
    }
}