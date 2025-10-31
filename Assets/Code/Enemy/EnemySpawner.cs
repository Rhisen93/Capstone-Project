using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace ForestSlice.Enemy
{
    /// <summary>
    /// Spawns waves of enemies at specified spawn points
    /// Can be configured for timed waves or manual triggering
    /// </summary>
    public class EnemySpawner : MonoBehaviour
    {
        [System.Serializable]
        public class SpawnWave
        {
            public string waveName = "Wave 1";
            public EnemySpawnData[] enemies;
            public float delayBeforeWave = 0f;
            public float delayAfterWave = 5f;
        }

        [System.Serializable]
        public class EnemySpawnData
        {
            public GameObject enemyPrefab;
            public int count = 1;
            public float spawnDelay = 0.5f; // Delay between each spawn in this group
        }

        [Header("Spawn Configuration")]
        [SerializeField] private Transform[] spawnPoints;
        [SerializeField] private SpawnWave[] waves;
        [SerializeField] private bool autoStart = true;
        [SerializeField] private bool loopWaves = false;

        [Header("Spawn Settings")]
        [SerializeField] private float spawnRadius = 1f; // Random offset from spawn point
        [SerializeField] private bool randomizeSpawnPoints = true;

        private int currentWaveIndex = 0;
        private bool isSpawning = false;
        private List<GameObject> activeEnemies = new List<GameObject>();

        public int CurrentWave => currentWaveIndex;
        public int TotalWaves => waves.Length;
        public int ActiveEnemyCount => activeEnemies.Count;
        public bool IsSpawning => isSpawning;

        private void Start()
        {
            if (spawnPoints == null || spawnPoints.Length == 0)
            {
                Debug.LogWarning($"{name}: No spawn points assigned! Using spawner position.");
                spawnPoints = new Transform[] { transform };
            }

            if (autoStart && waves.Length > 0)
            {
                StartSpawning();
            }
        }

        /// <summary>
        /// Start spawning waves
        /// </summary>
        public void StartSpawning()
        {
            if (!isSpawning && waves.Length > 0)
            {
                StartCoroutine(SpawnWavesRoutine());
            }
        }

        /// <summary>
        /// Spawn a specific wave by index
        /// </summary>
        public void SpawnWaveByIndex(int waveIndex)
        {
            if (waveIndex >= 0 && waveIndex < waves.Length)
            {
                StartCoroutine(SpawnSingleWave(waves[waveIndex]));
            }
        }

        /// <summary>
        /// Stop all spawning
        /// </summary>
        public void StopSpawning()
        {
            StopAllCoroutines();
            isSpawning = false;
        }

        /// <summary>
        /// Clear all active enemies
        /// </summary>
        public void ClearAllEnemies()
        {
            foreach (GameObject enemy in activeEnemies)
            {
                if (enemy != null)
                {
                    Destroy(enemy);
                }
            }
            activeEnemies.Clear();
        }

        private IEnumerator SpawnWavesRoutine()
        {
            isSpawning = true;

            do
            {
                for (int i = 0; i < waves.Length; i++)
                {
                    currentWaveIndex = i;
                    SpawnWave wave = waves[i];

                    Debug.Log($"Starting {wave.waveName}");

                    // Delay before wave
                    if (wave.delayBeforeWave > 0f)
                    {
                        yield return new WaitForSeconds(wave.delayBeforeWave);
                    }

                    // Spawn all enemies in this wave
                    yield return StartCoroutine(SpawnSingleWave(wave));

                    // Delay after wave
                    if (wave.delayAfterWave > 0f)
                    {
                        yield return new WaitForSeconds(wave.delayAfterWave);
                    }

                    Debug.Log($"{wave.waveName} complete!");
                }

                currentWaveIndex = 0;

            } while (loopWaves);

            isSpawning = false;
            Debug.Log("All waves complete!");
        }

        private IEnumerator SpawnSingleWave(SpawnWave wave)
        {
            foreach (EnemySpawnData spawnData in wave.enemies)
            {
                if (spawnData.enemyPrefab == null)
                {
                    Debug.LogWarning($"Enemy prefab not assigned in {wave.waveName}");
                    continue;
                }

                // Spawn each enemy in this group
                for (int i = 0; i < spawnData.count; i++)
                {
                    SpawnEnemy(spawnData.enemyPrefab);

                    // Delay between spawns
                    if (spawnData.spawnDelay > 0f && i < spawnData.count - 1)
                    {
                        yield return new WaitForSeconds(spawnData.spawnDelay);
                    }
                }
            }
        }

        private void SpawnEnemy(GameObject enemyPrefab)
        {
            // Choose spawn point
            Transform spawnPoint = randomizeSpawnPoints 
                ? spawnPoints[Random.Range(0, spawnPoints.Length)]
                : spawnPoints[currentWaveIndex % spawnPoints.Length];

            // Calculate spawn position with random offset
            Vector2 offset = Random.insideUnitCircle * spawnRadius;
            Vector3 spawnPosition = spawnPoint.position + new Vector3(offset.x, offset.y, 0f);

            // Spawn enemy
            GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
            enemy.transform.SetParent(transform); // Organize in hierarchy

            activeEnemies.Add(enemy);

            // Subscribe to death event to track active enemies
            EnemyController controller = enemy.GetComponent<EnemyController>();
            if (controller != null)
            {
                var health = controller.GetComponent<ForestSlice.Core.Health>();
                if (health != null)
                {
                    health.OnDeath.AddListener(() => OnEnemyDeath(enemy));
                }
            }

            Debug.Log($"Spawned {enemyPrefab.name} at {spawnPosition}");
        }

        private void OnEnemyDeath(GameObject enemy)
        {
            activeEnemies.Remove(enemy);
            Debug.Log($"Enemy died. Active enemies: {activeEnemies.Count}");
        }

        private void OnDrawGizmos()
        {
            if (spawnPoints == null) return;

            // Draw spawn points
            Gizmos.color = Color.green;
            foreach (Transform point in spawnPoints)
            {
                if (point != null)
                {
                    Gizmos.DrawWireSphere(point.position, spawnRadius);
                    Gizmos.DrawSphere(point.position, 0.3f);
                }
            }
        }
    }
}
