using UnityEngine;
using ForestSlice.Core;

namespace ForestSlice.Combat
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Projectile : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float speed = 10f;
        [SerializeField] private float lifetime = 3f;

        [Header("Damage")]
        [SerializeField] private float damage = 20f;
        [SerializeField] private float knockbackForce = 2f;
        [SerializeField] private LayerMask targetLayers;

        private Rigidbody2D rb;
        private GameObject owner;
        private Vector2 direction;
        private float timer;

        public void Initialize(GameObject owner, Vector2 direction, float damage, float speed, LayerMask targetLayers)
        {
            this.owner = owner;
            this.direction = direction.normalized;
            this.damage = damage;
            this.speed = speed;
            this.targetLayers = targetLayers;
            timer = lifetime;

            if (rb == null)
                rb = GetComponent<Rigidbody2D>();

            rb.velocity = this.direction * this.speed;

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }

        private void Update()
        {
            timer -= Time.deltaTime;
            if (timer <= 0f)
            {
                Destroy(gameObject);
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject == owner) return;

            if (((1 << collision.gameObject.layer) & targetLayers) == 0) return;

            IDamageable damageable = collision.GetComponent<IDamageable>();
            if (damageable != null && damageable.IsAlive)
            {
                Vector2 hitPoint = collision.ClosestPoint(transform.position);
                Vector2 knockbackDir = direction;

                DamagePacket packet = new DamagePacket(damage, owner, hitPoint)
                {
                    knockbackDirection = knockbackDir,
                    knockbackForce = knockbackForce,
                    type = DamageType.Magic
                };

                damageable.TakeDamage(packet);
                Destroy(gameObject);
            }
        }
    }
}
