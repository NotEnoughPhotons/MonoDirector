using Il2CppTMPro;
using MelonLoader;
using NEP.MonoDirector.Audio;
using UnityEngine;

namespace NEP.MonoDirector.Tools
{
    [RegisterTypeInIl2Cpp]
    public class SoundSource(IntPtr ptr) : PointToolEntity(ptr)
    {
        public AudioClip Clip { get => m_clip; }

        protected AudioSource m_source;
        protected AudioClip m_clip;

        protected TextMeshPro m_nameText;

        protected override void Awake()
        {
            base.Awake();

            m_source = GetComponent<AudioSource>();
            m_source.playOnAwake = false;
            m_source.dopplerLevel = 0f;

            m_nameText = transform.Find("SoundName").GetComponent<TextMeshPro>();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            m_nameText.text = "N/A";
        }

        protected override void Show()
        {
            base.Show();
            m_nameText.gameObject.SetActive(true);
        }

        protected override void Hide()
        {
            base.Hide();
            m_nameText.gameObject.SetActive(false);
        }

        private void OnTriggerEnter(Collider other)
        {
            SoundHolder soundHolder = other.GetComponent<SoundHolder>();

            if (soundHolder == null)
            {
                return;
            }

            m_source.clip = soundHolder.GetSound();
            m_clip = m_source.clip;
            m_nameText.text = m_clip.name;
            soundHolder.gameObject.SetActive(false);
            Main.feedbackSFX.LinkAudio();
        }

        protected override void OnStartPlayback() => m_source.Play();

        protected override void OnStopPlayback() => m_source.Stop();

        protected override void OnStartRecording() => m_source.Play();

        protected override void OnStopRecording() => m_source.Stop();

        public void Mute() => m_source.volume = 0f;
        public void Unmute() => m_source.volume = 1f;
    }
}