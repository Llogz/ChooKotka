using UnityEngine;

namespace Game.Traps
{
    public class ObjectKillObTouchView : TrapBehaviourView
    {
        [SerializeField] private Collider2D _killCollider;
        private ObjectKillOnTouchBehaviour _behaviour;
        public override ITrapBehaviour CreateBehaviour()
        {
            _behaviour = new ObjectKillOnTouchBehaviour();
            return _behaviour;
        }
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!_killCollider.IsTouching(other))
                return;

            if (!other.TryGetComponent(out HealthController hc))
                return;

            _behaviour.OnPlayerEnter(hc);
        }
    }
}
