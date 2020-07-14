/*  Base struct for playing sfx clips
 * 
 */

namespace Matt_Generics
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    [System.Serializable]
    public struct SfxWrapper
    {
        [Tooltip("The soundclip to associate with this sound file")]
        [SerializeField]
        private AudioClip soundClip;
        [Tooltip("The volume associated with this sound file. Range of [0f,1f]")]
        [Range(0.1f, 1f)]
        [SerializeField]
        private float soundVolume;

        // Creates a new SfxWrapper with the associated files
        public SfxWrapper(AudioClip currClip, float currVol)
        {
            soundClip = currClip;
            soundVolume = currVol;
        }

        // Plays the sound clip using a referenced Audio Source
        public void PlaySoundClip(AudioSource sfx)
        {
            sfx.PlayOneShot(soundClip, soundVolume);
        }
    }

}