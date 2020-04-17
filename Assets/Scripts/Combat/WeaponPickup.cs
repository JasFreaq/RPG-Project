using RPG.Control;
using RPG.Core;
using UnityEngine;

namespace RPG.Combat
{
    [SelectionBase]
    public class WeaponPickup : MonoBehaviour, IInteractable
    {
        [SerializeField] Weapon _weapon;

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

        public CursorType GetCursorType()
        {
            return CursorType.Interactable_Pickup;
        }
    }
}