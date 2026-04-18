using System;
using Game.System.Interaction;
using UnityEngine;

namespace Game.Player.Pet
{
    public class PetInteract : InteractableBase
    {
        public event Action OnInteraction;
        public override void Interact(IInteractionContext context)
        {
            OnInteraction?.Invoke();
        }
    }
}
