using RPG.Control;

namespace RPG.Core
{
    public interface IInteractable
    {
        bool HandleRaycast();
        CursorType GetCursorType();
    }
}