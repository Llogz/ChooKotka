
namespace Game.Traps
{

    public sealed class DamageOnTouchBehaviour : ITrapBehaviour
    {
        private readonly int _damage;
        public DamageOnTouchBehaviour(int damage)
        {
            _damage = damage;
        }
        public void DealDamage(HealthController healthController)
        {
            healthController.ApplyDamage(_damage);
        }
        public void OnPlayerEnter(HealthController healthController)
        {
        }
        
    }
}