using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MelonLoader.MelonLogger;

namespace NEP.MonoDirector.Audio
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    public class AudioManager(IntPtr ptr) : MonoBehaviour(ptr)
    {
        public static AudioManager Instance { get; private set; }

        private List<GameObject> _pooledObjects;

        private void Awake()
        {
            Instance = this;

            _pooledObjects = new List<GameObject>();

            GameObject list = new GameObject("Pooled Audio");
            list.transform.parent = transform;

            for(int i = 0; i < 256; i++)
            {
                GameObject pooledAudio = new GameObject("Poolee Audio");
                pooledAudio.transform.parent = list.transform;

                pooledAudio.AddComponent<PooledAudio>();
                pooledAudio.SetActive(false);
                _pooledObjects.Add(pooledAudio);
            }
        }

        public void Play(AudioClip clip)
        {
            GameObject inactiveSource = GetFirstInactive();
            AudioSource source = inactiveSource.GetComponent<AudioSource>();

            if(source != null)
            {
                source.clip = clip;
                source.spatialBlend = 1f;
                inactiveSource.SetActive(true);
            }
        }

        public void PlayAtPosition(AudioClip clip, Vector3 position)
        {
            GameObject inactiveSource = GetFirstInactive();
            AudioSource source = inactiveSource.GetComponent<AudioSource>();

            if (source != null)
            {
                source.clip = clip;
                source.spatialBlend = 1f;
                source.transform.position = position;
                inactiveSource.SetActive(true);
            }
        }

        private GameObject GetFirstInactive()
        {
            for(int i = 0; i < _pooledObjects.Count; i++)
            {
                if (!_pooledObjects[i].gameObject.activeInHierarchy)
                {
                    return _pooledObjects[i];
                }
            }

            return null;
        }
    }
}
