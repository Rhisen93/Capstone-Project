using UnityEngine;
using ForestSlice.UI;

namespace ForestSlice.Boss
{
    /// <summary>
    /// Boss arena trigger - starts boss encounter when player enters
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class BossArena : MonoBehaviour
    {
        [Header("Boss References")]
        [SerializeField] private BossController boss;
        [SerializeField] private BossHealthBar bossHealthBar;

        [Header("Arena Settings")]
        [SerializeField] private bool lockPlayerInArena = true;
        [SerializeField] private GameObject[] arenaWalls; // Activate on fight start
        [SerializeField] private GameObject[] arenaEffects; // Visual effects

        [Header("Audio")]
        [SerializeField] private AudioClip bossMusicClip;

        private bool encounterStarted = false;
        private Collider2D arenaTrigger;

        private void Awake()
        {
            arenaTrigger = GetComponent<Collider2D>();
            arenaTrigger.isTrigger = true;

            // Deactivate walls initially
            if (arenaWalls != null)
            {
                foreach (var wall in arenaWalls)
                {
                    if (wall != null) wall.SetActive(false);
                }
            }

            // Deactivate effects initially
            if (arenaEffects != null)
            {
                foreach (var effect in arenaEffects)
                {
                    if (effect != null) effect.SetActive(false);
                }
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (encounterStarted) return;

            if (other.CompareTag("Player"))
            {
                StartBossEncounter();
            }
        }

        private void StartBossEncounter()
        {
            if (boss == null)
            {
                Debug.LogError("Boss reference not set in BossArena!");
                return;
            }

            encounterStarted = true;

            Debug.Log($"Boss encounter started: {boss.BossName}");

            // Lock arena
            if (lockPlayerInArena && arenaWalls != null)
            {
                foreach (var wall in arenaWalls)
                {
                    if (wall != null) wall.SetActive(true);
                }
            }

            // Activate effects
            if (arenaEffects != null)
            {
                foreach (var effect in arenaEffects)
                {
                    if (effect != null) effect.SetActive(true);
                }
            }

            // Start boss encounter
            boss.StartEncounter();

            // Initialize health bar
            if (bossHealthBar != null)
            {
                bossHealthBar.Initialize(boss);
            }
            else
            {
                Debug.LogWarning("BossHealthBar reference not set!");
            }

            // Play boss music
            if (bossMusicClip != null)
            {
                // TODO: Integrate with AudioManager when available
                Debug.Log($"Playing boss music: {bossMusicClip.name}");
            }

            // Subscribe to boss defeat
            boss.OnBossDefeated.AddListener(OnBossDefeated);

            // Disable trigger
            arenaTrigger.enabled = false;
        }

        private void OnBossDefeated()
        {
            Debug.Log($"Boss defeated: {boss.BossName}");

            // Unlock arena after delay
            if (lockPlayerInArena)
            {
                Invoke(nameof(UnlockArena), 3f);
            }
        }

        private void UnlockArena()
        {
            // Deactivate walls
            if (arenaWalls != null)
            {
                foreach (var wall in arenaWalls)
                {
                    if (wall != null) wall.SetActive(false);
                }
            }

            // Deactivate effects
            if (arenaEffects != null)
            {
                foreach (var effect in arenaEffects)
                {
                    if (effect != null) effect.SetActive(false);
                }
            }

            Debug.Log("Arena unlocked!");
        }

        private void OnDrawGizmos()
        {
            // Draw arena trigger bounds
            Gizmos.color = new Color(1f, 0.5f, 0f, 0.3f);
            var col = GetComponent<Collider2D>();
            if (col != null && col is BoxCollider2D)
            {
                var boxCol = col as BoxCollider2D;
                Gizmos.DrawCube(transform.position + (Vector3)boxCol.offset, boxCol.size);
            }
        }

        private void OnDestroy()
        {
            if (boss != null)
            {
                boss.OnBossDefeated.RemoveListener(OnBossDefeated);
            }
        }
    }
}
