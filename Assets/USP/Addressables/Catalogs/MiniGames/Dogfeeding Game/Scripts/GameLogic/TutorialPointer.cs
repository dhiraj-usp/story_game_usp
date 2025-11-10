using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;

namespace USP.Minigame.DF_Game
{
    public class TutorialPointer : MonoBehaviour
    {
        [Header("Pointer Settings")]
        [SerializeField] private RectTransform pointer;        // UI Image of the hand or arrow
        [SerializeField] private float moveSpeed = 2f;         // Speed for swipe/tap animation
        [SerializeField] private float tapScale = 0.9f;        // Scale down on tap
        [SerializeField] private float tapDuration = 0.3f;     // Tap press animation time
        [SerializeField] private float idleTimeToReplay = 5f;  // Seconds before replay if no input
        [SerializeField] private float tutorialDisplayTime = 4f; // How long tutorial plays before stopping

        [Header("Tutorial Targets")]
        [SerializeField] private List<Transform> targets;      // One = tap, Two = swipe

        [Header("References")]
        [SerializeField] private Camera gameCamera;            // Camera rendering your 2D world
        [SerializeField] private Canvas canvas;                // Canvas where pointer lives (Screen Space - Camera)

        private Coroutine tutorialRoutine;
        private float lastInteractionTime;
        private bool isRunning;
        private bool hasStartedOnce;

        // New Input System action
        private InputAction clickOrTouchAction;

        void Awake()
        {
            clickOrTouchAction = new InputAction(type: InputActionType.PassThrough, binding: "<Pointer>/press");
            clickOrTouchAction.AddBinding("<Touchscreen>/press");

            if (canvas == null)
                canvas = pointer.GetComponentInParent<Canvas>();

            if (gameCamera == null)
                gameCamera = Camera.main;
        }

        void OnEnable()
        {
            clickOrTouchAction.Enable();
            clickOrTouchAction.performed += OnUserInteracted;
        }

        void OnDisable()
        {
            clickOrTouchAction.performed -= OnUserInteracted;
            clickOrTouchAction.Disable();
        }

        public void Stoptutorial()
        {
            targets.Clear();
        }

        private void OnUserInteracted(InputAction.CallbackContext ctx)
        {
            pointer.gameObject.SetActive(false);
            lastInteractionTime = Time.time;
        }

        void Start()
        {
            lastInteractionTime = Time.time;
        }

        void Update()
        {
            if (targets == null || targets.Count == 0)
            {
               
                return;
            }
             
            // Restart only if the tutorial was shown at least once
            if (hasStartedOnce && !isRunning && Time.time - lastInteractionTime > idleTimeToReplay)
            {
                StartTutorial();
            }
        }

        public void UpdateTargets(Transform target, Transform anchor = null)
        {
            targets.Clear();

            if (anchor == null)
                targets.Add(target);
            else
            {
                targets.Add(anchor);
                targets.Add(target);
            }

            StartTutorial();
        }

        void StartTutorial()
        {
            hasStartedOnce = true;

            if (tutorialRoutine != null)
                StopCoroutine(tutorialRoutine);

            if (targets == null || targets.Count == 0)
                return;

            if (targets.Count == 1)
                tutorialRoutine = StartCoroutine(TapRoutine(targets[0]));
            else if (targets.Count == 2)
                tutorialRoutine = StartCoroutine(SwipeRoutine(targets[0], targets[1]));
        }

        // -------------------------------
        // 🖐 TAP ROUTINE
        // -------------------------------
        IEnumerator TapRoutine(Transform target)
        {
            yield return new WaitForSeconds(2f);
            isRunning = true;
            pointer.gameObject.SetActive(true);
            float startTime = Time.time;

            while (Time.time - startTime < tutorialDisplayTime)
            {
                UpdatePointerPosition(target);

                // Tap down
                pointer.localScale = Vector3.one * tapScale;
                yield return new WaitForSeconds(tapDuration);

                // Tap up
                pointer.localScale = Vector3.one;
                yield return new WaitForSeconds(tapDuration * 2f);

                // Stop early if player interacted
                if (Time.time - lastInteractionTime < 0.5f)
                    break;
            }

            pointer.gameObject.SetActive(false);
            isRunning = false;
        }

        // -------------------------------
        // 👉 SWIPE ROUTINE
        // -------------------------------
        IEnumerator SwipeRoutine(Transform start, Transform end)
        {
            isRunning = true;
            pointer.gameObject.SetActive(true);
            float startTime = Time.time;

            Vector2 startLocal, endLocal;
            if (!WorldToCanvasLocal(start.position, out startLocal) ||
                !WorldToCanvasLocal(end.position, out endLocal))
            {
                Debug.LogWarning("Failed to convert swipe positions!");
                yield break;
            }

            while (Time.time - startTime < tutorialDisplayTime)
            {
                float t = 0f;
                while (t < 1f)
                {
                    t += Time.deltaTime * moveSpeed;
                    Vector2 lerpPos = Vector2.Lerp(startLocal, endLocal, t);
                    pointer.localPosition = lerpPos;
                    yield return null;
                }

                yield return new WaitForSeconds(0.5f);
                pointer.localPosition = startLocal;

                // Stop early if player interacted
                if (Time.time - lastInteractionTime < 0.5f)
                    break;
            }

            pointer.gameObject.SetActive(false);
            isRunning = false;
        }

        // -------------------------------
        // 🎯 UTILITY: Convert World → Canvas Local
        // -------------------------------
        private void UpdatePointerPosition(Transform worldTarget)
        {
            if (WorldToCanvasLocal(worldTarget.position, out Vector2 localPoint))
                pointer.localPosition = localPoint;
        }

        private bool WorldToCanvasLocal(Vector3 worldPos, out Vector2 localPoint)
        {
            localPoint = Vector2.zero;
            if (canvas == null || gameCamera == null)
                return false;

            Vector3 screenPoint = gameCamera.WorldToScreenPoint(worldPos);
            return RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform,
                screenPoint,
                canvas.worldCamera,
                out localPoint
            );
        }
    }
}
