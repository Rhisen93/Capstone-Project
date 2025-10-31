using UnityEngine;
using ForestSlice.Core;
using System.Collections;

namespace ForestSlice.Enemy
{
    /// <summary>
    /// Charger enemy archetype - Charges at high speed towards player
    /// After charging, needs recovery time before charging again
    /// </summary>
    public class ChargerEnemy : EnemyController
    {
        [Header("Charge Settings")]
        [SerializeField] private float chargeSpeed = 12f;
        [SerializeField] private float chargeDistance = 8f;
        [SerializeField] private float chargeCooldown = 3f;
        [SerializeField] private float chargePreparationTime = 0.5f; // Wind-up time
        [SerializeField] private float chargeRecoveryTime = 1f; // Recovery after charge
        [SerializeField] private LayerMask chargeTargetLayers;

        private bool isCharging = false;
        private bool canCharge = true;
        private Vector2 chargeDirection;
        private float chargeDistanceRemaining;

        // Custom charge state
        private ChargeState chargeState;

        protected override void Awake()
        {
            base.Awake();
            chargeState = new ChargeState(this, stateMachine);
        }

        public override void PerformAttack()
        {
            if (!canCharge) return;

            Debug.Log($"{name} initiates charge attack!");
            StartCoroutine(ChargeAttackRoutine());
        }

        private IEnumerator ChargeAttackRoutine()
        {
            canCharge = false;
            StopMovement();

            // Preparation phase - visual warning
            Debug.Log($"{name} preparing to charge...");
            yield return new WaitForSeconds(chargePreparationTime);

            // Calculate charge direction
            if (playerTransform != null)
            {
                chargeDirection = (playerTransform.position - transform.position).normalized;
                chargeDistanceRemaining = chargeDistance;
                isCharging = true;
                FaceDirection(chargeDirection);

                Debug.Log($"{name} charging!");

                // Charge phase
                while (chargeDistanceRemaining > 0f && isCharging)
                {
                    float step = chargeSpeed * Time.deltaTime;
                    rb.velocity = chargeDirection * chargeSpeed;

                    // Check for collisions during charge
                    CheckChargeCollisions();

                    chargeDistanceRemaining -= step;
                    yield return null;
                }

                isCharging = false;
                StopMovement();

                Debug.Log($"{name} charge ended, recovering...");

                // Recovery phase - stunned/tired
                yield return new WaitForSeconds(chargeRecoveryTime);

                // Reset cooldown
                yield return new WaitForSeconds(chargeCooldown);
                canCharge = true;
                Debug.Log($"{name} ready to charge again!");
            }
        }

        private void CheckChargeCollisions()
        {
            // Check for hits during charge
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, attackRange, chargeTargetLayers);

            foreach (Collider2D hit in hits)
            {
                if (hit.gameObject == gameObject) continue;

                IDamageable damageable = hit.GetComponent<IDamageable>();
                if (damageable != null && damageable.IsAlive)
                {
                    Vector2 hitPoint = hit.ClosestPoint(transform.position);
                    
                    DamagePacket packet = new DamagePacket(attackDamage, gameObject, hitPoint)
                    {
                        knockbackDirection = chargeDirection,
                        knockbackForce = knockbackForce * 1.5f, // Extra knockback during charge
                        type = DamageType.Physical
                    };

                    damageable.TakeDamage(packet);
                    Debug.Log($"{name} charge hit {hit.name}!");

                    // Stop charge on hit
                    isCharging = false;
                    StopMovement();
                }
            }
        }

        protected override void OnDrawGizmosSelected()
        {
            base.OnDrawGizmosSelected();

            // Draw charge distance
            Gizmos.color = Color.green;
            Vector3 chargeDir = transform.localScale.x > 0 ? Vector3.right : Vector3.left;
            Gizmos.DrawRay(transform.position, chargeDir * chargeDistance);
        }

        /// <summary>
        /// Custom charge state for this enemy type
        /// </summary>
        private class ChargeState : EnemyState
        {
            public ChargeState(EnemyController enemy, EnemyStateMachine stateMachine) : base(enemy, stateMachine)
            {
            }

            public override void Enter()
            {
                Debug.Log($"{enemy.name} entered Charge state");
            }

            public override void Execute()
            {
                // Charging logic handled in coroutine
            }

            public override void Exit()
            {
                // Clean up
            }
        }
    }
}
