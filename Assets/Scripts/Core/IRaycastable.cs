using RPG.Control;
using UnityEngine;

namespace RPG.Core
{
    public interface IRaycastable
    {
        bool IsRaycastHit(out CursorType cursorType, out RaycastableType raycastableType);
        Transform GetTransform();
    }
}