using System;
using System.Collections.Generic;
using UnityEngine;

namespace USP.Minigame.DF_Game
{
    public class DogBathManager : MonoBehaviour
    {
        [Header("Dog Settings")]
        [SerializeField] private SpriteRenderer dogSpriteRenderer;
        [SerializeField] private List<Sprite> dogSprites; // cleaner versions in order

        [Header("Bubbles")]
        [SerializeField] private List<GameObject> bubbles; // use BathBubble, not GameObject

        private int poppedCount = 0;
        private int nextSpriteThreshold = 5; // change sprite every 5 pops
        private int currentSpriteIndex = 0;

        private void Start()
        {
            Initialize();
        }

        public void Initialize()
        {
            poppedCount = 0;
            currentSpriteIndex = 0;
            nextSpriteThreshold = 5;

            dogSpriteRenderer.sprite = dogSprites[0];

            // Reactivate and bind bubble click events
            foreach (var bubble in bubbles)
            {
                bubble.SetActive(true);
            }
        }

        public void OnClickOnBathBubble(BathBubble bathBubble)
        {
            bathBubble.PopBubble();
            poppedCount++;

            // Play pop sound (if any)
            // SoundManager.Instance?.PlaySFX("BubblePop");

            // Check if it’s time to update dog sprite
            if (poppedCount >= nextSpriteThreshold)
            {
                UpdateDogSprite();
                nextSpriteThreshold += 5; // next milestone
            }

            // Optionally check for all bubbles cleared
            if (poppedCount >= bubbles.Count)
            {
                OnBathComplete();
            }
        }

        private void UpdateDogSprite()
        {
            currentSpriteIndex++;

            if (currentSpriteIndex < dogSprites.Count)
            {
                dogSpriteRenderer.sprite = dogSprites[currentSpriteIndex];
                Debug.Log($"Dog sprite updated to stage {currentSpriteIndex + 1}");
            }
        }

        private void OnBathComplete()
        {
            Debug.Log("Bathing complete! 🎉 Dog is fully clean!");
            // Optionally tell game manager
            // DF_GameManager.Instance.OnPhaseComplete(DF_GameManager.GamePhases.Bath);
        }
    }
}
