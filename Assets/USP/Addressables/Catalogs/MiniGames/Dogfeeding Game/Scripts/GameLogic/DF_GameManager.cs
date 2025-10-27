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
            StartDogFeeding();
        }

        #region Dog Feeding

        [Header("Dog Feeding")] [SerializeField]
        private DraggableTiltableObject2D FoodBox;

        [SerializeField] private GameObject FeedingBowl;
        [SerializeField] private Transform SnackParent;
        [SerializeField] private GameObject SnackPrefab;
        [SerializeField] private ParticleSystem SnackParticles;
        [SerializeField] private Animator SnackAnimator;
        [SerializeField] private Transform SnackPoint;
        [SerializeField] private List<Sprite> SnackSprites;
        [SerializeField] private float snackSpawnDelay = 0.5f;

        private readonly List<GameObject> spawnedSnacks = new List<GameObject>();
        private int snackIndex = 0;
        private int feedIndex = 0;
        private int totalTiltsRequired = 3;
        private bool isCycleActive = false;
        private bool isSnackDropComplete=false;

        public void StartDogFeeding()
        {
            ChangeGamePhase(GamePhases.Feeding);

            // reset state
            snackIndex = 0;
            feedIndex = 0;
            isCycleActive = false;
            isSnackDropComplete=false;
            spawnedSnacks.Clear();

            // ensure animation and particles are stopped
            SnackAnimator.enabled = false;
            SnackParticles.Stop();

            // attach event safely
            if(FoodBox.OnTiltThresholdReached != null)
                FoodBox.OnTiltThresholdReached -= OnBoxTiltedOverBowl;
            FoodBox.OnTiltThresholdReached += OnBoxTiltedOverBowl;

            Debug.Log("Dog Feeding Started — waiting for first tilt");
        }

        public void ResetDogFeeding()
        {
            FoodBox.OnTiltThresholdReached -= OnBoxTiltedOverBowl;
            StopAllCoroutines();
            SnackParticles.Stop();
            SnackAnimator.enabled = false;
            isCycleActive = false;

            foreach (var snack in spawnedSnacks)
                if (snack)
                    Destroy(snack);

            spawnedSnacks.Clear();
            snackIndex = 0;
        }

        private void OnBoxTiltedOverBowl()
        {
            if (isCycleActive) return; // block while a cycle is running
            if (snackIndex >= totalTiltsRequired)
            {
                Debug.Log("Feeding already complete!");
                return;
            }

            Debug.Log($"Tilt #{snackIndex + 1} detected — dropping snack!");
            isCycleActive = true;

            // disable dragging during pour
            FoodBox.enabled = false;

            // play particle and animation
            SnackAnimator.enabled = true;
            SnackAnimator.SetTrigger("Pour");
            SnackParticles.Play();

            StartCoroutine(SnackCycleRoutine());
        }

        private IEnumerator SnackCycleRoutine()
        {
            yield return new WaitForSeconds(snackSpawnDelay);

            // spawn one snack
            GameObject snack =
                Instantiate(SnackPrefab, SnackPoint.position, Quaternion.identity, SnackParent);
            if (snackIndex < SnackSprites.Count)
            {
                var sr = snack.GetComponent<SpriteRenderer>();
                if (sr) sr.sprite = SnackSprites[snackIndex];
            }

            snack.GetComponent<Snack>().OnuserClick += OnClickOnSnack;
            spawnedSnacks.Add(snack);
            snackIndex++;

            // stop pouring FX
            yield return new WaitForSeconds(0.5f);
            SnackParticles.Stop();
            SnackAnimator.enabled = false;

            // reset box to origin
            FoodBox.TriggerReturnToOrigin();

            // re-enable dragging
            yield return new WaitForSeconds(0.5f);
            FoodBox.enabled = true;
            isCycleActive = false;

            // check completion
            if (snackIndex >= totalTiltsRequired)
            {
                OnSnackDropComplete();
            }
        }

        private void OnSnackDropComplete()
        {
            isSnackDropComplete = true;
            FoodBox.enabled = false;
            Debug.Log("Feeding Complete! All snacks dropped 🎉");
            
        }

        private void OnClickOnSnack(Snack snack)
        {
            if(!isSnackDropComplete)
                return;
            //play eating audio
            //Destory snack object
            spawnedSnacks.Remove(snack.gameObject);
            Destroy(snack.gameObject);
            feedIndex++;
            if (feedIndex >= totalTiltsRequired)
            {
                //Feeding complete
                ResetDogFeeding();
                UpdateGameProgress(GamePhases.Feeding, true);
                ChangeGamePhase(GamePhases.GameStart);
            }
        }

        #endregion


    }
}