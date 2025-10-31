using UnityEngine;
using ForestSlice.Combat;

namespace ForestSlice.Enemy
{
    /// <summary>
    /// Ranged enemy archetype - Shoots projectiles at player from distance
    /// Tries to maintain optimal distance, kites away if player gets too close
    /// </summary>
    public class RangedEnemy : EnemyController
    {
        [Header("Ranged Settings")]
        [SerializeField] private GameObject projectilePrefab;
        [SerializeField] private Transform firePoint;
        [SerializeField] private float projectileSpeed = 8f;
        [SerializeField] private float optimalDistance = 5f; // Preferred distance from player
        [SerializeField] private float tooCloseDistance = 3f; // Distance to start retreating
        [SerializeField] private LayerMask projectileTargetLayers;

        protected override void Start()
        {
            base.Start();

            // Create fire point if not assigned
            if (firePoint == null)
            {
                GameObject fp = new GameObject("FirePoint");
                fp.transform.SetParent(transform);
                fp.transform.localPosition = Vector3.right * 0.5f;
                firePoint = fp.transform;
            }
        }

        protected override void Update()
        {
            base.Update();

            // Custom behavior: Maintain distance from player
            if (stateMachine.CurrentState == ChaseState && playerTransform != null)
            {
                float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

                // Too close - back away
                if (distanceToPlayer < tooCloseDistance)
                {
                    Vector2 retreatDirection = (transform.position - playerTransform.position).normalized;
                    rb.velocity = retreatDirection * chaseSpeed * 0.7f; // Move away at 70% speed
                    FaceTarget(playerTransform.position); // Still face player while backing up
                }
            }
        }

        public override void PerformAttack()
        {
            if (projectilePrefab == null)
            {
                Debug.LogWarning($"{name}: Projectile prefab not assigned!");
                return;
            }

            if (playerTransform == null) return;

            Debug.Log($"{name} fires projectile!");

            // Calculate direction to player
            Vector2 direction = (playerTransform.position - firePoint.position).normalized;

            // Spawn projectile
            GameObject projectileObj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
            Projectile projectile = projectileObj.GetComponent<Projectile>();

            if (projectile != null)
            {
                projectile.Initialize(
                    owner: gameObject,
                    direction: direction,
                    damage: attackDamage,
                    speed: projectileSpeed,
                    targetLayers: projectileTargetLayers
                );
            }
            else
            {
                Debug.LogError($"{name}: Projectile prefab missing Projectile component!");
                Destroy(projectileObj);
            }
        }

        protected override void OnDrawGizmosSelected()
        {
            base.OnDrawGizmosSelected();

            // Draw optimal distance
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, optimalDistance);

            // Draw too close distance
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, tooCloseDistance);

            // Draw fire point
            if (firePoint != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(firePoint.position, 0.2f);
            }
        }
    }
}
