using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem; // ✅ For new Input System

namespace USP.Minigame.DF_Game
{
    public class TutorialPointer : MonoBehaviour
    {
        [Header("Pointer Settings")]
        public Transform pointer;           // The pointer sprite in the scene
        public float moveSpeed = 2f;        // Speed for swipe motion
        public float tapScale = 0.9f;       // Scale when “pressing” down
        public float tapDuration = 0.4f;    // How long one tap takes
        public float idleDelay = 5f;        // Replay tutorial after X seconds of no input

        private Coroutine currentRoutine;
        private float idleTimer;
        private bool hasReplayed = false;

        private enum TutorialType { None, Tap, Swipe }
        private TutorialType lastTutorialType = TutorialType.None;

        private Transform lastTapTarget;
        private Transform swipeFrom, swipeTo;

        // ---------------- UNITY METHODS ----------------

        private void Update()
        {
            // ✅ Detect input using the new Input System
            if (Keyboard.current.anyKey.wasPressedThisFrame ||
                Mouse.current.leftButton.wasPressedThisFrame ||
                Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
            {
                idleTimer = 0f;
                hasReplayed = false;
            }
            else
            {
                idleTimer += Time.deltaTime;

                if (idleTimer >= idleDelay && !hasReplayed)
                {
                    idleTimer = 0f;
                    hasReplayed = true;
                    ReplayLastTutorial();
                }
            }
        }

        // 👉 Call this to show a TAP animation on a sprite (Transform)
        public void ShowTap(Transform target)
        {
            StopCurrent();
            currentRoutine = StartCoroutine(TapRoutine(target));

            lastTutorialType = TutorialType.Tap;
            lastTapTarget = target;

            idleTimer = 0f;
            hasReplayed = false;
        }

        // 👉 Call this to show a SWIPE animation between two sprites (Transform)
        public void ShowSwipe(Transform from, Transform to)
        {
            StopCurrent();
            currentRoutine = StartCoroutine(SwipeRoutine(from, to));

            lastTutorialType = TutorialType.Swipe;
            swipeFrom = from;
            swipeTo = to;

            idleTimer = 0f;
            hasReplayed = false;
        }

        // 👉 Stop any current tutorial animation
        public void StopPointer()
        {
            StopCurrent();
            pointer.gameObject.SetActive(false);
        }

        private void StopCurrent()
        {
            if (currentRoutine != null)
                StopCoroutine(currentRoutine);

            currentRoutine = null;
        }

        // ---------------- PRIVATE ROUTINES ----------------

        private IEnumerator TapRoutine(Transform target)
        {
            pointer.gameObject.SetActive(true);

            pointer.position = target.position; // Keep pointer aligned with target

            // Store the original scale once
            Vector3 originalScale = pointer.localScale;

            // Tap down + release using original scale as baseline
            yield return ScalePointer(originalScale * tapScale, tapDuration / 2f);
            yield return ScalePointer(originalScale, tapDuration / 2f);
            yield return new WaitForSeconds(0.3f);

            pointer.gameObject.SetActive(false);
        }

        private IEnumerator SwipeRoutine(Transform from, Transform to)
        {
            pointer.gameObject.SetActive(true);

            Vector3 start = from.position + Vector3.up * 0.5f;
            Vector3 end = to.position + Vector3.up * 0.5f;
            pointer.position = start;

            float elapsed = 0f;

            while (elapsed < 1f)
            {
                elapsed += Time.deltaTime * moveSpeed;
                pointer.position = Vector3.Lerp(start, end, Mathf.SmoothStep(0, 1, elapsed));
                yield return null;
            }

            yield return new WaitForSeconds(0.3f);
            pointer.gameObject.SetActive(false);
        }

        private IEnumerator ScalePointer(Vector3 targetScale, float duration)
        {
            Vector3 initialScale = pointer.localScale;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                pointer.localScale = Vector3.Lerp(initialScale, targetScale, elapsed / duration);
                yield return null;
            }
        }

        // ---------------- REPLAY LOGIC ----------------

        private void ReplayLastTutorial()
        {
            if (lastTutorialType == TutorialType.Tap && lastTapTarget != null)
            {
                ShowTap(lastTapTarget);
            }
            else if (lastTutorialType == TutorialType.Swipe && swipeFrom != null && swipeTo != null)
            {
                ShowSwipe(swipeFrom, swipeTo);
            }
        }
    }
}
