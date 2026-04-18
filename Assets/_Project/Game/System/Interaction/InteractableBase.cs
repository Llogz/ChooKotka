using UnityEngine;

namespace Game.System.Interaction
{
    public abstract class InteractableBase : MonoBehaviour, IInteractable
    {
        public virtual bool CanInteract(IInteractionContext context) => true;

        public abstract void Interact(IInteractionContext context);
    }
}