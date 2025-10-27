using System;
using UnityEngine;

namespace USP.Minigame.DF_Game
{
    public class AnimationTrigger : MonoBehaviour
    {
        public Action OnAnimationTriggered;
        [SerializeField] private AudioSource audioSource;
        

        public void TriggerAnimation()
        {
            OnAnimationTriggered?.Invoke();
            audioSource?.Play();
        }
    }
}
