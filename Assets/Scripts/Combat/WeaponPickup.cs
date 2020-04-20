using RPG.Control;
using RPG.Core;
using UnityEngine;

namespace RPG.Combat
{
    [SelectionBase]
    public class WeaponPickup : MonoBehaviour, IRaycastable
    {
        [SerializeField] WeaponConfig _weapon;

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player")
            {
                other.GetComponent<Fighter>().EquipWeapon(_weapon);
                Destroy(gameObject);
            }
        }

        public bool IsRaycastHit(out CursorType cursorType, out RaycastableType raycastableType)
        {
            cursorType = CursorType.Pickup;
            raycastableType = RaycastableType.Pickup;
            return true;
        }

        public bool HandleRaycast()
        {
            return true;
        }

        public Transform GetTransform()
        {
            return transform;
        }
    }
}