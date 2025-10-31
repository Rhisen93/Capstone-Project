using UnityEngine;
using UnityEngine.Events;

namespace ForestSlice.Core
{
    public class Health : MonoBehaviour, IDamageable
    {
        [SerializeField] private float maxHealth = 100f;
        [SerializeField] private float currentHealth;
        [SerializeField] private bool invulnerable = false;

        public UnityEvent<float> OnHealthChanged;
        public UnityEvent OnDeath;
        public UnityEvent<DamagePacket> OnDamaged;

        public float MaxHealth => maxHealth;
        public float CurrentHealth => currentHealth;
        public float HealthPercent => currentHealth / maxHealth;
        public bool IsAlive => currentHealth > 0f;

        private Rigidbody2D rb;

        private void Awake()
        {
            currentHealth = maxHealth;
            rb = GetComponent<Rigidbody2D>();
        }

        public void TakeDamage(DamagePacket packet)
        {
            if (!IsAlive || invulnerable) return;

            currentHealth -= packet.amount;
            currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

            OnHealthChanged?.Invoke(currentHealth);
            OnDamaged?.Invoke(packet);

            if (rb != null && packet.knockbackForce > 0f)
            {
                rb.AddForce(packet.knockbackDirection * packet.knockbackForce, ForceMode2D.Impulse);
            }

            if (currentHealth <= 0f)
            {
                OnDeath?.Invoke();
            }
        }

        public void Heal(float amount)
        {
            if (!IsAlive) return;

            currentHealth += amount;
            currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
            OnHealthChanged?.Invoke(currentHealth);
        }

        public void SetHealth(float health)
        {
            bool wasAlive = IsAlive;
            currentHealth = Mathf.Clamp(health, 0f, maxHealth);
            OnHealthChanged?.Invoke(currentHealth);

            // Trigger death if health reaches 0 and was previously alive
            if (currentHealth <= 0f && wasAlive)
            {
                OnDeath?.Invoke();
            }
        }

        public void SetInvulnerable(bool state)
        {
            invulnerable = state;
        }

        public Transform GetTransform()
        {
            return transform;
        }

        public void ResetHealth()
        {
            currentHealth = maxHealth;
            OnHealthChanged?.Invoke(currentHealth);
        }
    }
}
