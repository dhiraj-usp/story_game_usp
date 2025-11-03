using System.Collections.Generic;
using UnityEngine;
using System.Collections;

namespace USP.Minigame.DF_Game
{
    public class DogBathManager : MonoBehaviour
    {
        [Header("Dog Settings")]
        [SerializeField] private Animator dog;
         // cleaner versions in order

        [Header("Bubbles")]
        [SerializeField] private List<GameObject> bubbles; // use BathBubble, not GameObject
        
        [SerializeField] ParticleSystem shineparticle;
        
        [SerializeField] private DF_GameManager gameManager;

        [SerializeField] private int poppedCount = 0;
        [SerializeField] private int nextSpriteThreshold = 5; // change sprite every 5 pops
        [SerializeField] private int completedthreshold = 25;
        
        

        private void Start()
        {
            
        }

        private void OnEnable()
        {
            Initialize();
        }

        public void Initialize()
        {
            poppedCount = 0;
            nextSpriteThreshold = 5;

            

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
                UpdateDogSprite();// next milestone
                nextSpriteThreshold += 5;
            }

            // Optionally check for all bubbles cleared
            if (poppedCount >= bubbles.Count)
            {
                StartCoroutine(DogbathComplete());
            }
        }

        private IEnumerator DogbathComplete()
        {
            UpdateDogSprite();
            yield return new WaitForSeconds(0.5f);
            shineparticle.Play();
            yield return new WaitForSeconds(2f);
            OnBathComplete();
        }

        private void UpdateDogSprite()
        {
            dog.SetTrigger("Nextstage");
        }

        private void OnBathComplete()
        {
            Debug.Log("Bathing complete! 🎉 Dog is fully clean!");
            
            // Optionally tell game manager
            gameManager.UpdateGameProgress(DF_GameManager.GamePhases.Bath,true);
            gameManager.ChangeGamePhase(DF_GameManager.GamePhases.GameStart);
        }
    }
}
