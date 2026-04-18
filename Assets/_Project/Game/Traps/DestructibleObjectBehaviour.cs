using Game.Traps;
using UnityEngine;
namespace Game.Traps
{
    public class DestructibleObjectBehaviour : ITrapBehaviour
    {
        private int _hitPoints;
        private readonly int _damage;
        private GameObject _root;

        public DestructibleObjectBehaviour(int damage, GameObject root, int hitpoints)
        {
            _damage = damage;
            _root = root;
            _hitPoints = hitpoints;
        }

        public void ApplyDamage()
        {
            _hitPoints -= _damage;

            if (_hitPoints <= 0)
            {
                Object.Destroy(_root);
            }
        }

        public void OnPlayerEnter(HealthController healthController) { }
    }
}