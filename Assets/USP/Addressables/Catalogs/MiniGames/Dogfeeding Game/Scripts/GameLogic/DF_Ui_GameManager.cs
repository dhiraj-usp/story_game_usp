using System;
using UnityEngine;

namespace USP.Minigame.DF_Game
{
    public class DF_Ui_GameManager : MonoBehaviour
    {
        
        [SerializeField] private SpeechBubbleImage girlspeechbubble;
        [SerializeField] private SpeechBubbleImage dogspeechbubble;
        [SerializeField] private SpeechBubbleImage feedingspeechbubble;
        [SerializeField] private SpeechBubbleImage bathspeechbubble;

        [SerializeField] private ParticleSystem doorknockparticle;

        [SerializeField] private ClikcableObject door;
        [SerializeField] private ClikcableObject bathsbutton;
        [SerializeField] private ClikcableObject feedbutton;
        
        
        
        // Called when the game starts — initializes the scene
        private void Start()
        {
            InitializeGame();
        }

        private void OnEnable()
        {
            door.OnClick += OnKennelClicked;
            bathsbutton.OnClick += OnBathSelected;
            feedbutton.OnClick += OnFeedingSelected;
        }
        private void OnDisable()
        {
            door.OnClick -= OnKennelClicked;
            bathsbutton.OnClick -= OnBathSelected;
            feedbutton.OnClick -= OnFeedingSelected;
        }


        /// <summary>
        /// Sets initial state: all UI closed, waits for kennel interaction.
        /// </summary>
        private void InitializeGame()
        {
            Debug.Log("Game initialized. Waiting for kennel click...");
            CloseAllUI();
        }

        /// <summary>
        /// Closes all UI elements (speech bubbles, buttons, etc.)
        /// </summary>
        private void CloseAllUI()
        {
            // TODO: Hide all panels and speech bubbles
        }

        /// <summary>
        /// Called when the player clicks on the dog kennel.
        /// </summary>
        public void OnKennelClicked()
        {
            Debug.Log("Kennel clicked!");
            PlayKnockParticle();
            PlayKennelOpenAnimation();
        }

        /// <summary>
        /// Spawns or plays knock particle effect.
        /// </summary>
        private void PlayKnockParticle()
        {
            // TODO: Instantiate or play knock particle
        }

        /// <summary>
        /// Plays kennel opening animation and triggers dog entrance.
        /// </summary>
        private void PlayKennelOpenAnimation()
        {
            // TODO: Play kennel door animation
            // On animation end, call OnDogComesOut()
        }

        /// <summary>
        /// Called when dog comes out of the kennel.
        /// </summary>
        public void OnDogComesOut()
        {
            Debug.Log("Dog came out of the kennel.");
            PlayDogEntranceAnimation();
            ShowInitialDialogue();
        }

        /// <summary>
        /// Handles dog's entrance animation.
        /// </summary>
        private void PlayDogEntranceAnimation()
        {
            // TODO: Trigger dog animator
        }

        /// <summary>
        /// Starts the dialogue sequence: Girl Hi → Dog Hi → Girl How are you.
        /// </summary>
        private void ShowInitialDialogue()
        {
            Debug.Log("Starting dialogue sequence.");
            ShowGirlSpeech("Hi!");
            ShowDogSpeech("Hi!");
            ShowGirlSpeech("How are you?");
            ShowDogOptions();
        }

        /// <summary>
        /// Displays a speech bubble for the girl character.
        /// </summary>
        private void ShowGirlSpeech(string text)
        {
            // TODO: Set speech bubble text and enable girl UI
        }

        /// <summary>
        /// Displays a speech bubble for the dog character.
        /// </summary>
        private void ShowDogSpeech(string text)
        {
            // TODO: Set speech bubble text and enable dog UI
        }

        /// <summary>
        /// Displays the dog’s response options (Feeding / Bath).
        /// </summary>
        private void ShowDogOptions()
        {
            Debug.Log("Showing dog options (Feeding / Bath).");
            // TODO: Enable option buttons on UI
        }

        /// <summary>
        /// Called when player selects “Feeding”.
        /// </summary>
        public void OnFeedingSelected()
        {
            Debug.Log("Feeding sequence selected.");
            GoToFeedingSequence();
        }

        /// <summary>
        /// Called when player selects “Bath”.
        /// </summary>
        public void OnBathSelected()
        {
            Debug.Log("Bath sequence selected.");
            GoToBathSequence();
        }

        /// <summary>
        /// Loads or triggers the feeding mini-game.
        /// </summary>
        private void GoToFeedingSequence()
        {
            // TODO: Load feeding sequence
        }

        /// <summary>
        /// Loads or triggers the bath mini-game.
        /// </summary>
        private void GoToBathSequence()
        {
            // TODO: Load bath sequence
        }
    }
}
