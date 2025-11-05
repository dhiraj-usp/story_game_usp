using System.Collections;
using UnityEngine;

namespace USP.Minigame.DF_Game
{
    public class DF_Ui_GameManager : MonoBehaviour
    {
        [Header("Speech Bubbles")]
        [SerializeField] private SpeechBubbleImage girlspeechbubble;
        [SerializeField] private SpeechBubbleImage dogspeechbubble;
        [SerializeField] private SpeechBubbleImage feedingspeechbubble;
        [SerializeField] private SpeechBubbleImage bathspeechbubble;
        [SerializeField] private SpeechBubbleImage dogfinalspeechbubble;

        [Header("Particles")]
        [SerializeField] private ParticleSystem doorknockparticle;

        [Header("Clickable Objects")]
        [SerializeField] private ClikcableObject door;
        [SerializeField] private ClikcableObject bathsbutton;
        [SerializeField] private ClikcableObject feedbutton;

        [Header("Characters")]
        [SerializeField] private Transform girl;
        [SerializeField] private Transform dog;
        [SerializeField] private Animator kennelAnimator;
        [SerializeField] private Animator dogAnimator;
        [SerializeField] private Animator girlAnimator;
        
        [SerializeField] private DF_GameManager gameManager;

        [SerializeField] private GameObject feedingstatus;
        [SerializeField] private GameObject bathstatus;

        private bool DogspeechComplete;
        
        private void Start()
        {
            
        }

        private void OnEnable()
        {
            
        }

        public void CheckMiniGameStatus()
        {
            feedingstatus.SetActive(gameManager.GetGameProgress(DF_GameManager.GamePhases.Feeding));
            bathstatus.SetActive(gameManager.GetGameProgress(DF_GameManager.GamePhases.Bath));
            CheckForGameEnd();
        }

        private void OnDisable()
        {
            if(door==null || bathsbutton==null|| feedbutton==null)
                return;
            if(door.OnClick != null)
                door.OnClick -= OnKennelClicked;
            if(bathsbutton.OnClick != null)
                bathsbutton.OnClick -= OnBathSelected;
            if(feedbutton.OnClick != null)
                feedbutton.OnClick -= OnFeedingSelected;
        }

        /// <summary>
        /// Sets initial state: all UI closed, waits for kennel interaction.
        /// </summary>
        
        [ContextMenu("Initialize")]
        public void InitializeGame()
        {
            
            Debug.Log("Game initialized. Waiting for kennel click...");
            door.OnClick += OnKennelClicked;
            bathsbutton.OnClick += OnBathSelected;
            feedbutton.OnClick += OnFeedingSelected;
            CloseAllUI();
        }

        /// <summary>
        /// Closes all UI elements (speech bubbles, buttons, etc.)
        /// </summary>
        private void CloseAllUI()
        {
            girlspeechbubble.gameObject.SetActive(false);
            dogspeechbubble.gameObject.SetActive(false);
            feedingspeechbubble.gameObject.SetActive(false);
            bathspeechbubble.gameObject.SetActive(false);
            dogfinalspeechbubble.gameObject.SetActive(false);
            bathsbutton.gameObject.SetActive(false);
            feedbutton.gameObject.SetActive(false);
        }

        /// <summary>
        /// Called when the player clicks on the dog kennel.
        /// </summary>
        public void OnKennelClicked()
        {
            door.OnClick-= OnKennelClicked;
            Debug.Log("Kennel clicked!");
            PlayKnockParticle();
            PlayKennelOpenAnimation();
        }

        /// <summary>
        /// Plays knock particle effect.
        /// </summary>
        private void PlayKnockParticle()
        {
            if (doorknockparticle != null)
                doorknockparticle.Play();
        }

        /// <summary>
        /// Plays kennel opening animation and triggers dog entrance.
        /// </summary>
        private void PlayKennelOpenAnimation()
        {
            if (kennelAnimator != null)
            {
                kennelAnimator.SetTrigger("Open");
                // Assume the kennel animation calls OnDogComesOut() via animation event
            }
            else
            {
                // fallback
                OnDogComesOut();
            }
        }

        /// <summary>
        /// Called when dog comes out of the kennel.
        /// </summary>
       [ContextMenu("Simulate dailouge")]
        public void OnDogComesOut()
        {
            Debug.Log("Dog came out of the kennel.");
            PlayDogEntranceAnimation();
            StartCoroutine(DialogueSequence());
        }

        private void PlayDogEntranceAnimation()
        {
            if (dogAnimator != null)
                dogAnimator.SetTrigger("DogOut");
        }

        /// <summary>
        /// Full dialogue sequence coroutine: Girl Hi → Dog Hi → Girl How are you → Dog options.
        /// </summary>
        private IEnumerator DialogueSequence()
        {
            yield return new WaitForSeconds(2.5f);

            ShowGirlSpeech(0); // “Hi”
            yield return new WaitForSeconds(2.2f);

            ShowDogSpeech(0); // “Hi”
            dogspeechbubble.EnableClick(DogsaysHI);
            yield return new WaitUntil(() => DogspeechComplete);
            dogspeechbubble.DisableClick();
            DogspeechComplete = false;

            ShowGirlSpeech(1); // “How are you?”
            yield return new WaitForSeconds(2.2f);

            ShowDogOptions();
        }

