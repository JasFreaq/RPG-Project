using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.SceneManagement
{
    public class Fader : MonoBehaviour
    {
        CanvasGroup _canvasGroup;

        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        public void FadeOutImmediate()
        {
            _canvasGroup.alpha = 1;

            _canvasGroup.interactable = true;
            _canvasGroup.blocksRaycasts = true;
        }

        public IEnumerator FadeOutRoutine(float time)
        {
            _canvasGroup.interactable = true;
            _canvasGroup.blocksRaycasts = true;

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

            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
        }
    }
}