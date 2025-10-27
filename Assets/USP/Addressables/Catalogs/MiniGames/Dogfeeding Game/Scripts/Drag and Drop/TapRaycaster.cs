using UnityEngine;
using UnityEngine.InputSystem; // New Input System

namespace USP.Minigame.DF_Game
{
    public class TapRaycaster : MonoBehaviour
    {
        [SerializeField] private Camera mainCamera;

        private void Awake()
        {
            if (!mainCamera)
                mainCamera = Camera.main;
        }

        private void Update()
        {
#if UNITY_EDITOR || UNITY_STANDALONE
            // 👇 Handle clicks with mouse when running in Editor or PC build
            if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            {
                Vector2 screenPos = Mouse.current.position.ReadValue();
                HandleTap(screenPos);
            }

#elif UNITY_ANDROID || UNITY_IOS
            // 👇 Handle touch input when running on mobile
            if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
            {
                Vector2 screenPos = Touchscreen.current.primaryTouch.position.ReadValue();
                HandleTap(screenPos);
            }
#endif
        }

        private void HandleTap(Vector2 screenPosition)
        {
            if (mainCamera == null)
            {
                Debug.LogWarning("TapRaycaster: mainCamera is not assigned or missing MainCamera tag.");
                return;
            }

            Vector2 worldPos = mainCamera.ScreenToWorldPoint(screenPosition);
            RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero);

            if (hit.collider == null)
            {
                 Debug.Log("No collider hit at: " + worldPos);
                return;
            }

            var clickable = hit.collider.GetComponent<IClickable>();
            if (clickable != null)
            {
                clickable.OnClicked();
            }
            else
            {
                Debug.Log("Hit object is not clickable: " + hit.collider.name);
            }
        }
    }
}