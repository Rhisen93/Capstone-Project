using UnityEngine;
using ForestSlice.Core;

namespace ForestSlice.Player.Abilities
{
    [CreateAssetMenu(fileName = "AoEAbility", menuName = "ForestSlice/Abilities/AoE")]
    public class AoEAbility : AbilityBase
    {
        [Header("AoE Settings")]
        public float explosionRadius = 3f;
        public LayerMask targetLayers;
        public bool useMousePosition = false;

        public override void Execute(PlayerController player)
        {
            Vector2 explosionPosition;

            if (useMousePosition)
            {
                Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(UnityEngine.InputSystem.Mouse.current.position.ReadValue());
                explosionPosition = new Vector2(mouseWorld.x, mouseWorld.y);
            }
            else
            {
                explosionPosition = (Vector2)player.transform.position + player.AimDirection * range;
            }

            DealAoEDamage(player, explosionPosition);

            if (impactVFX != null)
            {
                Instantiate(impactVFX, explosionPosition, Quaternion.identity);
            }

            Debug.Log($"AoE explosion at {explosionPosition}");
        }

        private void DealAoEDamage(PlayerController player, Vector2 center)
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(center, explosionRadius, targetLayers);

            foreach (var hit in hits)
            {
                IDamageable damageable = hit.GetComponent<IDamageable>();
                if (damageable != null && damageable.IsAlive)
                {
                    float distance = Vector2.Distance(center, hit.transform.position);
                    float damageMultiplier = 1f - (distance / explosionRadius);
                    float finalDamage = damage * Mathf.Clamp01(damageMultiplier);

                    Vector2 knockbackDir = (hit.transform.position - (Vector3)center).normalized;
                    
                    DamagePacket packet = new DamagePacket(finalDamage, player.gameObject, center)
                    {
                        knockbackDirection = knockbackDir,
                        knockbackForce = 4f,
                        type = DamageType.Magic
                    };

                    damageable.TakeDamage(packet);
                }
            }

            Debug.Log($"AoE hit {hits.Length} enemies");
        }

        public override bool CanExecute(PlayerController player)
        {
            return true;
        }
    }
}
