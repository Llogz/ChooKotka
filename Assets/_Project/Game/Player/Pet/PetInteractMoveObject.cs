using Core;
using Core.Services;
using Cysharp.Threading.Tasks;
using Game.System.Services.AI;
using UnityEngine;
using VContainer;

namespace Game.Player.Pet
{
    public class PetInteractMoveObject : PetNodeInteraction
    {
        public override UpdateType UpdateType { get; set; } = UpdateType.Game;

        public override bool BlockPet => _block;
        public override Node StartNode => startNode;

        private bool _block = false;

        [Inject] private ILifetime _lifetime;

        [Header("Setup")]
        [SerializeField] private NodeProvider pet;
        [SerializeField] private Node startNode;
        [SerializeField] private Node endNode;
        [SerializeField] private Transform objectToMove;
        [SerializeField] private Transform followPoint;

        [SerializeField] private float followSpeed = 5f;
        [SerializeField] private bool expire = true;

        public override async UniTask OnInteract()
        {
            _block = true;

            pet.MoveTo(endNode);

            await RunWhile(
                () => UniTask.CompletedTask, 
                () => pet.CurrentNode != endNode, 
                PlayerLoopTiming.Update, _lifetime);

            _block = false;
            enabled = !expire;
        }
        
        public override void GameUpdate(float dt)
        {
            if (!_block) return;
            
            objectToMove.position = Vector3.Lerp(
                objectToMove.position, 
                followPoint.position,
                followSpeed * dt
            );
        }
    }
}