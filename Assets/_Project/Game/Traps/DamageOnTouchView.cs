using UnityEngine;

namespace Game.Traps
{
    public sealed class DamageOnTouchView : TrapBehaviourView
    {
        [SerializeField] private int _damage = 1;
        [SerializeField] private Collider2D _damageCollider;

        private DamageOnTouchBehaviour _behaviour;

        // Мы не храним поведение как состояние.
        // Оно создаётся при инициализации контроллера.
        public override ITrapBehaviour CreateBehaviour()
        {
            _behaviour = new DamageOnTouchBehaviour(_damage);
            return _behaviour;
        }
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!_damageCollider.IsTouching(other))
                return;

            if (!other.TryGetComponent(out HealthController hc))
                return;

            _behaviour.DealDamage(hc);
        }
    }

}