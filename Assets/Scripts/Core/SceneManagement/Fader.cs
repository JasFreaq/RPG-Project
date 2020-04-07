﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Core.SceneManagement
{
    public class Fader : MonoBehaviour
    {
        CanvasGroup _canvasGroup;

        private void Start()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        public IEnumerator FadeOutRoutine(float time)
        {
            while (_canvasGroup.alpha < 1) 
            {
                _canvasGroup.alpha += Time.deltaTime / time;

                yield return null;
            }
        }

        public IEnumerator FadeInRoutine(float time)
        {
            while (_canvasGroup.alpha > 0)
            {
                _canvasGroup.alpha -= Time.deltaTime / time;

                yield return null;
            }
        }
    }
}