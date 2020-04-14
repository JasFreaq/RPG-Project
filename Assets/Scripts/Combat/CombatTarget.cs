using RPG.Resources;
using UnityEngine;

namespace RPG.Combat
{
    [RequireComponent(typeof(Health))]
    public class CombatTarget : MonoBehaviour
    {
        Collider _collider;

        private void Start()
        {
            _collider = GetComponent<Collider>();
        }

        private void Kill()
        {
            Destroy(_collider);
        }
    }
}