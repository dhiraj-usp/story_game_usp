using UnityEngine;
using UnityEngine.Events;

namespace USP.Minigame.DF_Game
{
    public class DF_AnimationTrigger : MonoBehaviour
    {
        [Header("Trigger Settings")]
        [Tooltip("When to invoke the event (OnStart, OnEnable, OnTriggerEnter, Manual etc.)")]
        public TriggerType triggerType = TriggerType.OnStart;

        [Header("Event to Invoke")]
        [Tooltip("Assign actions here (e.g., play animation, enable object, call method, etc.)")]
        public UnityEvent onTriggered;

        public enum TriggerType
        {
            OnStart,
            OnEnable,
            OnTriggerEnter,
            Manual
        }

        private void Start()
        {
            if (triggerType == TriggerType.OnStart)
                TriggerEvent();
        }

        private void OnEnable()
        {
            if (triggerType == TriggerType.OnEnable)
                TriggerEvent();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (triggerType == TriggerType.OnTriggerEnter)
                TriggerEvent();
        }

        /// <summary>
        /// Manually trigger this from another script.
        /// </summary>
        public void TriggerEvent()
        {
            onTriggered?.Invoke();
        }
    }
}