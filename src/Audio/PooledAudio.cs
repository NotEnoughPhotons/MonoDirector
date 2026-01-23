using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NEP.MonoDirector.Audio
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    public class PooledAudio(IntPtr ptr) : MonoBehaviour(ptr)
    {
        private AudioSource source;
        private float time;

        private void Awake()
        {
            source = gameObject.AddComponent<AudioSource>();
            source.spatialBlend = 1f;
            source.volume = 1f;
        }

        private void Update()
        {
            time += Time.deltaTime;

            if (time >= source.clip.length)
            {
                gameObject.SetActive(false);
                time = 0f;
            }
        }
    }
}
