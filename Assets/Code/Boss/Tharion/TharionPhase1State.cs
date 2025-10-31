using UnityEngine;
using ForestSlice.Combat;

namespace ForestSlice.Boss.Tharion
{
    /// <summary>
    /// Tharion Phase 1 - Basic melee attacks and charge
    /// </summary>
    public class TharionPhase1State : BossState
    {
        private TharionBoss tharion;
        private float attackCooldown = 2f;
        private float attackTimer = 0f;
        private float chargeDistance = 8f;
        private float attackRange = 2f;

        public TharionPhase1State(BossController boss, BossStateMachine stateMachine) : base(boss, stateMachine)
        {
            tharion = boss as TharionBoss;
        }

        public override void Enter()
        {
            attackTimer = 0f;
            Debug.Log("Tharion Phase 1 - Melee attacks");
        }

        public override void Execute()
        {
            if (boss.PlayerTransform == null) return;

            float distanceToPlayer = boss.GetDistanceToPlayer();
            attackTimer += Time.deltaTime;

            // If player is far, charge towards them
            if (distanceToPlayer > chargeDistance)
            {
                boss.MoveTowardsPlayer();
            }
            // If in attack range, perform melee attack
            else if (distanceToPlayer <= attackRange && attackTimer >= attackCooldown)
            {
                PerformMeleeAttack();
                attackTimer = 0f;
            }
            // Move towards player
            else if (distanceToPlayer > attackRange)
            {
                boss.MoveTowardsPlayer();
            }
            else
            {
                boss.StopMovement();
                boss.FacePlayer();
            }
        }

        private void PerformMeleeAttack()
        {
            Debug.Log("Tharion melee attack!");
            boss.FacePlayer();

            // Create damage hitbox
            Collider2D[] hits = Physics2D.OverlapCircleAll(
                boss.transform.position, 
                attackRange, 
                LayerMask.GetMask("Player")
            );

            foreach (var hit in hits)
            {
                var player = hit.GetComponent<Core.IDamageable>();
                if (player != null)
                {
                    var damagePacket = new Core.DamagePacket(20f, boss.gameObject, boss.transform.position)
                    {
                        knockbackForce = 5f,
                        knockbackDirection = (hit.transform.position - boss.transform.position).normalized
                    };
                    player.TakeDamage(damagePacket);
                }
            }
        }

        public override void Exit()
        {
            boss.StopMovement();
        }
    }
}
