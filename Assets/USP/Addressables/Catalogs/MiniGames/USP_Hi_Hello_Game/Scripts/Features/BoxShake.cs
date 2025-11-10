using UnityEngine;

namespace USP.Minigame.DF_Game
{
    public class BoxShake : MonoBehaviour
    {
        public float shakeDuration = 0.5f;
        public float shakeMagnitude = 0.1f;
        public float dampingSpeed = 1.0f;

        private Vector3 initialPosition;
        private float remainingShakeTime;

        void Start()
        {
            initialPosition = transform.localPosition;
        }
        
        [ContextMenu("Shake")]
    
        public void Shake()
        {
            remainingShakeTime = shakeDuration;
        }

        void Update()
        {
            if (remainingShakeTime > 0)
            {
                transform.localPosition = initialPosition + Random.insideUnitSphere * shakeMagnitude;
                remainingShakeTime -= Time.deltaTime * dampingSpeed;
            }
            else
            {
                transform.localPosition = initialPosition;
            }
        }
    }
}