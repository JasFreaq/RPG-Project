using Campbell.Control;
using UnityEngine;

namespace Campbell.Core
{
    public interface IRaycastable
    {
        bool IsRaycastHit(out CursorType cursorType, out RaycastableType raycastableType);

        Transform GetTransform();
    }
}