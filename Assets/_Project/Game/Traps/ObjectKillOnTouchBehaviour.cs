using UnityEngine;

namespace Game.Traps
{
    public class ObjectKillOnTouchBehaviour : ITrapBehaviour 
    {
        public void OnPlayerEnter(HealthController healthController)
        {
            healthController.Kill();
        }
    }
}
