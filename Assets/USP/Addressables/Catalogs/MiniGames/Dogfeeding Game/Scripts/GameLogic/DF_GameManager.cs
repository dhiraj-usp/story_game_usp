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
        [SerializeField] private DF_Ui_GameManager uiGameManager;
        [SerializeField] private GameObject mainscene;
        [SerializeField] private GameObject bathscene;
        [SerializeField] private GameObject feedscene;
        public void UpdateGameProgress(GamePhases gamePhase, bool isCompleted)
        {
            gameProgress.Where(x=>x.gamePhase == gamePhase).FirstOrDefault().IsCompleted = isCompleted;
        }

        public bool GetGameProgress(GamePhases gamePhase)
        {
            return gameProgress.Where(x => x.gamePhase == gamePhase).FirstOrDefault().IsCompleted;
        }

        public void ChangeGamePhase(GamePhases phase)
        {
            gamePhase = phase;
            OnChangeGamePhase();

        }

        private void Resetgamephases()
        {
            mainscene.SetActive(false);
            bathscene.SetActive(false);
            feedscene.SetActive(false);
        }
        

        private void Start()
        {
            SetScreen();
            OnGameStart();
            ChangeGamePhase(GamePhases.GameStart);
        }

        private void OnChangeGamePhase()
        {
            Resetgamephases();
            switch (gamePhase)
            {
                case GamePhases.GameStart:
                    mainscene.SetActive(true);
                    uiGameManager.CheckMiniGameStatus();
                    break;
                case GamePhases.Feeding:
                    feedscene.SetActive(true);
                    break;
                case GamePhases.Bath:
                    bathscene.SetActive(true);
                    break;
                case GamePhases.GameEnd:
                    mainscene.SetActive(true);
                    uiGameManager.CheckMiniGameStatus();
                    break;
                default:
                    break;
            }
        }

        private void SetScreen()
        {
            Screen.orientation = ScreenOrientation.LandscapeLeft;
            
        }

        private void OnGameStart()
        {
            uiGameManager.InitializeGame();
            
        }

       

    }
}