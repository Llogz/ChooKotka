using UnityEngine;
using System.Collections;

namespace Game.Traps
{
    public class DestroyableOnTouchView : TrapBehaviourView
    {
        [SerializeField] private float _damageDelay = 0.5f;

        private DestroyableOnTouchBehaviour _behaviour;
        private DestructibleObjectView _destructible;

        private Coroutine _damageCoroutine;
        private bool _playerOnPlatform;

        private void Awake()
        {
            _destructible = GetComponent<DestructibleObjectView>();
        }

        public override ITrapBehaviour CreateBehaviour()
        {
            _behaviour = new DestroyableOnTouchBehaviour(_destructible);
            return _behaviour;
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (!collision.gameObject.TryGetComponent(out HealthController health))
                return;

            _playerOnPlatform = true;

            if (_damageCoroutine == null)
                _damageCoroutine = StartCoroutine(DamageLoop());
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            if (!collision.gameObject.TryGetComponent(out HealthController health))
                return;

            _playerOnPlatform = false;

            if (_damageCoroutine != null)
            {
                StopCoroutine(_damageCoroutine);
                _damageCoroutine = null;
            }
        }

        private IEnumerator DamageLoop()
        {
            while (_playerOnPlatform)
            {
                yield return new WaitForSeconds(_damageDelay);
                _behaviour.DamagePlatform();
            }
        }

    }
}