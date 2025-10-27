using System;
using System.Collections.Generic;
using UnityEngine;

namespace USP.Minigame.DF_Game
{
    public class DF_GameManager : MonoBehaviour
    {

        [Serializable]
        public enum GamePhases
        {
            GameStart,
            Feeding,//The scene has a dog food box which is draggable and tiltable on tilting dog food comes out
            Bath, // Bubbles appear over the dog which burst on click , as each bubble is burst dog becomes cleaner
            GameEnd,
                
        }
        
        public GamePhases gamePhase = GamePhases.GameStart;

        public void ChangeGamePhase(GamePhases phase)
        {
            gamePhase = phase;
        }

        private void Start()
        {
            StartDogFeeding();
        }

        #region Dog Feeding

        [Header("Dog Feeding")] 
        [SerializeField]  private DraggableTiltableObject2D FoodBox;
        [SerializeField]  private GameObject FeedingBowl;
        [SerializeField]  private GameObject SnackPrefab;
        [SerializeField]  private ParticleSystem SnackParticles;
        [SerializeField]  private Animator SnackAnimator;
        [SerializeField]  private Transform SnackPoint;
        [SerializeField]  private List<Sprite> SnackSprite;

        private int SnackIndex = 0;

        public void StartDogFeeding()
        {
            ChangeGamePhase(GamePhases.Feeding);
            FoodBox.OnTiltThresholdReached += OnBoxTiltedOverBowl;
        }

        private void ResetDogFeeding()
        {
            FoodBox.OnTiltThresholdReached -= ResetDogFeeding;
        }

        private void OnBoxTiltedOverBowl()
        {
            FoodBox.enabled = false;
            Debug.Log("Dog Feeding");
            SnackAnimator.enabled = true;
            SnackParticles.Play();
            SnackIndex++;
            //wait for particle effect to run and gradually generate a snack prefab on the point with corresponding sprite
            //keep track of all generated sprites
            //
            FoodBox.enabled = true;
            FoodBox.TriggerReturnToOrigin();
        }


        #endregion

    }
}