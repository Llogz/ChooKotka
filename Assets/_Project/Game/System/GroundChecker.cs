using System.Collections.Generic;
using UnityEngine;

namespace Game.System
{
    public class GroundChecker : MonoBehaviour
    {
        public bool IsTouchingGround => _onGround;
        private bool _onGround = false;
        
        private readonly List<Collider2D> _colliders = new();

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.GetComponent<Ground>())
            {
                _colliders.Add(collision);
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.GetComponent<Ground>())
            {
                _colliders.Remove(collision);
            }
        }

        private void FixedUpdate()
        {
            _onGround = _colliders.Count > 0;
        }
    }
}
