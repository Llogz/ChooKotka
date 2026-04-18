using Core.Services;
using Cysharp.Threading.Tasks;
using Game.System.Services.AI;
using UnityEngine;

namespace Game.Player.Pet
{
    public class PetInteractJump : PetNodeInteraction
    {
        public override UpdateType UpdateType { get; set; } = UpdateType.Game;

        public override bool BlockPet => false;
        public override Node StartNode => startNode;

        [SerializeField] private Node startNode;
        [SerializeField] private Rigidbody2D player;
        [SerializeField] private Vector2 force;
        
        public override UniTask OnInteract()
        {
            player.linearVelocity += force;
            return UniTask.CompletedTask;
        }
    }
}