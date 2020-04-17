using RPG.Control;

namespace RPG.Core
{
    public interface IRaycastable
    {
        bool HandleRaycast(PlayerController callingController);
        CursorType GetCursorType();
        bool IsMovementRequired();
    }
}