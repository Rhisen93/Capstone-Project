using System.Collections;
using UnityEngine;
using TMPro;

namespace ForestSlice.Narrative
{
    [RequireComponent(typeof(CanvasGroup))]
    public class NarrativeUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI narrativeText;
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private float fadeSpeed = 2f;
        
        private CanvasGroup canvasGroup;
        private Coroutine currentDisplay;
        private bool canSkip = true;

        private static NarrativeUI instance;
        public static NarrativeUI Instance => instance;

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
            canvasGroup = GetComponent<CanvasGroup>();
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }

        private void Update()
        {
            if (currentDisplay != null && canSkip && Input.anyKeyDown)
            {
                StopCoroutine(currentDisplay);
                currentDisplay = null;
                StartCoroutine(FadeOut());
            }
        }

        public void ShowText(string text, float duration = 5f, string title = "")
        {
            ShowText(text, duration, true, title);
        }

        public void ShowText(string text, float duration, bool canSkip, string title = "")
        {
            if (currentDisplay != null)
            {
                StopCoroutine(currentDisplay);
            }

            this.canSkip = canSkip;
            currentDisplay = StartCoroutine(DisplayTextRoutine(text, duration, title));
        }

        public void ShowEntry(NarrativeEntryData entry)
        {
            if (entry == null) return;
            ShowText(entry.text, entry.displayDuration, entry.canSkip, entry.title);
        }

        private IEnumerator DisplayTextRoutine(string text, float duration, string title)
        {
            narrativeText.text = text;
            titleText.text = title;

            yield return FadeIn();

            yield return new WaitForSeconds(duration);

            yield return FadeOut();

            currentDisplay = null;
        }

        private IEnumerator FadeIn()
        {
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;

            while (canvasGroup.alpha < 1f)
            {
                canvasGroup.alpha += Time.deltaTime * fadeSpeed;
                yield return null;
            }

            canvasGroup.alpha = 1f;
        }

        private IEnumerator FadeOut()
        {
            while (canvasGroup.alpha > 0f)
            {
                canvasGroup.alpha -= Time.deltaTime * fadeSpeed;
                yield return null;
            }

            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }

        public void Hide()
        {
            if (currentDisplay != null)
            {
                StopCoroutine(currentDisplay);
                currentDisplay = null;
            }
            StartCoroutine(FadeOut());
        }
    }
}
