using UnityEngine;

namespace Game.System.Interaction
{
    public interface IInteractionContext
    {
        GameObject Instigator { get; }
    }
}