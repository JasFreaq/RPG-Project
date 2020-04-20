using RPG.Control;
using UnityEngine;

namespace RPG.Core
{
    public interface IRaycastable
    {
        bool HandleRaycast(PlayerController callingController);
        CursorType GetCursorType();
        bool IsMovementRequired();
        Vector3 GetTransform();
    }
}