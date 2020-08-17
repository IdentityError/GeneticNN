// Copyright (c) 2020 Matteo Beltrame

using System;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.TUtils.Audio
{
    public class AudioManager : MonoBehaviour
    {
        //SOUNDS NAMES

        [SerializeField] private Sound[] sounds = null;

        private static AudioManager instance = null;

        /// <summary>
        ///   Get the AudioManager singleton instance
        /// </summary>
        public static AudioManager GetInstance() { return instance; }

        private bool audioActive = true;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            DontDestroyOnLoad(gameObject);

            int length = sounds.Length;
            for (int i = 0; i < length; i++)
            {
                sounds[i].source = gameObject.AddComponent<AudioSource>();
                sounds[i].source.clip = sounds[i].clip;
                sounds[i].source.volume = sounds[i].volume;
                sounds[i].source.pitch = sounds[i].pitch;
                sounds[i].source.loop = sounds[i].loop;
            }
        }

        /// <summary>
        ///   Stop all the currently playing sounds
        /// </summary>
        private void StopAllSound()
        {
            int length = sounds.Length;
            for (int i = 0; i < length; i++)
            {
                if (sounds[i].source.isPlaying)
                {
                    sounds[i].source.Stop();
                }
            }
        }

        /// <summary>
        ///   <para> Smooth out a sound in a specified time with specified stride chunks </para>
        ///   Returns: the smoothed sound
        /// </summary>
        public void SmoothOutSound(Sound s, float stride, float duration)
        {
            StartCoroutine(SmoothOutSound_C(s, stride, duration));
        }

        private IEnumerator SmoothOutSound_C(Sound s, float stride, float duration)
        {
            if (s != null)
            {
                float currentVolume = s.volume;
                while (currentVolume - stride > 0f)
                {
                    currentVolume -= stride;
                    s.source.volume = currentVolume;
                    yield return new WaitForSecondsRealtime(duration * stride);
                }
                s.source.Stop();
                s.source.volume = s.volume;
            }
        }

        /// <summary>
        ///   <para> Smooth in a sound in a specified time with specified stride chunks </para>
        ///   Returns: the smoothed sound
        /// </summary>
        public Sound SmoothInSound(string name, float stride, float duration)
        {
            if (!audioActive)
            {
                return null;
            }

            Sound s = Array.Find(sounds, sound => sound.name == name);
            if (!s.source.isPlaying)
            {
                StartCoroutine(SmoothInSound_C(s, stride, duration));
            }
            return s;
        }

        private IEnumerator SmoothInSound_C(Sound s, float stride, float duration)
        {
            if (s != null)
            {
                float currentVolume = s.source.volume = 0f;
                s.source.Play();
                while (currentVolume + stride < s.volume)
                {
                    currentVolume += stride;
                    s.source.volume = currentVolume;
                    yield return new WaitForSecondsRealtime(duration * stride);
                }
                s.source.volume = s.volume;
            }
        }

        /// <summary>
        ///   <para> Play a sound </para>
        ///   Returns: the sound instance
        /// </summary>
        public Sound PlaySound(string name)
        {
            if (!audioActive)
            {
                return null;
            }

            Sound s = Array.Find(sounds, sound => sound.name == name);
            if (!s.source.isPlaying)
            {
                s.source.Play();
            }

            return s;
        }

        /// <summary>
        ///   Stop a specified sound
        /// </summary>
        public void StopSound(Sound s)
        {
            s.source.Stop();
        }

        /// <summary>
        ///   Stop a specified sound with the name
        /// </summary>
        public void StopSound(string name)
        {
            Sound s = Array.Find(sounds, sound => sound.name == name);
            if (!s.source.isPlaying)
            {
                s.source.Stop();
            }
        }

        /// <summary>
        ///   Notify the audio state to the manager (Enable disable sound)
        /// </summary>
        public void ToggleAudio(bool audioActive)
        {
            if (!audioActive)
            {
                StopAllSound();
            }

            this.audioActive = audioActive;
        }
    }
}