using System.Linq;
using Core;
using Core.Services;
using Cysharp.Threading.Tasks;
using Game.System;
using Game.System.Services.AI;
using UnityEngine;
using VContainer;

namespace Game.Player.Pet
{
    public class Pet : UpdatableBehaviour
    {
        [Header("Navigation Setup")]
        [SerializeField] private Rigidbody2D rg;
        [SerializeField] private NodeProvider provider;
        [SerializeField] private float pathFindDelay = 0.5f;
        [SerializeField] private Transform target;
        
        [Header("Interaction")]
        [SerializeField] private PetInteract interact;
        [SerializeField] private PetNodeInteraction[] nodeInteractions;
        
        [Header("Animation")]
        [SerializeField] private SpriteRenderer spr;
        [SerializeField] private float moveSens = 0.1f;
        [SerializeField] private AnimController animController;
        [SerializeField] private string idleAnim;
        [SerializeField] private float idleAnimTime;
        [SerializeField] private string runAnim;
        [SerializeField] private float runAnimTime;

        [Inject] private ITimerService _timerService;
        [Inject] private INodeService _nodeService;
        [Inject] private ILifetime _lifetime;

        public override UpdateType UpdateType { get; set; } = UpdateType.Game;
        
        private bool IsBlocked() => nodeInteractions.Any(interaction => interaction.BlockPet);
        
        private void Start()
        {
            Move().Forget();
        }

        private async UniTask Move()
        {
            while (!IsDestroyed)
            {
                if (IsBlocked())
                {
                    await UniTask.Yield();
                    continue;
                }

                await _timerService.Delay(pathFindDelay, () =>
                {
                    if (provider.CurrentMoveType == Node.NodeType.Jump) return;
                    var targetNode = Node.FindClosestNode(_nodeService.GetAllNodes()[0], target.position);
                    provider.MoveTo(targetNode);
                }, UpdateType, _lifetime);
            }
        }

        public override void OnEnable()
        {
            base.OnEnable();
            interact.OnInteraction += OnInteract;
        }

        public override void OnDisable()
        {
            base.OnDisable();
            interact.OnInteraction -= OnInteract;
        }

        public override void GameUpdate(float dt)
        {
            if (Mathf.Abs(rg.linearVelocity.x) > moveSens)
            {
                spr.flipX = rg.linearVelocity.x < 0f;
                animController.PullAnimation(runAnim, runAnimTime);
            }
            else
            {
                animController.PullAnimation(idleAnim, idleAnimTime);
            }
        }
        
        private void OnInteract()
        {
            foreach (var interaction in nodeInteractions)
            {
                if (interaction.StartNode == provider.CurrentNode)
                    interaction.OnInteract().Forget();
            }
        }
    }
}