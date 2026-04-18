using UnityEngine;

namespace Game.Traps
{
    public class DestroyOnEnterBehaviour : ITrapBehaviour
    {
        private GameObject _root;
        public DestroyOnEnterBehaviour(GameObject root)
        {
            _root = root;
        }
        public void OnPlayerEnter(HealthController healthController)
        {
            GameObject.Destroy(_root);
        }

    }
}
