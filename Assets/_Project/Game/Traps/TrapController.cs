using System.Collections.Generic;

namespace Game.Traps
{
    public class TrapController
    {
        private readonly IEnumerable<ITrapBehaviour> _behaviours;

        public TrapController(IEnumerable<ITrapBehaviour> behaviours)
        {
            _behaviours = behaviours;
        }

        public void OnPlayerEnter(HealthController healthController)
        {
            foreach (var behaviour in _behaviours)
            {
                behaviour.OnPlayerEnter(healthController);
            }
        }
    }
}