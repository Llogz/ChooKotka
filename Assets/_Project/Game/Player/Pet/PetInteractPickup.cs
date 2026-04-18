using Core;
using Core.Services;
using Cysharp.Threading.Tasks;
using Game.System.Services.AI;
using UnityEngine;
using VContainer;

namespace Game.Player.Pet
{
    public class PetInteractPickup : PetNodeInteraction
    {
        public override UpdateType UpdateType { get; set; } = UpdateType.Game;

        public override bool BlockPet => _block;
        public override Node StartNode => startNode;

        private bool _block = false;

        [Inject] private ILifetime _lifetime;
        
        [SerializeField] private NodeProvider pet;
        [SerializeField] private Controller player;
        [SerializeField] private Node startNode;
        [SerializeField] private Node endNode;
        
        [Header("Jump")]
        [SerializeField] private float speed = 8f;
        [SerializeField] private Transform jumpPos;
        [SerializeField] private float jumpDuration;
        [SerializeField] private float jumpHeight;

        private float _jumpTime = 0f;
        private Vector2 _startPos;
        
        public override async UniTask OnInteract()
        {
            _block = true;
            _jumpTime = 0f;

            pet.MoveTo(endNode);

            _startPos = player.transform.position;

            await RunWhile(
                () => UniTask.CompletedTask,
                () => pet.CurrentNode != endNode,
                PlayerLoopTiming.Update,
                _lifetime
            );

            _block = false;
        }

        public override void GameFixedUpdate(float dt)
        {
            if (!_block) return;

            _jumpTime += dt;

            var t = Mathf.Clamp01(_jumpTime / jumpDuration);

            var dx = Mathf.Abs(jumpPos.position.x - _startPos.x);
            var curX = t * dx;

            var yOffset = ProjMath.EasingFunctions.JumpGraph(
                curX,
                dx,
                jumpPos.position.y - _startPos.y,
                jumpHeight
            );

            var targetPos = new Vector2(
                Mathf.Lerp(_startPos.x, jumpPos.position.x, t),
                _startPos.y + yOffset
            );

            player.transform.position = Vector2.Lerp(
                player.transform.position,
                targetPos,
                speed * dt
            );
        }
    }
}