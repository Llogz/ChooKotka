namespace Game.System.Interaction
{
    public interface IInteractable
    {
        bool CanInteract(IInteractionContext context);
        void Interact(IInteractionContext context);
    }
}