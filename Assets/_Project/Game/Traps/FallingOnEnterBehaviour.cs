using Core;
using Core.Services;
using UnityEngine;

namespace Game.Traps
{
    public class FallingOnEnterBehaviour : ITrapBehaviour
    {
        private readonly Rigidbody2D _rb;

        private readonly ITimerService _timerService;
        private readonly float _delay;
        private readonly UpdateType  _updateType;
        private readonly ILifetime _lifetime;

        public FallingOnEnterBehaviour(
            Rigidbody2D rb, 
            ITimerService timerService, 
            float delay, 
            UpdateType updateType,
            ILifetime lifetime
            )
        {
            _rb = rb;
            _timerService = timerService;
            _delay = delay;
            _updateType = updateType;
            _lifetime = lifetime;
        }

        public void OnPlayerEnter(HealthController healthController)
        {
            _timerService.Delay(_delay, () => _rb.gravityScale = 1f, _updateType, _lifetime);
        }
    }
}
