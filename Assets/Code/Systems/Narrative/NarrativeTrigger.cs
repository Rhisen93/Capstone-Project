using UnityEngine;

namespace ForestSlice.Narrative
{
    public class NarrativeTrigger : MonoBehaviour
    {
        [SerializeField] private string narrativeId;
        [SerializeField] private NarrativeTriggerType triggerType = NarrativeTriggerType.OnEnter;
        [SerializeField] private bool autoOnStart = false;
        [SerializeField] private bool triggerOnce = true;
        [SerializeField] private string playerTag = "Player";

        private bool hasTriggered = false;

        private void Start()
        {
            if (autoOnStart && triggerType == NarrativeTriggerType.OnStart)
            {
                TriggerNarrative();
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (triggerType != NarrativeTriggerType.OnEnter) return;
            if (hasTriggered && triggerOnce) return;
            if (!collision.CompareTag(playerTag)) return;

            TriggerNarrative();
        }

        public void TriggerNarrative()
        {
            if (hasTriggered && triggerOnce) return;

            NarrativeEntryData entry = NarrativeJsonLoader.GetEntry(narrativeId);
            
            if (entry != null && NarrativeUI.Instance != null)
            {
                NarrativeUI.Instance.ShowEntry(entry);
                hasTriggered = true;
            }
            else
            {
                Debug.LogWarning($"Cannot trigger narrative: {narrativeId}. Entry or UI missing.");
            }
        }

        public void ResetTrigger()
        {
            hasTriggered = false;
        }

        private void OnDrawGizmos()
        {
            if (triggerType == NarrativeTriggerType.OnEnter)
            {
                Gizmos.color = new Color(1f, 0.8f, 0f, 0.3f);
                Gizmos.DrawWireCube(transform.position, Vector3.one);
            }
        }
    }
}
