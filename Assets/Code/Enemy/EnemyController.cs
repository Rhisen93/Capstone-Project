using UnityEngine;
using ForestSlice.Core;
using ForestSlice.Items;

namespace ForestSlice.Enemy
{
    /// <summary>
    /// Base controller for all enemy types. Handles AI, movement, detection, and attacks.
    /// Override PerformAttack() in derived classes for specific enemy behaviors.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Health))]
    public abstract class EnemyController : MonoBehaviour
    {
        [Header("AI Settings")]
        [SerializeField] protected float detectionRange = 8f;
        [SerializeField] protected float attackRange = 1.5f;
        [SerializeField] protected float attackCooldown = 1.5f;
        [SerializeField] protected LayerMask playerLayer;

        [Header("Movement")]
        [SerializeField] protected float patrolSpeed = 2f;
        [SerializeField] protected float chaseSpeed = 4f;
        [SerializeField] protected Transform[] patrolPoints;

        [Header("Combat")]
        [SerializeField] protected float attackDamage = 15f;
        [SerializeField] protected float knockbackForce = 3f;

        [Header("Loot")]
        [SerializeField] protected LootTable lootTable;
        [SerializeField] protected GameObject pickupPrefab;

        // State Machine
        protected EnemyStateMachine stateMachine;
        public IdleState IdleState { get; protected set; }
        public PatrolState PatrolState { get; protected set; }
        public ChaseState ChaseState { get; protected set; }
        public AttackState AttackState { get; protected set; }
        public DeathState DeathState { get; protected set; }

        // Components
        protected Rigidbody2D rb;
        protected Health health;
        protected Collider2D col;

        // References
        protected Transform playerTransform;
        protected int currentPatrolIndex = 0;
        protected float currentSpeed;

        // Properties
        public float PatrolSpeed => patrolSpeed;
        public float ChaseSpeed => chaseSpeed;
        public float AttackCooldown => attackCooldown;
        public Transform PlayerTransform => playerTransform;

        protected virtual void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            health = GetComponent<Health>();
            col = GetComponent<Collider2D>();

            // Initialize state machine
            stateMachine = new EnemyStateMachine(this);

            // Create all states
            IdleState = new IdleState(this, stateMachine);
            PatrolState = new PatrolState(this, stateMachine);
            ChaseState = new ChaseState(this, stateMachine);
            AttackState = new AttackState(this, stateMachine);
            DeathState = new DeathState(this, stateMachine);
        }

        protected virtual void Start()
        {
            // Find player
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerTransform = player.transform;
            }
            else
            {
                Debug.LogWarning($"{name}: Player not found! Make sure Player has 'Player' tag.");
            }

            // Subscribe to death event
            health.OnDeath.AddListener(OnDeath);

            // Start in Idle state
            stateMachine.Initialize(IdleState);
            currentSpeed = patrolSpeed;
        }

        protected virtual void Update()
        {
            stateMachine.Update();
        }

        protected virtual void FixedUpdate()
        {
            stateMachine.FixedUpdate();
        }

        #region Detection
        /// <summary>
        /// Check if player is within detection range and alive
        /// </summary>
        public bool CanSeePlayer()
        {
            if (playerTransform == null) return false;

            float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
            
            // Check if player is within detection range
            if (distanceToPlayer <= detectionRange)
            {
                // Check if player is alive
                Health playerHealth = playerTransform.GetComponent<Health>();
                if (playerHealth != null && playerHealth.IsAlive)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Check if player is within attack range
        /// </summary>
        public bool IsInAttackRange()
        {
            if (playerTransform == null) return false;
            
            float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
            return distanceToPlayer <= attackRange;
        }
        #endregion

        #region Movement
        /// <summary>
        /// Move towards a target position
        /// </summary>
        public void MoveTowards(Vector2 targetPosition)
        {
            Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;
            rb.velocity = direction * currentSpeed;

            // Face movement direction
            FaceDirection(direction);
        }

        /// <summary>
        /// Move towards the player
        /// </summary>
        public void MoveTowardsPlayer()
        {
            if (playerTransform != null)
            {
                MoveTowards(playerTransform.position);
            }
        }

        /// <summary>
        /// Stop all movement
        /// </summary>
        public void StopMovement()
        {
            rb.velocity = Vector2.zero;
        }

        /// <summary>
        /// Set current movement speed
        /// </summary>
        public void SetSpeed(float speed)
        {
            currentSpeed = speed;
        }

        /// <summary>
        /// Face a specific direction
        /// </summary>
        public void FaceDirection(Vector2 direction)
        {
            if (direction.x != 0)
            {
                transform.localScale = new Vector3(direction.x > 0 ? 1 : -1, 1, 1);
            }
        }

        /// <summary>
        /// Face a specific target position
        /// </summary>
        public void FaceTarget(Vector2 targetPosition)
        {
            Vector2 direction = targetPosition - (Vector2)transform.position;
            FaceDirection(direction);
        }
        #endregion

        #region Patrol
        /// <summary>
        /// Check if this enemy has patrol points
        /// </summary>
        public bool HasPatrolPoints()
        {
            return patrolPoints != null && patrolPoints.Length > 0;
        }

        /// <summary>
        /// Get the current patrol point
        /// </summary>
        public Vector2 GetCurrentPatrolPoint()
        {
            if (!HasPatrolPoints()) return transform.position;
            return patrolPoints[currentPatrolIndex].position;
        }

        /// <summary>
        /// Move to next patrol point
        /// </summary>
        public void NextPatrolPoint()
        {
            if (!HasPatrolPoints()) return;
            
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
        }
        #endregion

        #region Combat
        /// <summary>
        /// Perform attack - Override in derived classes for specific attack behavior
        /// </summary>
        public abstract void PerformAttack();

        /// <summary>
        /// Called when enemy dies
        /// </summary>
        protected virtual void OnDeath()
        {
            Debug.Log($"{name} died!");
            
            // Drop loot
            if (lootTable != null)
            {
                lootTable.SpawnLoot(transform.position, pickupPrefab);
            }
            
            stateMachine.ChangeState(DeathState);
        }

        /// <summary>
        /// Disable collision (used in death state)
        /// </summary>
        public void DisableCollision()
        {
            if (col != null)
            {
                col.enabled = false;
            }
        }
        #endregion

        #region Debug
        protected virtual void OnDrawGizmosSelected()
        {
            // Detection range
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, detectionRange);

            // Attack range
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);

            // Patrol points
            if (patrolPoints != null && patrolPoints.Length > 0)
            {
                Gizmos.color = Color.blue;
                for (int i = 0; i < patrolPoints.Length; i++)
                {
                    if (patrolPoints[i] != null)
                    {
                        Gizmos.DrawSphere(patrolPoints[i].position, 0.3f);
                        
                        // Draw line to next patrol point
                        int nextIndex = (i + 1) % patrolPoints.Length;
                        if (patrolPoints[nextIndex] != null)
                        {
                            Gizmos.DrawLine(patrolPoints[i].position, patrolPoints[nextIndex].position);
                        }
                    }
                }
            }
        }
        #endregion
    }
}
