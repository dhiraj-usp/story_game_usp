using System;
using System.Collections.Generic;
using UnityEngine;

namespace USP.Minigame.DF_Game
{
    
    public class GameResolution : MonoBehaviour
    {

        [Serializable]
        public class GameResolutionData
        {
            public DF_GameManager.GamePhases gamePhase;
            public float ipadscale;
            public float iphonescale;
        }
        
        
        [SerializeField] ViewportHandler viewportHandler;
        [SerializeField] GameResolutionData gameResolutionData;

        public void Awake()
        {
           CheckDeviceType();
        }
        private void CheckDeviceType()
        {
            int width = Screen.width;
            int height = Screen.height;
            float aspect = (float)width / height;

            Debug.Log($"Resolution: {width}x{height} | Aspect: {aspect:F2}");

            // Common aspect ratio ranges
            if (aspect >= 1.7f && aspect <= 1.8f)
            {
                viewportHandler.UnitsSize=gameResolutionData.iphonescale;
            }
            else if (aspect >= 1.3f && aspect <= 1.4f)
            {
                Debug.Log("💻 Likely iPad (4:3)");
                viewportHandler.UnitsSize=gameResolutionData.ipadscale;
                
            }
            else
            {
                Debug.Log("🖥️ Other aspect ratio detected");
            }
        }
    }
}