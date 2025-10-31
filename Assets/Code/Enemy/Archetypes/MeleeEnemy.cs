using UnityEngine;
using ForestSlice.Core;
using ForestSlice.Combat;

namespace ForestSlice.Enemy
{
    /// <summary>
    /// Melee enemy archetype - Chases and attacks player in close range with melee swings
    /// Aggressive behavior, high damage, short attack range
    /// </summary>
    public class MeleeEnemy : EnemyController
    {
        [Header("Melee Settings")]
        [SerializeField] private LayerMask targetLayers;

        protected override void Awake()
        {
            base.Awake();
        }

        public override void PerformAttack()
        {
            Debug.Log($"{name} performs melee attack!");

            // Use OverlapCircle for melee detection
            Vector2 attackOrigin = transform.position;
            Vector2 directionToPlayer = (playerTransform.position - transform.position).normalized;

            // Use OverlapCircle for simple melee detection
            Collider2D[] hits = Physics2D.OverlapCircleAll(attackOrigin, attackRange, targetLayers);

            foreach (Collider2D hit in hits)
            {
                if (hit.gameObject == gameObject) continue; // Don't hit self

                IDamageable damageable = hit.GetComponent<IDamageable>();
                if (damageable != null && damageable.IsAlive)
                {
                    Vector2 hitPoint = hit.ClosestPoint(attackOrigin);
                    Vector2 knockbackDir = (hit.transform.position - transform.position).normalized;

                    DamagePacket packet = new DamagePacket(attackDamage, gameObject, hitPoint)
                    {
                        knockbackDirection = knockbackDir,
                        knockbackForce = knockbackForce,
                        type = DamageType.Physical
                    };

                    damageable.TakeDamage(packet);
                    Debug.Log($"{name} hit {hit.name} for {attackDamage} damage!");
                }
            }
        }

        protected override void OnDrawGizmosSelected()
        {
            base.OnDrawGizmosSelected();

            // Draw melee attack range in front of enemy
            Gizmos.color = Color.magenta;
            Vector3 attackDirection = transform.localScale.x > 0 ? Vector3.right : Vector3.left;
            Gizmos.DrawRay(transform.position, attackDirection * attackRange);
        }
    }
}
