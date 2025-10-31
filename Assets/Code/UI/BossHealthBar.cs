using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ForestSlice.Core;

namespace ForestSlice.UI
{
    /// <summary>
    /// Boss health bar UI with phase indicator
    /// </summary>
    public class BossHealthBar : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private GameObject bossBarPanel;
        [SerializeField] private Image healthFillBar;
        [SerializeField] private Image phase2FillBar; // Shows remaining health in phase 2
        [SerializeField] private TextMeshProUGUI bossNameText;
        [SerializeField] private TextMeshProUGUI phaseText;

        [Header("Settings")]
        [SerializeField] private Color phase1Color = Color.red;
        [SerializeField] private Color phase2Color = new Color(0.5f, 0f, 0.8f); // Purple
        [SerializeField] private float smoothSpeed = 5f;

        private Health bossHealth;
        private Boss.BossController bossController;
        private float targetFillAmount = 1f;
        private float currentFillAmount = 1f;

        private void Awake()
        {
            // Hide by default
            if (bossBarPanel != null)
            {
                bossBarPanel.SetActive(false);
            }
        }

        /// <summary>
        /// Initialize boss health bar for a specific boss
        /// </summary>
        public void Initialize(Boss.BossController boss)
        {
            bossController = boss;
            bossHealth = boss.Health;

            if (bossHealth == null)
            {
                Debug.LogError("Boss has no Health component!");
                return;
            }

            // Set boss name
            if (bossNameText != null)
            {
                bossNameText.text = boss.BossName;
            }

            // Subscribe to health changes
            bossHealth.OnHealthChanged.AddListener(OnHealthChanged);
            bossHealth.OnDeath.AddListener(OnBossDeath);

            // Subscribe to phase changes
            boss.OnPhase2Start.AddListener(OnPhase2Started);

            // Reset fill
            targetFillAmount = 1f;
            currentFillAmount = 1f;
            UpdateFillBar();

            // Show phase 1
            UpdatePhaseUI(Boss.BossPhase.Phase1);

            // Show bar
            Show();
        }

        private void Update()
        {
            // Smooth fill bar animation
            if (Mathf.Abs(currentFillAmount - targetFillAmount) > 0.001f)
            {
                currentFillAmount = Mathf.Lerp(currentFillAmount, targetFillAmount, Time.deltaTime * smoothSpeed);
                UpdateFillBar();
            }
        }

        private void OnHealthChanged(float currentHealth)
        {
            if (bossHealth == null) return;

            targetFillAmount = bossHealth.HealthPercent;
        }

        private void OnPhase2Started()
        {
            UpdatePhaseUI(Boss.BossPhase.Phase2);
        }

        private void OnBossDeath()
        {
            // Hide after delay
            Invoke(nameof(Hide), 2f);
        }

        private void UpdateFillBar()
        {
            if (healthFillBar != null)
            {
                healthFillBar.fillAmount = currentFillAmount;
            }

            if (phase2FillBar != null)
            {
                phase2FillBar.fillAmount = currentFillAmount;
            }
        }

        private void UpdatePhaseUI(Boss.BossPhase phase)
        {
            switch (phase)
            {
                case Boss.BossPhase.Phase1:
                    if (phaseText != null) phaseText.text = "PHASE 1";
                    if (healthFillBar != null) healthFillBar.color = phase1Color;
                    if (phase2FillBar != null) phase2FillBar.gameObject.SetActive(false);
                    break;

                case Boss.BossPhase.Phase2:
                    if (phaseText != null) phaseText.text = "PHASE 2";
                    if (healthFillBar != null) healthFillBar.color = phase2Color;
                    if (phase2FillBar != null) 
                    {
                        phase2FillBar.color = phase2Color;
                        phase2FillBar.gameObject.SetActive(true);
                    }
                    break;
            }
        }

        public void Show()
        {
            if (bossBarPanel != null)
            {
                bossBarPanel.SetActive(true);
            }
        }

        public void Hide()
        {
            if (bossBarPanel != null)
            {
                bossBarPanel.SetActive(false);
            }
        }

        private void OnDestroy()
        {
            // Unsubscribe from events
            if (bossHealth != null)
            {
                bossHealth.OnHealthChanged.RemoveListener(OnHealthChanged);
                bossHealth.OnDeath.RemoveListener(OnBossDeath);
            }

            if (bossController != null)
            {
                bossController.OnPhase2Start.RemoveListener(OnPhase2Started);
            }
        }
    }
}
