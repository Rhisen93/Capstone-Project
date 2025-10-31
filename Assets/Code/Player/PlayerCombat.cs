using UnityEngine;

namespace ForestSlice.Player
{
    public class PlayerCombat : MonoBehaviour
    {
        [Header("Attack Settings")]
        [SerializeField] private float attackDamage = 15f;
        [SerializeField] private float attackRange = 1.5f;
        [SerializeField] private float attackCooldown = 0.5f;
        [SerializeField] private float attackKnockback = 3f;
        [SerializeField] private LayerMask enemyLayers;

        [Header("Hitbox")]
        [SerializeField] private GameObject hitboxPrefab;
        [SerializeField] private Transform attackPoint;

        private PlayerController controller;
        private float attackCooldownTimer = 0f;
        private bool isAttacking = false;
        private float attackAnimationTimer = 0f;
        private float attackAnimationDuration = 0.3f;

        public bool CanAttack => attackCooldownTimer <= 0f && !isAttacking;

        private void Awake()
        {
            controller = GetComponent<PlayerController>();

            if (attackPoint == null)
            {
                GameObject point = new GameObject("AttackPoint");
                point.transform.SetParent(transform);
                point.transform.localPosition = Vector3.right * attackRange;
                attackPoint = point.transform;
            }
        }

        private void Update()
        {
            UpdateCooldown();
            UpdateAttackAnimation();
        }

        public void Attack()
        {
            if (!CanAttack) return;

            isAttacking = true;
            attackCooldownTimer = attackCooldown;
            attackAnimationTimer = attackAnimationDuration;

            PerformAttack();
        }

        private void PerformAttack()
        {
            Vector2 attackDirection = controller.AimDirection;
            Vector2 attackPosition = (Vector2)transform.position + attackDirection * attackRange;

            Debug.Log($"Attacking at position: {attackPosition}, direction: {attackDirection}");

            if (hitboxPrefab != null)
            {
                GameObject hitboxObj = Instantiate(hitboxPrefab, attackPosition, Quaternion.identity);
                Combat.Hitbox hitbox = hitboxObj.GetComponent<Combat.Hitbox>();
                
                if (hitbox != null)
                {
                    hitbox.Initialize(gameObject, attackDamage, attackKnockback);
                }
            }
            else
            {
                Collider2D[] hits = Physics2D.OverlapCircleAll(attackPosition, 0.5f, enemyLayers);

                Debug.Log($"Found {hits.Length} potential targets in attack range");

                foreach (var hit in hits)
                {
                    Debug.Log($"Hit: {hit.gameObject.name} on layer {hit.gameObject.layer}");
                    
                    Core.IDamageable damageable = hit.GetComponent<Core.IDamageable>();
                    if (damageable != null && damageable.IsAlive)
                    {
                        Vector2 knockbackDir = (hit.transform.position - transform.position).normalized;
                        
                        Core.DamagePacket packet = new Core.DamagePacket(attackDamage, gameObject, attackPosition)
                        {
                            knockbackDirection = knockbackDir,
                            knockbackForce = attackKnockback,
                            type = Core.DamageType.Physical
                        };

                        damageable.TakeDamage(packet);
                        Debug.Log($"Dealt {attackDamage} damage to {hit.gameObject.name}");
                    }
                    else
                    {
                        Debug.LogWarning($"{hit.gameObject.name} has no IDamageable component or is dead");
                    }
                }
            }
        }

        private void UpdateCooldown()
        {
            if (attackCooldownTimer > 0f)
            {
                attackCooldownTimer -= Time.deltaTime;
            }
        }

        private void UpdateAttackAnimation()
        {
            if (!isAttacking) return;

            attackAnimationTimer -= Time.deltaTime;
            if (attackAnimationTimer <= 0f)
            {
                isAttacking = false;
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (controller != null)
            {
                Vector2 attackPosition = (Vector2)transform.position + controller.AimDirection * attackRange;
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(attackPosition, 0.5f);
            }
        }
    }
}
