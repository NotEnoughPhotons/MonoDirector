using Il2CppSLZ.Marrow;
using MelonLoader;
using NEP.MonoDirector.Audio;
using UnityEngine;
using static Il2CppSLZ.Marrow.LadderInfo;

namespace NEP.MonoDirector.Tools
{
    [RegisterTypeInIl2Cpp]
    public class SoundSource(IntPtr ptr) : PointToolEntity(ptr)
    {
        protected AudioSource m_source;

        protected override void Awake()
        {
            base.Awake();

            m_source = GetComponent<AudioSource>();
        }

        private void OnTriggerEnter(Collider other)
        {
            SoundHolder soundHolder = other.GetComponent<SoundHolder>();

            if (soundHolder == null)
            {
                return;
            }

            m_source.clip = soundHolder.GetSound();
            soundHolder.gameObject.SetActive(false);
            Main.feedbackSFX.LinkAudio();
        }

        protected override void OnStartPlayback() => m_source.Play();

        protected override void OnStopPlayback() => m_source.Stop();

        protected override void OnStartRecording() => m_source.Play();

        protected override  void OnStopRecording() => m_source.Stop();
    }
}