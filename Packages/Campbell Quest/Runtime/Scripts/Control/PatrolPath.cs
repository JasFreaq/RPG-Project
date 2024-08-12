using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Campbell.Control
{
    public class PatrolPath : MonoBehaviour
    {
        const float _gizmoSize = 0.5f;

        private void OnDrawGizmos()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                int j = GetJ(i);

                Gizmos.DrawSphere(GetWaypoint(i), _gizmoSize);
                Gizmos.DrawLine(GetWaypoint(i), GetWaypoint(j));
            }
        }

        public int GetJ(int i)
        {
            if (i == transform.childCount - 1) 
                return 0;
            else
                return i + 1;
        }

        public Vector3 GetWaypoint(int i)
        {
            return transform.GetChild(i).position;
        }
    }
}