using System;
using System.Collections.Generic;
using UnityEngine;

namespace USP.Minigame.DF_Game
{
    
    public class GameResolution : MonoBehaviour
    {
        [Serializable]
        public class SceneObjects
        {
            public GameObject ipad_object;
            public GameObject iphone_object;
        }

        [Serializable]
        public class GameResolutionData
        {
            public DF_GameManager.GamePhases gamePhase;
            public float ipadscale;
            public float iphonescale;
            public List<SceneObjects> sceneObjects;
        }
        
        
        [SerializeField] Camera orthographiccamera;
        [SerializeField] GameResolutionData gameResolutionData;
        [SerializeField] private SpriteRenderer background;
        [SerializeField] private float defaultsizevalue;
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
                orthographiccamera.orthographicSize=gameResolutionData.iphonescale;
                AdjustCameraSize();
                EnableIpadObjects(false);
            }
            else if (aspect >= 1.3f && aspect <= 1.4f)
            {
                Debug.Log("💻 Likely iPad (4:3)");
                orthographiccamera.orthographicSize=gameResolutionData.ipadscale;
                //Keep default orthoscale
                EnableIpadObjects(true);
                
            }
            else
            {
                orthographiccamera.orthographicSize=gameResolutionData.iphonescale;
                AdjustCameraSize();
                EnableIpadObjects(false);
                Debug.Log("🖥️ Other aspect ratio detected");
            }
        }
        void AdjustCameraSize()
        {
            const float maxSize = 5.4F;
            float aspectRatio = (float) Screen.width / Screen.height;
            float requiredSize = background.bounds.size.x / (2F * aspectRatio);
            orthographiccamera.orthographicSize = Mathf.Min(requiredSize, maxSize);
        }

        void EnableIpadObjects(bool enable)
        {
            gameResolutionData.sceneObjects.ForEach(x=>x.ipad_object.SetActive(enable));
            gameResolutionData.sceneObjects.ForEach(x=>x.iphone_object.SetActive(!enable));
        }


        public void HandleSceneSpecificOrthographic(float sizevalue)
        {
            if (checkifdeviceisIpad())
            {
                return;
            }
            defaultsizevalue=orthographiccamera.orthographicSize;
            orthographiccamera.orthographicSize=sizevalue;
        }

        public void ResetSceneSpecificOrthographic()
        {
            if (checkifdeviceisIpad())
            {
                return;
            }
            orthographiccamera.orthographicSize=defaultsizevalue;
        }

        private bool checkifdeviceisIpad()
        {
            int width = Screen.width;
            int height = Screen.height;
            float aspect = (float)width / height;
             if (aspect >= 1.3f && aspect <= 1.4f)
            {
                Debug.Log("💻 Likely iPad (4:3)");
                return true;
                
            }
             else
             {
                 return false;
             }
        }
    }
}