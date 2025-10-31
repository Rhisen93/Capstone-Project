using UnityEngine;
using ForestSlice.Combat;

namespace ForestSlice.Player.Abilities
{
    [CreateAssetMenu(fileName = "ProjectileAbility", menuName = "ForestSlice/Abilities/Projectile")]
    public class ProjectileAbility : AbilityBase
    {
        [Header("Projectile Settings")]
        public GameObject projectilePrefab;
        public float projectileSpeed = 10f;
        public float projectileLifetime = 3f;
        public LayerMask targetLayers;

        public override void Execute(PlayerController player)
        {
            if (projectilePrefab == null)
            {
                Debug.LogError($"Projectile prefab not assigned for {abilityName}");
                return;
            }

            Vector2 spawnPosition = player.transform.position;
            Vector2 direction = player.AimDirection;

            GameObject projectileObj = Instantiate(projectilePrefab, spawnPosition, Quaternion.identity);
            Projectile projectile = projectileObj.GetComponent<Projectile>();

            if (projectile != null)
            {
                projectile.Initialize(player.gameObject, direction, damage, projectileSpeed, targetLayers);
            }

            if (castVFX != null)
            {
                Instantiate(castVFX, spawnPosition, Quaternion.identity);
            }

            Debug.Log($"Fired projectile: {abilityName}");
        }

        public override bool CanExecute(PlayerController player)
        {
            return projectilePrefab != null;
        }
    }
}
