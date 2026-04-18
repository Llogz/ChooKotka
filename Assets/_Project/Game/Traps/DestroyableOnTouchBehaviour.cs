namespace Game.Traps
{
    public class DestroyableOnTouchBehaviour : ITrapBehaviour
    {
        private readonly DestructibleObjectView _destructible;

        public DestroyableOnTouchBehaviour(DestructibleObjectView destructible)
        {
            _destructible = destructible;
        }

        public void DamagePlatform()
        {
            _destructible.ApplyDamage();
        }

        public void OnPlayerEnter(HealthController healthController)
        {
        }

    }
}