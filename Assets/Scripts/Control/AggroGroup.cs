using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Combat
{
    public class AggroGroup : MonoBehaviour
    {
        [SerializeField] private List<Fighter> _fighters = new List<Fighter>();

        [SerializeField] private bool _activateOnStart = false;

        void Start()
        {
            Activate(_activateOnStart);
        }

        public void Activate(bool shouldActivate)
        {
            foreach (Fighter fighter in _fighters)
            {
                fighter.enabled = shouldActivate;
                CombatTarget target = fighter.GetComponent<CombatTarget>();
                if (target)
                {
                    target.enabled = shouldActivate;
                }
            }
        }
    }
}
