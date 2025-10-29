using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace USP.Minigame.DF_Game
{
    public class DogFeedingManager : MonoBehaviour
    {
        [SerializeField] private DF_GameManager gameManager; 

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
        [SerializeField] private bool isCycleActive = false;
        private bool isSnackDropComplete=false;

        public void StartDogFeeding()
        {
            gameManager.ChangeGamePhase(DF_GameManager.GamePhases.Feeding);

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
                gameManager.UpdateGameProgress(DF_GameManager.GamePhases.Feeding, true);
                
            }
        }
    }
}