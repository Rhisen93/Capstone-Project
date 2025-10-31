using UnityEngine;
using ForestSlice.Combat;

namespace ForestSlice.Boss.Tharion
{
    /// <summary>
    /// Tharion Phase 2 - Enhanced attacks with ground slam AoE
    /// </summary>
    public class TharionPhase2State : BossState
    {
        private TharionBoss tharion;
        private float attackCooldown = 1.5f; // Faster attacks
        private float slamCooldown = 5f;
        private float attackTimer = 0f;
        private float slamTimer = 0f;
        private float attackRange = 2.5f;
        private float slamRadius = 4f;

        public TharionPhase2State(BossController boss, BossStateMachine stateMachine) : base(boss, stateMachine)
        {
            tharion = boss as TharionBoss;
        }

        public override void Enter()
        {
            attackTimer = 0f;
            slamTimer = 0f;
            Debug.Log("Tharion Phase 2 - Enhanced attacks with ground slam!");
        }

        public override void Execute()
        {
            if (boss.PlayerTransform == null) return;

            float distanceToPlayer = boss.GetDistanceToPlayer();
            attackTimer += Time.deltaTime;
            slamTimer += Time.deltaTime;

            // Prioritize ground slam if ready
            if (slamTimer >= slamCooldown && distanceToPlayer <= slamRadius)
            {
                PerformGroundSlam();
                slamTimer = 0f;
                return;
            }

            // If in attack range, perform melee attack
            if (distanceToPlayer <= attackRange && attackTimer >= attackCooldown)
            {
                PerformMeleeAttack();
                attackTimer = 0f;
            }
            // Move towards player faster
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
            Debug.Log("Tharion enhanced melee attack!");
            boss.FacePlayer();

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
                    var damagePacket = new Core.DamagePacket(30f, boss.gameObject, boss.transform.position)
                    {
                        knockbackForce = 7f,
                        knockbackDirection = (hit.transform.position - boss.transform.position).normalized
                    };
                    player.TakeDamage(damagePacket);
                }
            }
        }

        private void PerformGroundSlam()
        {
            Debug.Log("Tharion GROUND SLAM!");
            boss.StopMovement();

            // AoE attack around boss
            Collider2D[] hits = Physics2D.OverlapCircleAll(
                boss.transform.position, 
                slamRadius, 
                LayerMask.GetMask("Player")
            );

            foreach (var hit in hits)
            {
                var player = hit.GetComponent<Core.IDamageable>();
                if (player != null)
                {
                    var damagePacket = new Core.DamagePacket(40f, boss.gameObject, boss.transform.position)
                    {
                        knockbackForce = 10f,
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
