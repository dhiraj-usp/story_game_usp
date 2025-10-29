using System;
using UnityEngine;

namespace USP.Minigame.DF_Game
{
    public class BathBubble : MonoBehaviour,IClickable
    {
        [Header("Sway Settings")]
        public float swayAmplitude = 0.2f;   // how far it moves horizontally
        public float swayFrequency = 1.0f;   // how fast it sways
        public float bobAmplitude = 0.1f;    // how far it moves vertically
        public float bobFrequency = 0.5f;    // how fast it bobs

        private Vector3 _startPos;
        private float _randomOffset;
        
        [SerializeField] private DF_GameManager gameManager;
        [SerializeField] private ParticleSystem burstParticlePrefab; 

        private void Start()
        {
            _startPos = transform.position;
            _randomOffset = UnityEngine.Random.Range(0f, Mathf.PI * 2f); 
            // gives each bubble a unique sway pattern
        }

        private void Update()
        {
            // Sway (left-right)
            float sway = Mathf.Sin((Time.time + _randomOffset) * swayFrequency) * swayAmplitude;
            // Bob (up-down)
            float bob = Mathf.Sin((Time.time + _randomOffset) * bobFrequency) * bobAmplitude;

            transform.position = _startPos + new Vector3(sway, bob, 0f);
        }
        
       
        public void OnClicked()
        {
            Debug.Log("Bath Bubble Clicked");
            gameManager.OnClickonBathBubble(this);
        }
        
        public void PopBubble()
        {
            if (burstParticlePrefab != null)
            {
                // Instantiate particle at bubble position
                ParticleSystem burst = Instantiate(burstParticlePrefab, transform.position, Quaternion.identity);
                burst.Play();

                // Destroy the particle system after it finishes
                Destroy(burst.gameObject, burst.main.duration);
            }
            else
            {
                Debug.LogWarning($"No burst particle assigned for {gameObject.name}");
            }

            // Optionally play sound effect
            // gameManager?.soundManager.PlaySFX("BubblePop", 0.6f);

            // Deactivate bubble
            gameObject.SetActive(false);
        }
    }
}