using UnityEngine;
using ForestSlice.Core;
using System.Collections;

namespace ForestSlice.Player.Abilities
{
    [CreateAssetMenu(fileName = "DashStrikeAbility", menuName = "ForestSlice/Abilities/DashStrike")]
    public class DashStrikeAbility : AbilityBase
    {
        [Header("Dash Settings")]
        public float dashDistance = 5f;
        public float dashDuration = 0.3f;
        public float aoeRadius = 2f;
        public LayerMask targetLayers;

        public override void Execute(PlayerController player)
        {
            PlayerMovement movement = player.GetComponent<PlayerMovement>();
            if (movement != null)
            {
                player.StartCoroutine(DashStrikeRoutine(player));
            }
        }

        private IEnumerator DashStrikeRoutine(PlayerController player)
        {
            PlayerMovement movement = player.GetComponent<PlayerMovement>();
            Health health = player.GetComponent<Health>();
            
            Vector2 dashDirection = player.AimDirection;
            Vector2 startPosition = player.transform.position;
            Vector2 targetPosition = startPosition + dashDirection * dashDistance;

            float elapsed = 0f;

            if (health != null)
            {
                health.SetInvulnerable(true);
            }

            while (elapsed < dashDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / dashDuration;
                player.transform.position = Vector2.Lerp(startPosition, targetPosition, t);
                yield return null;
            }

            if (health != null)
            {
                health.SetInvulnerable(false);
            }

            DealAoEDamage(player);

            if (impactVFX != null)
            {
                Instantiate(impactVFX, player.transform.position, Quaternion.identity);
            }
        }

        private void DealAoEDamage(PlayerController player)
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(player.transform.position, aoeRadius, targetLayers);

            foreach (var hit in hits)
            {
                IDamageable damageable = hit.GetComponent<IDamageable>();
                if (damageable != null && damageable.IsAlive)
                {
                    Vector2 knockbackDir = (hit.transform.position - player.transform.position).normalized;
                    
                    DamagePacket packet = new DamagePacket(damage, player.gameObject, player.transform.position)
                    {
                        knockbackDirection = knockbackDir,
                        knockbackForce = 5f,
                        type = DamageType.Physical
                    };

                    damageable.TakeDamage(packet);
                }
            }

            Debug.Log($"DashStrike hit {hits.Length} enemies");
        }

        public override bool CanExecute(PlayerController player)
        {
            return true;
        }
    }
}
