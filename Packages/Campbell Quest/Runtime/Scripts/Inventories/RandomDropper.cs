using UnityEngine;
using UnityEngine.AI;

namespace Campbell.InventorySystem
{
    public class RandomDropper : ItemDropper
    {
        //Config Data
        [Tooltip("How far the pickups are scattered from the dropper.")]
        [SerializeField] [Range(1,5)] float _scatterDistance = 1;
        
        //Constants
        const int _Attempts = 25;

        protected override Vector3 GetDropLocation()
        {
            for (int i = 0; i < _Attempts; i++)
            {
                Vector3 randomPoint = transform.position + Random.insideUnitSphere * _scatterDistance;
                NavMeshHit hit;
                if (NavMesh.SamplePosition(randomPoint, out hit, 0.1f, NavMesh.AllAreas))
                {
                    return hit.position;
                }
            }

            Debug.LogWarning(this.name + " is probably not on a NavMesh");
            return transform.position;
        }
    }
}