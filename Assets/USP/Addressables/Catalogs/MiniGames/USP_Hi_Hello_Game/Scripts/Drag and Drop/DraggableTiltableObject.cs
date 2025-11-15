using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace USP.Minigame.DF_Game
{
    [RequireComponent(typeof(Collider2D))]
    public class DraggableTiltableObject2D : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [Header("References")]
        [SerializeField] private Camera mainCamera;

        [Header("Tilt Settings")]
        [SerializeField] private float maxTiltAngle = 25f;
        [SerializeField] private float tiltThreshold = 15f;
        [SerializeField] private float tiltSpeed = 5f;

        [Header("Drag Settings")]
        [SerializeField] private float dragSmoothness = 10f;
        [SerializeField] private float returnSpeed = 5f;

        [Header("Visual Feedback")]
        [SerializeField] private float dragPopScale = 1.08f; // Slight scale-up on grab
        [SerializeField] private float bounceDuration = 0.1f; // Small bounce duration
        [SerializeField] private float wobbleStrength = 2f; // Degrees of small rotation wobble

      

        [Header("Action Settings")]
        public Action OnTiltThresholdReached;
        public Action OnTiltThresholdnotReached;

        private Vector3 dragOffset;
        [SerializeField] private float currentTilt = 0f;
        private bool isDragging = false;
        private bool thresholdTriggered = false;

        private Collider2D currentOverlapObject;
        private Vector3 originalPosition;
        private Quaternion originalRotation;
        private Vector3 originalScale;

        private float wobbleTime = 0f;

        private void Awake()
        {
            if (!mainCamera)
                mainCamera = Camera.main;

            originalPosition = transform.position;
            originalRotation = transform.rotation;
            originalScale = transform.localScale;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(eventData.position);
            mouseWorldPos.z = 0;
            dragOffset = transform.position - mouseWorldPos;
            isDragging = true;
            StopAllCoroutines();

            // ✨ Small "pop" scale effect on grab
            StartCoroutine(ScaleTo(originalScale * dragPopScale, bounceDuration));
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!isDragging) return;

            Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(eventData.position);
            mouseWorldPos.z = 0;

            Vector3 targetPos = mouseWorldPos + dragOffset;
            transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * dragSmoothness);

            // Tilt based on horizontal drag movement
            float tiltInput = Mathf.Clamp(eventData.delta.x, -1f, 1f);
            currentTilt = Mathf.Lerp(currentTilt, tiltInput * maxTiltAngle, Time.deltaTime * tiltSpeed);
            transform.rotation = Quaternion.Euler(0, 0, -currentTilt);

            // ✨ Add a subtle wobble motion while dragging
            wobbleTime += Time.deltaTime * 10f;
            float wobble = Mathf.Sin(wobbleTime) * wobbleStrength * (currentTilt / maxTiltAngle);
            transform.rotation *= Quaternion.Euler(0, 0, wobble);
           
            // Trigger once when threshold exceeded
            if (!thresholdTriggered && Mathf.Abs(currentTilt) >= tiltThreshold)
            {
                thresholdTriggered = true;
                OnTiltThresholdReached?.Invoke();
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            isDragging = false;
            OnTiltThresholdnotReached?.Invoke();
            StartCoroutine(ReturnToOrigin());
        }

        public void TriggerReturnToOrigin()
        {
            StartCoroutine(ReturnToOrigin());
        }

        private IEnumerator ReturnToOrigin()
        {
            // ✨ Small "release bounce" scale-down first
            StartCoroutine(ScaleTo(originalScale, 0.2f));

            while (Vector3.Distance(transform.position, originalPosition) > 0.01f ||
                   Quaternion.Angle(transform.rotation, originalRotation) > 0.5f)
            {
                transform.position = Vector3.Lerp(transform.position, originalPosition, Time.deltaTime * returnSpeed);
                transform.rotation = Quaternion.Lerp(transform.rotation, originalRotation, Time.deltaTime * tiltSpeed);
                yield return null;
            }

            transform.position = originalPosition;
            transform.rotation = originalRotation;
            transform.localScale = originalScale;
            currentTilt = 0f;
            thresholdTriggered = false;
        }

        // ✨ Smooth scale animation
        private IEnumerator ScaleTo(Vector3 targetScale, float duration)
        {
            Vector3 startScale = transform.localScale;
            float time = 0f;

            while (time < duration)
            {
                time += Time.deltaTime;
                float t = time / duration;
                t = t * t * (3f - 2f * t); // smoothstep easing
                transform.localScale = Vector3.Lerp(startScale, targetScale, t);
                yield return null;
            }

            transform.localScale = targetScale;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            currentOverlapObject = other;
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (currentOverlapObject == other)
                currentOverlapObject = null;
        }

        public GameObject GetCurrentOverlapObject()
        {
            return currentOverlapObject ? currentOverlapObject.gameObject : null;
        }
    }
}
