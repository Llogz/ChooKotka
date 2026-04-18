using Core;
using Core.Services;
using UnityEngine;
using VContainer;

namespace Game.Traps
{
    [RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
    public class FallingOnEnterView : TrapBehaviourView
    {
        
        [SerializeField] private float _delay = 1f;
        [SerializeField] private int _damage = 10;
        
        [SerializeField] private Rigidbody2D _rb;

        [SerializeField] private Collider2D _activationTrigger;

        private ITimerService _timer;
        private ILifetime _lifetime;

        private ITrapBehaviour _behaviour;

        public override ITrapBehaviour CreateBehaviour()
        {
            _behaviour = new FallingOnEnterBehaviour(_rb, _timer, _delay, UpdateType, _lifetime);

            return _behaviour;
        }
        [Inject]
        private void Init(ITimerService timer, ILifetime lifetime)
        {
            _timer = timer;
            _lifetime = lifetime;
        }
    }
}