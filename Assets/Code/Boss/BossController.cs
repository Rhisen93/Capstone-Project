using UnityEngine;
using UnityEngine.Events;
using ForestSlice.Core;
using ForestSlice.Narrative;

namespace ForestSlice.Boss
{
    /// <summary>
    /// Base controller for all boss encounters with multi-phase system
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Health))]
    public abstract class BossController : MonoBehaviour
    {
        [Header("Boss Info")]
        [SerializeField] protected string bossName = "Boss";
        [SerializeField] protected string bossId = "boss_01";

        [Header("Phase Settings")]
        [SerializeField] protected float phase2HealthThreshold = 0.5f; // 50% HP triggers phase 2
        [SerializeField] protected float enrageHealthThreshold = 0.25f; // 25% HP enrage mode

        [Header("Movement")]
        [SerializeField] protected float moveSpeed = 3f;
        [SerializeField] protected float dashSpeed = 10f;

        [Header("Combat")]
        [SerializeField] protected float attackDamage = 20f;
        [SerializeField] protected float knockbackForce = 5f;
        [SerializeField] protected LayerMask playerLayer;

        [Header("Arena")]
        [SerializeField] protected Transform arenaCenter;
        [SerializeField] protected float arenaRadius = 15f;

        [Header("Narrative")]
        [SerializeField] protected string introDialogueId;
        [SerializeField] protected string phase2DialogueId;
        [SerializeField] protected string defeatDialogueId;

        // Events
        public UnityEvent OnBossStart = new UnityEvent();
        public UnityEvent OnPhase2Start = new UnityEvent();
        public UnityEvent OnBossDefeated = new UnityEvent();

        // State Machine
        protected BossStateMachine stateMachine;
        public BossState IntroState { get; protected set; }
        public BossState Phase1State { get; protected set; }
        public BossState Phase2State { get; protected set; }
        public BossState DeathState { get; protected set; }

        // Components
        protected Rigidbody2D rb;
        protected Health health;
        protected Collider2D col;

        // References
        protected Transform playerTransform;
        protected BossPhase currentPhase = BossPhase.Inactive;
        protected bool isEnraged = false;

        // Properties
        public BossPhase CurrentPhase => currentPhase;
        public bool IsEnraged => isEnraged;
        public string BossName => bossName;
        public string BossId => bossId;
        public Transform PlayerTransform => playerTransform;
        public Health Health => health;

        protected virtual void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            health = GetComponent<Health>();
            col = GetComponent<Collider2D>();

            // Initialize state machine
            stateMachine = new BossStateMachine(this);

            // Create states (to be overridden in derived classes)
            InitializeStates();
        }

        protected virtual void Start()
        {
            // Find player
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerTransform = player.transform;
            }

            // Subscribe to health events
            health.OnHealthChanged.AddListener(OnHealthChanged);
            health.OnDeath.AddListener(OnDeath);

            // Start in intro state (triggered by arena)
            currentPhase = BossPhase.Inactive;
        }

        protected virtual void Update()
        {
            stateMachine.Update();
        }

        protected virtual void FixedUpdate()
        {
            stateMachine.FixedUpdate();
        }

        /// <summary>
        /// Initialize boss states (override in derived classes)
        /// </summary>
        protected abstract void InitializeStates();

        /// <summary>
        /// Start boss encounter
        /// </summary>
        public virtual void StartEncounter()
        {
            if (currentPhase != BossPhase.Inactive) return;

            Debug.Log($"{bossName} encounter started!");
            currentPhase = BossPhase.Intro;

            // Show intro dialogue
            if (!string.IsNullOrEmpty(introDialogueId))
            {
                var narrativeUI = FindObjectOfType<NarrativeUI>();
                var entry = NarrativeJsonLoader.GetEntry(introDialogueId);
                if (entry != null)
                {
                    narrativeUI?.ShowEntry(entry);
                }
            }

            stateMachine.Initialize(IntroState);
            OnBossStart?.Invoke();
        }

        /// <summary>
        /// Transition to Phase 1
        /// </summary>
        public virtual void StartPhase1()
        {
            currentPhase = BossPhase.Phase1;
            stateMachine.ChangeState(Phase1State);
            Debug.Log($"{bossName} entered Phase 1");
        }

        /// <summary>
        /// Transition to Phase 2
        /// </summary>
        public virtual void StartPhase2()
        {
            if (currentPhase == BossPhase.Phase2) return;

            currentPhase = BossPhase.Phase2;
            stateMachine.ChangeState(Phase2State);
            
            // Show phase 2 dialogue
            if (!string.IsNullOrEmpty(phase2DialogueId))
            {
                var narrativeUI = FindObjectOfType<NarrativeUI>();
                var entry = NarrativeJsonLoader.GetEntry(phase2DialogueId);
                if (entry != null)
                {
                    narrativeUI?.ShowEntry(entry);
                }
            }

            OnPhase2Start?.Invoke();
            Debug.Log($"{bossName} entered Phase 2!");
        }

        /// <summary>
        /// Called when boss health changes
        /// </summary>
        protected virtual void OnHealthChanged(float currentHealth)
        {
            float healthPercent = health.HealthPercent;

            // Check for phase transition
            if (currentPhase == BossPhase.Phase1 && healthPercent <= phase2HealthThreshold)
            {
                StartPhase2();
            }

            // Check for enrage
            if (!isEnraged && healthPercent <= enrageHealthThreshold)
            {
                isEnraged = true;
                OnEnrage();
            }
        }

        /// <summary>
        /// Called when boss enters enrage mode
        /// </summary>
        protected virtual void OnEnrage()
        {
            Debug.Log($"{bossName} is enraged!");
            // Override in derived classes for enrage effects
        }

        /// <summary>
        /// Called when boss dies
        /// </summary>
        protected virtual void OnDeath()
        {
            currentPhase = BossPhase.Death;
            stateMachine.ChangeState(DeathState);

            // Show defeat dialogue
            if (!string.IsNullOrEmpty(defeatDialogueId))
            {
                var narrativeUI = FindObjectOfType<NarrativeUI>();
                var entry = NarrativeJsonLoader.GetEntry(defeatDialogueId);
                if (entry != null)
                {
                    narrativeUI?.ShowEntry(entry);
                }
            }

            OnBossDefeated?.Invoke();
            Debug.Log($"{bossName} defeated!");
        }

        #region Movement
        public void MoveTowards(Vector2 targetPosition)
        {
            Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;
            rb.velocity = direction * moveSpeed;
            FaceDirection(direction);
        }

        public void MoveTowardsPlayer()
        {
            if (playerTransform != null)
            {
                MoveTowards(playerTransform.position);
            }
        }

        public void StopMovement()
        {
            rb.velocity = Vector2.zero;
        }

        public void FaceDirection(Vector2 direction)
        {
            if (direction.x != 0)
            {
                transform.localScale = new Vector3(direction.x > 0 ? 1 : -1, 1, 1);
            }
        }

        public void FacePlayer()
        {
            if (playerTransform != null)
            {
                Vector2 direction = playerTransform.position - transform.position;
                FaceDirection(direction);
            }
        }
        #endregion

        #region Utilities
        public float GetDistanceToPlayer()
        {
            if (playerTransform == null) return float.MaxValue;
            return Vector2.Distance(transform.position, playerTransform.position);
        }

        public Vector2 GetDirectionToPlayer()
        {
            if (playerTransform == null) return Vector2.zero;
            return (playerTransform.position - transform.position).normalized;
        }

        public bool IsPlayerInRange(float range)
        {
            return GetDistanceToPlayer() <= range;
        }
        #endregion

        protected virtual void OnDrawGizmosSelected()
        {
            // Draw arena
            if (arenaCenter != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(arenaCenter.position, arenaRadius);
            }
            else
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(transform.position, arenaRadius);
            }
        }
    }
}
