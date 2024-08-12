using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Campbell.UI
{
    public class InGameUI : MonoBehaviour
    {
        void LateUpdate()
        {
            transform.forward = Camera.main.transform.forward;
        }

        public void Destroy()
        {
            Destroy(gameObject);
        }
    }
}