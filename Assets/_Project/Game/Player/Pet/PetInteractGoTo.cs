using System;
using Core;
using Core.Services;
using Cysharp.Threading.Tasks;
using Game.System.Services.AI;
using UnityEngine;
using VContainer;

namespace Game.Player.Pet
{
    public class PetInteractGoTo : PetNodeInteraction
    {
        public override UpdateType UpdateType { get; set; } = UpdateType.Game;

        public override bool BlockPet => _block;
        public override Node StartNode => startNode;

        private bool _block = false;

        [Inject] private ILifetime _lifetime;
        
        [SerializeField] private NodeProvider pet;
        [SerializeField] private Node startNode;
        [SerializeField] private Node endNode;
        
        public override async UniTask OnInteract()
        {
            pet.MoveTo(endNode);

            _block = true;
            await RunWhile(() => UniTask.CompletedTask, () => pet.CurrentNode != endNode, PlayerLoopTiming.Update, _lifetime);
            _block = false;
        }
    }
}