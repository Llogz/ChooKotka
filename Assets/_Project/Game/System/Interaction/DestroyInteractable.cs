using UnityEngine;

namespace Game.System.Interaction
{
    public sealed class DestroyInteractable : InteractableBase
    {
        [SerializeField] private GameObject _target;

        public override void Interact(IInteractionContext context)
        {
            if (_target != null)
                Object.Destroy(_target);
        }
    }
}