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
    }
}