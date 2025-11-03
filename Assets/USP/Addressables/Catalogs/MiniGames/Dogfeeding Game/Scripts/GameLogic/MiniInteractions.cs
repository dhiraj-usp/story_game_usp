using System;
using System.Collections.Generic;
using UnityEngine;

namespace USP.Minigame.DF_Game
{
    public class MiniInteractions : MonoBehaviour
    {
        [SerializeField] private DraggableTiltableObject2D waterpot;
        [SerializeField] private ParticleSystem waterParticles;
        [SerializeField] private ClikcableObject foodbowl;
        [SerializeField] private ParticleSystem shineParticles;
        [SerializeField] private ClikcableObject tree;
        [SerializeField] private ParticleSystem treeParticles;
        [SerializeField] private List<CameraAnchor> cameraAnchors;

        [SerializeField] private Animator postboxanimator;
        [SerializeField] private ClikcableObject postbox;

        [SerializeField] private Animator boneanimator;
        [SerializeField] private ClikcableObject bone;
        
        [SerializeField] private List<SpriteRenderer> bulbs;
        [SerializeField] private List<ClikcableObject> bulbbuttons;
        [SerializeField] private Sprite bulboff;
        [SerializeField] private Sprite shine;
        private bool isBulboff = true;
        
        

        public void DisableDraggableAnchors()
        {
            foreach (var cameraAnchor in cameraAnchors)
            {
                cameraAnchor.enabled = false;
            }
        }

        private void Start()
        {
            DisableDraggableAnchors();
            InitializeInteractions();
        }

        public void InitializeInteractions()
        {
            waterpot.OnTiltThresholdReached += PlayWaterParticles;
            waterpot.OnTiltThresholdnotReached += StopWaterParticles;
            foodbowl.OnClick += OnclikcedFoodBowl;
            tree.OnClick += Onclikcedtree;
            postbox.OnClick += OnclickedPostbox;
            bone.OnClick += OnclickedBone;
            bulbbuttons.ForEach(x=>x.OnClick+=OnclickBulb);
        }

        public void PlayWaterParticles()
        {
            waterParticles.Play();
        }

        public void StopWaterParticles()
        {
            waterParticles.Stop();
        }

        public void OnclikcedFoodBowl()
        {
            shineParticles.Play();
        }
        public void Onclikcedtree()
        {
            treeParticles.Play();
        }

        public void OnclickedPostbox()
        {
            postboxanimator.SetTrigger("open");
        }
        public void OnclickedBone()
        {
            boneanimator.SetTrigger("open");
        }

        public void OnclickBulb()
        {
            if (isBulboff)
            {
                isBulboff = false;
                foreach (var bulb in bulbs)
                {
                    bulb.sprite = shine;
                }
            }
            else
            {
                isBulboff = true;
                foreach (var bulb in bulbs)
                {
                    bulb.sprite = bulboff;
                }
            }
        }
    }
}