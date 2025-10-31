using UnityEngine;
using ForestSlice.Core;

namespace ForestSlice.Combat
{
    public class Hitbox : MonoBehaviour
    {
        [SerializeField] private float damage = 10f;
        [SerializeField] private float knockbackForce = 5f;
        [SerializeField] private DamageType damageType = DamageType.Physical;
        [SerializeField] private LayerMask targetLayers;
        [SerializeField] private bool destroyOnHit = false;
        [SerializeField] private float lifetime = 0.2f;

        private GameObject owner;
        private float timer;
        private bool hasHit = false;

        public void Initialize(GameObject owner, float damage, float knockback = 0f)
        {
            this.owner = owner;
            this.damage = damage;
            this.knockbackForce = knockback;
            timer = lifetime;
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
            if (hasHit && destroyOnHit) return;
            if (collision.gameObject == owner) return;

            if (((1 << collision.gameObject.layer) & targetLayers) == 0) return;

            IDamageable damageable = collision.GetComponent<IDamageable>();
            if (damageable != null && damageable.IsAlive)
            {
                Vector2 hitPoint = collision.ClosestPoint(transform.position);
                Vector2 knockbackDir = (collision.transform.position - transform.position).normalized;

                DamagePacket packet = new DamagePacket(damage, owner, hitPoint)
                {
                    knockbackDirection = knockbackDir,
                    knockbackForce = knockbackForce,
                    type = damageType
                };

                damageable.TakeDamage(packet);
                hasHit = true;

                if (destroyOnHit)
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}