        private void DogsaysHI()
        {
            DogspeechComplete = true;
            CloseDogSpeech();
        }
        
        
        

        /// <summary>
        /// Displays a speech bubble for the girl character.
        /// </summary>
        private void ShowGirlSpeech(int bubbleIndex)
        {
            girlspeechbubble.gameObject.SetActive(true);
            girlspeechbubble.ShowBubble(bubbleIndex);
        }

        /// <summary>
        /// Displays a speech bubble for the dog character.
        /// </summary>
        private void ShowDogSpeech(int bubbleIndex)
        {
            dogspeechbubble.gameObject.SetActive(true);
            dogspeechbubble.ShowBubble(bubbleIndex,-1f,false);
        }

        private void CloseDogSpeech()
        {
            StartCoroutine(dogspeechbubble.PopOutBubble());
        }

        /// <summary>
        /// Displays the dog’s response options (Feeding / Bath).
        /// </summary>
        private void ShowDogOptions()
        {
            Debug.Log("Showing dog options (Feeding / Bath).");

            bathsbutton.gameObject.SetActive(true);
            feedbutton.gameObject.SetActive(true);

            bathspeechbubble.gameObject.SetActive(true);
            feedingspeechbubble.gameObject.SetActive(true);

            // Optional pop animation
            bathspeechbubble.ShowBubble(0,-1f,false);
            feedingspeechbubble.ShowBubble(00,-1f,false);
        }

        /// <summary>
        /// Called when player selects “Feeding”.
        /// </summary>
        public void OnFeedingSelected()
        {
            feedbutton.OnClick -= OnFeedingSelected;
            Debug.Log("Feeding sequence selected.");
            GoToFeedingSequence();
        }

        /// <summary>
        /// Called when player selects “Bath”.
        /// </summary>
        public void OnBathSelected()
        {
            bathsbutton.OnClick -= OnBathSelected;
            Debug.Log("Bath sequence selected.");
            GoToBathSequence();
        }

        /// <summary>
        /// Loads or triggers the feeding mini-game.
        /// </summary>
        private void GoToFeedingSequence()
        {
            // You can transition scene or activate feeding phase here
            gameManager.ChangeGamePhase(DF_GameManager.GamePhases.Feeding);
            Debug.Log("Transition to Feeding mini-game...");
        }

        /// <summary>
        /// Loads or triggers the bath mini-game.
        /// </summary>
        private void GoToBathSequence()
        {
            gameManager.ChangeGamePhase(DF_GameManager.GamePhases.Bath);
            Debug.Log("Transition to Bath mini-game...");
        }
        
        /// <summary>
        /// Checks if both mini-games are completed, then triggers the game ending sequence.
        /// </summary>
        public void CheckForGameEnd()
        {
            bool feedingDone = gameManager.GetGameProgress(DF_GameManager.GamePhases.Feeding);
            bool bathDone = gameManager.GetGameProgress(DF_GameManager.GamePhases.Bath);
            if (feedingDone || bathDone)
            {
                dogAnimator.SetTrigger("DogOutidle");
            }
            if (feedingDone && bathDone)
            {
                Debug.Log("All mini-games completed. Triggering game ending sequence...");
                StartCoroutine(GameEndSequence());
            }
        }

        
        /// <summary>
        /// Plays the ending dialogue and animations (dog says bye, girl says bye, both go back in).
        /// </summary>
        private IEnumerator GameEndSequence()
        {
            CloseAllUI();

            yield return new WaitForSeconds(5f);

            // Dog says “Bye!”
            dogfinalspeechbubble.gameObject.SetActive(true);
            dogfinalspeechbubble.ShowBubble(0); // Assume index 2 = "Bye!"
            dogfinalspeechbubble.EnableClick(DogfinalSpeechcomplete);
            yield return new WaitUntil(() => DogspeechComplete);
            dogfinalspeechbubble.DisableClick();
            DogspeechComplete = false;

            // Girl says “Bye!”
            girlspeechbubble.gameObject.SetActive(true);
            girlspeechbubble.ShowBubble(2); // Assume index 2 = "Bye!"
            yield return new WaitForSeconds(2.2f);
            girlspeechbubble.gameObject.SetActive(false);
            dogspeechbubble.gameObject.SetActive(false);
            // Dog goes back in
            if (dogAnimator != null)
                dogAnimator.SetTrigger("DogIn");
            Debug.Log("Dog going back inside kennel...");

            yield return new WaitForSeconds(2f);

            // Girl also goes back
            if (girl != null)
            {
                // Optional: Animate girl moving out of scene
                girlAnimator.SetTrigger("Go in");
                Debug.Log("Girl going back...");
            }

            yield return new WaitForSeconds(3f);

            

            // Optional: Trigger kennel close animation
            if (kennelAnimator != null)
                kennelAnimator.SetTrigger("Close");

            Debug.Log("Game Ended! All characters back inside.");
            gameManager.ChangeGamePhase(DF_GameManager.GamePhases.GameEnd);
        }

        private void DogfinalSpeechcomplete()
        {
            DogspeechComplete = true;
            StartCoroutine(dogfinalspeechbubble.PopOutBubble());
        }

    }
}

