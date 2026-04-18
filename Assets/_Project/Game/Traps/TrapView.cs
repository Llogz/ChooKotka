using System.Collections.Generic;
using Game.Player;
using UnityEngine;

namespace Game.Traps
{
    public sealed class TrapView : MonoBehaviour
    {
        private TrapController _controller;

        private void Awake()
        {
            Initialize();
        }

        private void Initialize()
        {
            var behaviourViews = GetComponents<TrapBehaviourView>();

            var behaviours = new List<ITrapBehaviour>();

            foreach (var view in behaviourViews)
            {
                behaviours.Add(view.CreateBehaviour());
            }

            _controller = new TrapController(behaviours);
            
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent<Controller>(out var player) == false) return;
                
            if (player.TryGetComponent<HealthController>(out var playerHealth) == false) return;
                
            _controller.OnPlayerEnter(playerHealth);
        }
    }

}