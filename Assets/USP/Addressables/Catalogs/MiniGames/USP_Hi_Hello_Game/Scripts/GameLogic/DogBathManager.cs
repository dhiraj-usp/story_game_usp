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
         
         [SerializeField] private SoundManager soundManager;

        [Header("Bubbles")]
        [SerializeField] private List<GameObject> bubbles; // use BathBubble, not GameObject
        
        [SerializeField] ParticleSystem shineparticle;
        
        [SerializeField] private DF_GameManager gameManager;

        [SerializeField] private int poppedCount = 0;
        [SerializeField] private int nextSpriteThreshold = 5; // change sprite every 5 pops
        [SerializeField] private int completedthreshold = 25;
        
        [SerializeField] private Animator animator;
        [SerializeField] private string[] stateNames; // e.g. "Stage1", "Stage2", "Stage3", etc.
        [SerializeField] private TutorialPointer tutorialPointer;
        private int currentIndex = 0;
        

        private void Start()
        {
            
        }

        private void OnEnable()
        {
            
        }

        public void Initialize()
        {
            poppedCount = 0;
            nextSpriteThreshold = 7;

            

            // Reactivate and bind bubble click events
            foreach (var bubble in bubbles)
            {
                bubble.SetActive(true);
            }
            tutorialPointer.UpdateTargets(bubbles[0].transform);
            
        }

        public void OnClickOnBathBubble(BathBubble bathBubble)
        {
            bathBubble.PopBubble();
            poppedCount++;
            
            // Play pop sound (if any)
            soundManager.PlaySFX("bubblepop");
            // SoundManager.Instance?.PlaySFX("BubblePop");

            // Check if it’s time to update dog sprite
            if (poppedCount >= nextSpriteThreshold)
            {
                UpdateDogSprite();// next milestone
                soundManager.PlaySFX("Woof");
                nextSpriteThreshold += 7;
            }

            // Optionally check for all bubbles cleared
            if (poppedCount >= bubbles.Count)
            {
                StartCoroutine(DogbathComplete());
            }
            
            
        }

        private IEnumerator DogbathComplete()
        {
            PlayNextStage(true);
            yield return new WaitForSeconds(0.5f);
            shineparticle.Play();
            yield return new WaitForSeconds(6f);
            OnBathComplete();
        }

        private void UpdateDogSprite()
        {
            //dog.SetTrigger("Nextstage");
            PlayNextStage();
        }

        private void OnBathComplete()
        {
            Debug.Log("Bathing complete! 🎉 Dog is fully clean!");
            soundManager.PlaySFX("shine");
            // Optionally tell game manager
            gameManager.UpdateGameProgress(DF_GameManager.GamePhases.Bath,true);
            gameManager.ChangeGamePhase(DF_GameManager.GamePhases.GameStart);
        }
        
        
       

        public void PlayNextStage(bool shouldplayfromstart = false)
        {
            if (animator == null || stateNames.Length == 0) return;

            // Get current animation info
            AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(0);
            float normalizedTime = info.normalizedTime % 1f; // ensures it’s between 0–1

            // Move to next
            currentIndex = (currentIndex + 1) % stateNames.Length;

            string nextState = stateNames[currentIndex];
            
            
            // Blend to the next clip from the same point
            if(!shouldplayfromstart)
                animator.Play(nextState, 0, normalizedTime);
            else
            {
                animator.Play(nextState, 0, 0f);
            }
        }

       
        
        
    }
}
