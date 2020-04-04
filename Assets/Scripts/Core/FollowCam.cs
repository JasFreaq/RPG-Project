using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Core
{
    [ExecuteInEditMode]
    public class FollowCam : MonoBehaviour
    {
        [SerializeField] Transform _target = null;

        void LateUpdate()
        {
            transform.position = _target.position;
        }
    }
}