using UnityEngine;

namespace Game.System.Interaction
{
    public sealed class MoveInteractable : InteractableBase
    {
        [SerializeField] private Transform _target;
        [SerializeField] private Vector3 _destination;

        public override void Interact(IInteractionContext context)
        {
            if (_target != null)
                _target.position = _destination;
        }
    }
}