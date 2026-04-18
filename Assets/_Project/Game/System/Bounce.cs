using UnityEngine;

namespace Game.System
{
    public class Bounce : MonoBehaviour
    {
        [SerializeField] private float reflectMultiplier = 1.0f;

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.TryGetComponent(out Bullet bullet))
            {
                Vector2 normal = (other.transform.position - transform.position).normalized;
                var rg = bullet.GetComponent<Rigidbody2D>();
                
                rg.linearVelocity = Vector2.Reflect(rg.linearVelocity, normal) * reflectMultiplier;
            }
        }
    }
}
