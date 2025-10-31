using UnityEngine;
using ForestSlice.Core;

namespace ForestSlice.Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerMovement : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float acceleration = 50f;
        [SerializeField] private float deceleration = 50f;

        [Header("Dash")]
        [SerializeField] private float dashSpeed = 15f;
        [SerializeField] private float dashDuration = 0.2f;
        [SerializeField] private float dashCooldown = 1f;
        [SerializeField] private float iFramesDuration = 0.3f;

        private Rigidbody2D rb;
        private Health health;
        private Vector2 currentVelocity;
        
        private bool isDashing = false;
        private float dashTimer = 0f;
        private float dashCooldownTimer = 0f;
        private Vector2 dashDirection;

        private bool isInvulnerable = false;
        private float iFramesTimer = 0f;

        public bool IsDashing => isDashing;
        public bool CanDash => dashCooldownTimer <= 0f && !isDashing;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            health = GetComponent<Health>();
        }

        private void Update()
        {
            UpdateDashTimer();
            UpdateCooldownTimer();
            UpdateIFrames();
        }

        public void Move(Vector2 input)
        {
            if (isDashing) return;

            Vector2 targetVelocity = input * moveSpeed;
            float accel = input.magnitude > 0.1f ? acceleration : deceleration;
            
            currentVelocity = Vector2.MoveTowards(currentVelocity, targetVelocity, accel * Time.fixedDeltaTime);
            rb.velocity = currentVelocity;
        }

        public void Dash()
        {
            if (!CanDash) return;

            if (currentVelocity.magnitude < 0.1f)
            {
                dashDirection = Vector2.right;
            }
            else
            {
                dashDirection = currentVelocity.normalized;
            }

            isDashing = true;
            dashTimer = dashDuration;
            dashCooldownTimer = dashCooldown;

            ActivateIFrames();
        }

        private void UpdateDashTimer()
        {
            if (!isDashing) return;

            dashTimer -= Time.deltaTime;

            if (dashTimer > 0f)
            {
                rb.velocity = dashDirection * dashSpeed;
            }
            else
            {
                isDashing = false;
                rb.velocity = currentVelocity;
            }
        }

        private void UpdateCooldownTimer()
        {
            if (dashCooldownTimer > 0f)
            {
                dashCooldownTimer -= Time.deltaTime;
            }
        }

        private void ActivateIFrames()
        {
            if (health == null) return;

            isInvulnerable = true;
            iFramesTimer = iFramesDuration;
            health.SetInvulnerable(true);
        }

        private void UpdateIFrames()
        {
            if (!isInvulnerable) return;

            iFramesTimer -= Time.deltaTime;

            if (iFramesTimer <= 0f)
            {
                isInvulnerable = false;
                if (health != null)
                {
                    health.SetInvulnerable(false);
                }
            }
        }

        public void Stop()
        {
            currentVelocity = Vector2.zero;
            rb.velocity = Vector2.zero;
        }
    }
}
