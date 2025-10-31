using UnityEngine;
using UnityEngine.InputSystem;
using ForestSlice.Core;
using ForestSlice.Player.Abilities;

namespace ForestSlice.Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Health))]
    public class PlayerController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform aimPivot;
        
        private Rigidbody2D rb;
        private Health health;
        private PlayerMovement movement;
        private PlayerCombat combat;
        private AbilityManager abilityManager;
        
        private Vector2 moveInput;
        private Vector2 aimDirection;
        private bool isDead = false;

        public Vector2 AimDirection => aimDirection;
        public bool IsDead => isDead;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            health = GetComponent<Health>();
            movement = GetComponent<PlayerMovement>();
            combat = GetComponent<PlayerCombat>();
            abilityManager = GetComponent<AbilityManager>();

            if (aimPivot == null)
            {
                GameObject pivot = new GameObject("AimPivot");
                pivot.transform.SetParent(transform);
                pivot.transform.localPosition = Vector3.zero;
                aimPivot = pivot.transform;
            }
        }

        private void Start()
        {
            health.OnDeath.AddListener(HandleDeath);
        }

        private void Update()
        {
            if (isDead) return;

            UpdateAimDirection();
            UpdateAimPivot();
        }

        private void FixedUpdate()
        {
            if (isDead) return;

            if (movement != null)
            {
                movement.Move(moveInput);
            }
        }

        public void SetMoveInput(Vector2 input)
        {
            moveInput = input;
        }

        public void OnDashInput()
        {
            if (movement != null && !isDead)
            {
                movement.Dash();
            }
        }

        public void OnAttackInput()
        {
            if (combat != null && !isDead)
            {
                combat.Attack();
            }
        }

        public void OnAbilityInput(int abilityIndex)
        {
            if (isDead) return;
            
            if (abilityManager != null)
            {
                abilityManager.TryExecuteAbility(abilityIndex - 1);
            }
            else
            {
                Debug.Log($"Ability {abilityIndex} pressed (AbilityManager not found)");
            }
        }

        private void UpdateAimDirection()
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            mouseWorldPos.z = 0f;
            
            aimDirection = (mouseWorldPos - transform.position).normalized;
        }

        private void UpdateAimPivot()
        {
            if (aimPivot != null && aimDirection != Vector2.zero)
            {
                float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
                aimPivot.rotation = Quaternion.Euler(0f, 0f, angle);
            }
        }

        private void HandleDeath()
        {
            isDead = true;
            moveInput = Vector2.zero;
            rb.velocity = Vector2.zero;
            Debug.Log("Player died!");
        }

        public void Respawn(Vector3 position)
        {
            transform.position = position;
            isDead = false;
            health.ResetHealth();
            rb.velocity = Vector2.zero;
        }

        private void OnDestroy()
        {
            if (health != null)
            {
                health.OnDeath.RemoveListener(HandleDeath);
            }
        }
    }
}
