using UnityEngine;

namespace Game.System.Interaction
{
    public sealed class PlayerInteractionContext : IInteractionContext
    {
        public GameObject Instigator { get; }

        public PlayerInteractionContext(GameObject instigator)
        {
            Instigator = instigator;
        }
    }
}