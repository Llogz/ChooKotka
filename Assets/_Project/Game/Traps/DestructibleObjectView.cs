using UnityEngine;

namespace Game.Traps
{
    public class DestructibleObjectView : TrapBehaviourView
    {
        [SerializeField] private int _damage = 1;
        [SerializeField] private int _hitpoints = 5;

        private DestructibleObjectBehaviour _behaviour;

        public override ITrapBehaviour CreateBehaviour()
        {
            _behaviour = new DestructibleObjectBehaviour(_damage, gameObject, _hitpoints);
            return _behaviour;
        }

        public void ApplyDamage()
        {
            _behaviour.ApplyDamage();
        }
    }
}