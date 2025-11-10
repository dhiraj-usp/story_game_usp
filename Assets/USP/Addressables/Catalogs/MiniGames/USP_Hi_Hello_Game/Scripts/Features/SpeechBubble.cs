using System;
using UnityEngine;
using System.Collections;

namespace USP.Minigame.DF_Game
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class SpeechBubbleImage : MonoBehaviour,IClickable
    {
        [Header("Bubble Sprites")] [SerializeField]
        private Sprite[] bubbleSprites; // Assign all your bubble images here

        [SerializeField] private float showDuration = 2f;
        [SerializeField] private Vector3 offset = new Vector3(0, 2f, 0);
        [SerializeField] private bool faceCamera = true;
        [SerializeField] private Camera gamecamera;

        private SpriteRenderer bubbleRenderer;
        private Transform target;
        private Coroutine activeRoutine;
        private Vector3 originalScale;
        private Action Onclickcallback;

        private void Awake()
        {
            bubbleRenderer = GetComponent<SpriteRenderer>();
            originalScale = transform.localScale; // store the original size
            bubbleRenderer.enabled = false;
            transform.localScale = Vector3.zero;
        }



        public void AttachTo(Transform targetTransform)
        {
            target = targetTransform;
        }

        [ContextMenu("Show Bubble Image")]
        public void Test1()
        {
            ShowBubble(1);
        }

        /// <summary>
        /// Show a specific speech bubble image by index.
        /// </summary>
        public void ShowBubble(int index, float duration = -1f, bool shouldpopout = true)
        {
            if (index < 0 || index >= bubbleSprites.Length)
            {
                Debug.LogWarning("Invalid bubble index");
                return;
            }

            if (activeRoutine != null)
                StopCoroutine(activeRoutine);

            activeRoutine = StartCoroutine(ShowRoutine(bubbleSprites[index], duration > 0 ? duration : showDuration,
                shouldpopout));
        }

        private IEnumerator ShowRoutine(Sprite sprite, float duration, bool shouldpopout)
        {
            bubbleRenderer.sprite = sprite;
            bubbleRenderer.enabled = true;

            // Pop in to original scale
            float t = 0f;
            while (t < 0.2f)
            {
                t += Time.deltaTime;
                transform.localScale = Vector3.Lerp(Vector3.zero, originalScale, t / 0.2f);
                yield return null;
            }

            yield return new WaitForSeconds(duration);

            if (!shouldpopout)
                yield break;
            // Pop out back to zero
            t = 0f;
            while (t < 0.2f)
            {
                t += Time.deltaTime;
                transform.localScale = Vector3.Lerp(originalScale, Vector3.zero, t / 0.2f);
                yield return null;
            }

            bubbleRenderer.enabled = false;
        }

        public IEnumerator PopOutBubble()
        {
            // Pop out back to zero
            float t = 0f;
            while (t < 0.2f)
            {
                t += Time.deltaTime;
                transform.localScale = Vector3.Lerp(originalScale, Vector3.zero, t / 0.2f);
                yield return null;
            }

            bubbleRenderer.enabled = false;
        }

        public void EnableClick(Action callback)
        {
            Onclickcallback = callback;
        }

        public void DisableClick()
        {
            Onclickcallback = null;
        }

        public void OnClicked()
        {
            Onclickcallback?.Invoke();
        }
    }
}