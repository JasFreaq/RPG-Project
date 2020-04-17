using UnityEngine;
using RPG.Movement;
using RPG.Combat;
using UnityEngine.EventSystems;
using RPG.Core;
using System;

namespace RPG.Control
{
    public class PlayerController : MonoBehaviour
    {
        [System.Serializable]
        struct CursorMapping
        {
            public CursorType type;
            public Texture2D icon;
        }

        Mover _mover;
        Fighter _fighter;

        [Header("Cursor Config")]
        [SerializeField] CursorMapping[] _cursorMappings;

        bool _cursorOverInteractable = false;

        // Start is called before the first frame update
        void Awake()
        {
            _mover = GetComponent<Mover>();
            _fighter = GetComponent<Fighter>();
        }

        // Update is called once per frame
        void Update()
        {
            if (!InteractWithUI())
            {
                RaycastHit[] hits = RaycastHitSorted();

                _cursorOverInteractable = InteractWithComponent(hits);

                if (InteractWithCombat(hits)) return;

                if (InteractWithMovement()) return;

                SetCursor(CursorType.None);
            }
        }
        
        private bool InteractWithUI()
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                SetCursor(CursorType.UI);
                return true;
            }

            return false;
        }

        private bool InteractWithComponent(RaycastHit[] hits)
        {
            foreach (RaycastHit hit in hits)
            {
                IInteractable[] interactables = hit.transform.GetComponents<IInteractable>();
                if (interactables != null)
                {
                    foreach (IInteractable interactable in interactables)
                    {
                        if (interactable.HandleRaycast())
                        {
                            SetCursor(interactable.GetCursorType());

                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private bool InteractWithCombat(RaycastHit[] hits)
        {
            foreach(RaycastHit hit in hits)
            {
                CombatTarget target = hit.transform.GetComponent<CombatTarget>();
                if (target)
                {
                    if (Input.GetMouseButtonDown(0))
                        _fighter.Attack(target.gameObject);

                    SetCursor(CursorType.Combat);
                    return true;
                }
            }
            return false;
        }
             
        RaycastHit[] RaycastHitSorted()
        {
            RaycastHit[] hits = Physics.RaycastAll(GetMouseRay());

            float[] distances = new float[hits.Length];
            for (int i = 0; i < hits.Length; i++)
            {
                distances[i] = hits[i].distance;
            }

            Array.Sort(distances, hits);

            return hits;
        }

        private bool InteractWithMovement()
        {
            RaycastHit hit;
            bool hasHit = Physics.Raycast(GetMouseRay(), out hit);
            
            if (hasHit)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    _mover.MoveToCursor(hit.point);
                }

                if (!_cursorOverInteractable) 
                    SetCursor(CursorType.Movement);

                return true;
            }

            return false;
        }
        
        private void SetCursor(CursorType cursorType)
        {
            CursorMapping mapping = GetCursorMapping(cursorType);
            Cursor.SetCursor(mapping.icon, Vector2.zero, CursorMode.Auto);
        }

        private CursorMapping GetCursorMapping(CursorType cursorType)
        {
            foreach(CursorMapping mapping in _cursorMappings)
            {
                if (mapping.type == cursorType)
                    return mapping;
            }

            Debug.LogError("No CursorMapping found.");
            return new CursorMapping();
        }

        private static Ray GetMouseRay()
        {
            return Camera.main.ScreenPointToRay(Input.mousePosition);
        }

        private void Kill()
        {
            this.enabled = false;
        }
    }
}