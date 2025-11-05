using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace USP.Minigame.DF_Game
{
    public class SoundManager : MonoBehaviour
    {
        [Header("Audio Source")]
        [SerializeField] private AudioSource sfxSource;
        
        [SerializeField] private AudioSource sfxSourcelooping;

        [Header("Sound Clips (Optional)")]
        [SerializeField] private List<AudioClip> soundClips; // Optional preloaded clips

        private Dictionary<string, AudioClip> soundLibrary = new Dictionary<string, AudioClip>();

        private void Awake()
        {
            if (sfxSource == null)
                sfxSource = gameObject.AddComponent<AudioSource>();

            // Build a quick-access library for named clips
            foreach (var clip in soundClips)
            {
                if (clip != null && !soundLibrary.ContainsKey(clip.name))
                    soundLibrary.Add(clip.name, clip);
            }
        }

        /// <summary>
        /// Plays a sound effect by clip name (must exist in soundClips list).
        /// </summary>
        private Coroutine sfxCoroutine;

        public void PlaySFX(string clipName, float volume = 1f,bool overlap = false)
        {
            if (!soundLibrary.TryGetValue(clipName, out AudioClip clip))
            {
                Debug.LogWarning($"[SoundManager] No sound found with name '{clipName}'");
                return;
            }

            // If the same clip is already playing, wait and then play again
            if (sfxSource.isPlaying && sfxSource.clip == clip && overlap)
            {
                if (sfxCoroutine != null)
                    StopCoroutine(sfxCoroutine);

                sfxCoroutine = StartCoroutine(WaitForClipAndReplay(clip, volume));
                return;
            }

            // Otherwise, play immediately
            sfxSource.clip = clip;
            sfxSource.volume = volume;
            sfxSource.Play();
        }

        public void PlaySFX(string clipName)
        {
            PlaySFX(clipName, sfxSource.volume,false);
        }

        private IEnumerator WaitForClipAndReplay(AudioClip clip, float volume)
        {
            // Wait for the currently playing clip to finish
            yield return new WaitWhile(() => sfxSource.isPlaying && sfxSource.clip == clip);

            sfxSource.clip = clip;
            sfxSource.volume = volume;
            sfxSource.Play();
        }


        /// <summary>
        /// Plays a sound effect from a given AudioClip.
        /// </summary>
        public void PlaySFX(AudioClip clip, float volume = 1f)
        {
            if (clip == null) return;
            sfxSource.PlayOneShot(clip, volume);
        }

        /// <summary>
        /// Sets the volume of all SFX.
        /// </summary>
        public void SetSFXVolume(float volume)
        {
            sfxSource.volume = Mathf.Clamp01(volume);
        }

        public void PlaySFXLooping(string clipName, float volume = 1f)
        {
            if (soundLibrary.TryGetValue(clipName, out AudioClip clip))
            {
                if (sfxSourcelooping.clip == clip && !sfxSourcelooping.isPlaying)
                {
                    sfxSourcelooping.UnPause();
                    return;
                }
                sfxSourcelooping.clip = clip;
                sfxSourcelooping.volume = volume;
                sfxSourcelooping.Play();

               
            }
            else
            {
                Debug.LogWarning($"[SoundManager] No sound found with name '{clipName}'");
            }
        }

        public void pauseSFXLooping()
        {
            sfxSourcelooping.Pause();
        }
        
        
    }
}
