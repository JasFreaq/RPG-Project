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

        public bool HandleRaycast()
        {
            return true;
        }

        public bool HandleRaycast(PlayerController callingController)
        {
            return true;
        }

        public CursorType GetCursorType()
        {
            return CursorType.Pickup;
        }

        public bool IsMovementRequired()
        {
            return true;
        }

        public Vector3 GetTransform()
        {
            return transform.position;
        }
    }
}