using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace USP.Minigame.DF_Game
{
    public class DF_GameManager : MonoBehaviour
    {

        [Serializable]
        public enum GamePhases
        {
            GameStart,
            Feeding, //The scene has a dog food box which is draggable and tiltable on tilting dog food comes out
            Bath, // Bubbles appear over the dog which burst on click , as each bubble is burst dog becomes cleaner
            GameEnd,

        }
        [Serializable]
        public class GameProgress
        {
            public GamePhases gamePhase;
            public bool IsCompleted;
        }

        public GamePhases gamePhase = GamePhases.GameStart;
        public List<GameProgress> gameProgress;
        
        [SerializeField] private Camera camera;
        [SerializeField] private DogFeedingManager dogFeedingManager;
        [SerializeField] private DogBathManager dogBathManager;
        public void UpdateGameProgress(GamePhases gamePhase, bool isCompleted)
        {
            gameProgress.Where(x=>x.gamePhase == gamePhase).FirstOrDefault().IsCompleted = isCompleted;
        }

        public void ChangeGamePhase(GamePhases phase)
        {
            gamePhase = phase;
           
        }
        

        private void Start()
        {
            SetScreen();
           dogFeedingManager.StartDogFeeding();
            
        }

        private void SetScreen()
        {
            Screen.orientation = ScreenOrientation.LandscapeLeft;
            // Detect approximate iPad aspect ratio (around 4:3)
            float aspect = (float)Screen.width / Screen.height;

            // Typical iPad aspect ratios range between 1.3 and 1.4
            if (aspect > 1.28f && aspect < 1.36f)
            {
                camera.orthographicSize = 6f;
                Debug.Log("Detected iPad-like aspect ratio — orthographicSize set to 6");
            }
            
        }

       

    }
}