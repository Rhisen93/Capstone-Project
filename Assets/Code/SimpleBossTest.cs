using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Simple boss HP tester - Press 8 for Phase2, 9 for Enrage, 0 for Kill
/// </summary>
public class SimpleBossTest : MonoBehaviour
{
    public ForestSlice.Core.Health bossHealth;

    void Update()
    {
        if (bossHealth == null) return;

        // Press 8 = 50% HP (Phase 2)
        if (Keyboard.current != null && Keyboard.current.digit8Key.wasPressedThisFrame)
        {
            float targetHP = bossHealth.MaxHealth * 0.5f;
            bossHealth.SetHealth(targetHP);
            UnityEngine.Debug.Log($"<color=yellow>Boss HP set to 50% ({targetHP}/{bossHealth.MaxHealth})</color>");
        }

        // Press 9 = 25% HP (Enrage)
        if (Keyboard.current != null && Keyboard.current.digit9Key.wasPressedThisFrame)
        {
            float targetHP = bossHealth.MaxHealth * 0.25f;
            bossHealth.SetHealth(targetHP);
            UnityEngine.Debug.Log($"<color=orange>Boss HP set to 25% ({targetHP}/{bossHealth.MaxHealth})</color>");
        }

        // Press 0 = 0% HP (Kill)
        if (Keyboard.current != null && Keyboard.current.digit0Key.wasPressedThisFrame)
        {
            UnityEngine.Debug.Log($"<color=red>Attempting to kill boss... Current HP: {bossHealth.CurrentHealth}, IsAlive: {bossHealth.IsAlive}</color>");
            bossHealth.SetHealth(0f);
            UnityEngine.Debug.Log($"<color=red>After SetHealth(0): HP = {bossHealth.CurrentHealth}, IsAlive: {bossHealth.IsAlive}</color>");
        }

        // Press 7 = Reset to 100%
        if (Keyboard.current != null && Keyboard.current.digit7Key.wasPressedThisFrame)
        {
            bossHealth.SetHealth(bossHealth.MaxHealth);
            UnityEngine.Debug.Log($"<color=green>Boss HP reset to 100% ({bossHealth.MaxHealth})</color>");
        }
    }
}
