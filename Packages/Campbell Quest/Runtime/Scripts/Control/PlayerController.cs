using UnityEngine;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using Campbell.Combat;
using Campbell.Core;
using Campbell.Dialogues;
using Campbell.Movement;
using UnityEngine.AI;

namespace Campbell.Control
{
    public class PlayerController : MonoBehaviour
    {
        [System.Serializable]
        struct CursorMapping
        {
            public CursorType type;
            public Texture2D icon;
        }

        private Mover _mover;
        private Fighter _fighter;
        private PlayerConversationHandler _playerConversationHandler;

        [Header("Cursor Config")]
        [SerializeField] float _selectionRadius = 0.25f;
        [SerializeField] CursorMapping[] _cursorMappings = null;
        [SerializeField] float _maxNavMeshProjectionDist = 1f, _maxNavPathLength = 25f;

        bool _cursorOverInteractable = false;
        bool _isdraggingUI = false;

        // Start is called before the first frame update
        void Awake()
        {
            _mover = GetComponent<Mover>();
            _fighter = GetComponent<Fighter>();
            _playerConversationHandler = GetComponent<PlayerConversationHandler>();
        }

        // Update is called once per frame
        void Update()
        {
            if (!InteractWithUI() && !_isdraggingUI)  
            {
                if (_cursorOverInteractable = InteractWithComponent()) 
                    return;

                if (InteractWithMovement()) return;

                SetCursor(CursorType.None);
            }
        }
        
        private bool InteractWithUI()
        {
            if (Input.GetMouseButtonUp(0))
                _isdraggingUI = false;

            if (EventSystem.current.IsPointerOverGameObject())
            {
                SetCursor(CursorType.UI);

                if (Input.GetMouseButtonDown(0))
                    _isdraggingUI = true;

                return true;
            }

            return false;
        }

        private bool InteractWithComponent()
        {
            RaycastHit[] hits = RaycastAllSorted();
            foreach (RaycastHit hit in hits)
            {
                IRaycastable[] raycastables = hit.transform.GetComponents<IRaycastable>();
                foreach (IRaycastable raycastable in raycastables)
                {
                    Transform raycastableTransform = raycastable.GetTransform();
                    bool pathStatus = GetPathStatus(raycastableTransform.position);

                    if (pathStatus || 
                        (Vector3.Distance(raycastableTransform.position, transform.position) <= _fighter.GetWeaponsRange() && _fighter.GetProjectileStatus())) 
                    {
                        CursorType cursorType;
                        RaycastableType raycastableType;
                        if (raycastable.IsRaycastHit(out cursorType, out raycastableType))
                        {
                            SetCursor(cursorType);

                            if (raycastableType == RaycastableType.Enemy)
                            {
                                if (Input.GetMouseButtonDown(0))
                                {
                                    _fighter.Attack(raycastableTransform.gameObject);
                                }
                            }
                            else if (raycastableType == RaycastableType.Dialogue)
                            {
                                if (Input.GetMouseButtonDown(0))
                                {
                                    StartCoroutine(InteractWithDialogueRoutine(raycastableTransform));
                                }
                            }
                            else
                            {
                                if (pathStatus)
                                {
                                    switch (raycastableType)
                                    {
                                        case RaycastableType.ProximityPickup:
                                            InteractWithMovement();
                                            break;
                                        case RaycastableType.ClickablePickup:
                                            break;
                                    }
                                }
                                else
                                    SetCursor(CursorType.None);
                            }

                            return true;
                        }
                    }
                }
            }
            return false;
        }

        RaycastHit[] RaycastAllSorted()
        {
            RaycastHit[] hits = Physics.SphereCastAll(GetMouseRay(), _selectionRadius);

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
            Vector3 target;

            if (RaycastNavMesh(out target))
            {
                if (Input.GetMouseButtonDown(0))
                {
                    _mover.MoveToCursor(target);
                }

                if (!_cursorOverInteractable) 
                    SetCursor(CursorType.Movement);

                return true;
            }

            return false;
        }

        IEnumerator InteractWithDialogueRoutine(Transform nPCTransform)
        {
            bool reached = false;
            Action checkReached = () =>
            {
                reached = true;
            };

            _mover.MoveToObject(nPCTransform);
            _mover.RegisterOnDestinationReached(checkReached);

            while (!reached)
            {
                yield return new WaitForEndOfFrame();
            }

            AIConversationHandler aiConversationHandler = nPCTransform.GetComponent<AIConversationHandler>();
            _playerConversationHandler.StartDialogue(aiConversationHandler);

            _mover.DeregisterOnDestinationReached(checkReached);
        }

        bool RaycastNavMesh(out Vector3 target)
        {
            RaycastHit hit;
            bool hasHit = Physics.Raycast(GetMouseRay(), out hit);
            target = hit.point;

            if (!hasHit) return false;

            NavMeshHit navMeshHit;
            bool hasHitNavMesh = NavMesh.SamplePosition(target, out navMeshHit, _maxNavMeshProjectionDist, NavMesh.AllAreas);

            if (!hasHitNavMesh) return false;

            if (!GetPathStatus(target))
                return false;

            return true;
        }

        private bool GetPathStatus(Vector3 target)
        {
            NavMeshPath path = new NavMeshPath();
            bool hasPath = NavMesh.CalculatePath(transform.position, target, NavMesh.AllAreas, path);
            
            if (!hasPath) return false;
            if (path.status != NavMeshPathStatus.PathComplete) return false;
            if (GetNavPathLength(path) > _maxNavPathLength) return false;

            return true;
        }

        private float GetNavPathLength(NavMeshPath path)
        {
            float distance = 0;

            if (path.corners.Length < 2) return Mathf.Infinity;

            for (int i = 0; i < path.corners.Length - 1; i++)
            {
                distance += Vector3.Distance(path.corners[i], path.corners[i + 1]);
            }

            return distance;
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