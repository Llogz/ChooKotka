using Core.Services;
using Cysharp.Threading.Tasks;
using Game.System.Services.AI;
using UnityEngine;

namespace Game.Player.Pet
{
    public abstract class PetNodeInteraction : UpdatableBehaviour
    {
        public abstract bool BlockPet { get; }
        public abstract Node StartNode { get; }
        public abstract UniTask OnInteract();
    }
}