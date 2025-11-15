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
        [SerializeField] private float lastInteractionTime;
        [SerializeField] private bool isRunning;
        [SerializeField] private bool hasStartedOnce;
       

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
            
            isRunning = false;
            pointer.gameObject.SetActive(false);
            if (tutorialRoutine != null)
                StopCoroutine(tutorialRoutine);
        }

        private void OnUserInteracted(InputAction.CallbackContext ctx)
        {
            Stoptutorial();
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
            isRunning = true;
            pointer.gameObject.SetActive(true);

            Canvas canvas = pointer.GetComponentInParent<Canvas>();
            if (canvas == null)
            {
                Debug.LogWarning("Pointer must be under a Canvas!");
                yield break;
            }

            float elapsed = 0f;

            while (elapsed < tutorialDisplayTime)
            {
                // Update pointer position to follow target (important for moving objects)
                Vector3 screenPos = gameCamera.WorldToScreenPoint(target.position);
                if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                        canvas.transform as RectTransform,
                        screenPos,
                        canvas.worldCamera,
                        out Vector2 localPoint))
                {
                    pointer.localPosition = localPoint;
                }

                // Tap animation (down)
                pointer.localScale = Vector3.one * tapScale;
                yield return new WaitForSeconds(tapDuration);
                elapsed += tapDuration;

                // Tap animation (up)
                pointer.localScale = Vector3.one;
                yield return new WaitForSeconds(tapDuration * 2f);
                elapsed += tapDuration * 2f;

                // Check for user input — stop early
                if (Time.time - lastInteractionTime < 0.5f)
                {
                    Debug.Log("Tutorial stopped due to user input");
                    break;
                }

                // Add elapsed frame time
                elapsed += Time.deltaTime;
            }

            pointer.gameObject.SetActive(false);
            isRunning = false;

            // After tutorial ends, start idle timer again
            lastInteractionTime = Time.time;
        }


        // -------------------------------
        // 👉 SWIPE ROUTINE
        // -------------------------------
        IEnumerator SwipeRoutine(Transform start, Transform end)
        {
            isRunning = true;
            pointer.gameObject.SetActive(true);

            Canvas canvas = pointer.GetComponentInParent<Canvas>();
            if (canvas == null)
            {
                Debug.LogWarning("Pointer must be under a Canvas!");
                yield break;
            }

            float elapsed = 0f;

            while (elapsed < tutorialDisplayTime)
            {
                Vector3 startPos = gameCamera.WorldToScreenPoint(start.position);
                Vector3 endPos = gameCamera.WorldToScreenPoint(end.position);

                if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                        canvas.transform as RectTransform,
                        startPos,
                        canvas.worldCamera,
                        out Vector2 localStart) &&
                    RectTransformUtility.ScreenPointToLocalPointInRectangle(
                        canvas.transform as RectTransform,
                        endPos,
                        canvas.worldCamera,
                        out Vector2 localEnd))
                {
                    float t = 0f;
                    while (t < 1f)
                    {
                        t += Time.deltaTime * moveSpeed;
                        pointer.localPosition = Vector3.Lerp(localStart, localEnd, t);
                        yield return null;
                    }

                    yield return new WaitForSeconds(0.5f);
                    elapsed += (1f / moveSpeed) + 0.5f; // approximate swipe + pause time
                }

                if (Time.time - lastInteractionTime < 0.5f)
                {
                    Debug.Log("Tutorial stopped due to user input");
                    break;
                }

                elapsed += Time.deltaTime;
            }

            pointer.gameObject.SetActive(false);
            isRunning = false;
            lastInteractionTime = Time.time;
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
