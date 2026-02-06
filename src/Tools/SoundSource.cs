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
        private SoundSourceTether m_tether;
        private LineRenderer m_lineRenderer;

        protected override void Awake()
        {
            base.Awake();

            m_source = GetComponent<AudioSource>();
            m_nameText = transform.Find("SoundName").GetComponent<TextMeshPro>();

            Transform tether = transform.Find("TetherGizmo");
            m_tether = tether.GetComponent<SoundSourceTether>();

            m_lineRenderer = transform.Find("Line").GetComponent<LineRenderer>();
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
            m_lineRenderer.enabled = true;
            m_tether.Show();
        }

        protected override void Hide()
        {
            base.Hide();
            m_nameText.gameObject.SetActive(false);
            m_lineRenderer.enabled = false;
            m_tether.Hide();
        }

        private void Update()
        {
            float distance = Vector3.Distance(m_tether.transform.position, transform.position);

            m_lineRenderer.SetPosition(1, m_tether.transform.localPosition);
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
            FeedbackSFX.LinkAudio();
        }

        protected override void OnStartPlayback() => m_source.Play();

        protected override void OnStopPlayback() => m_source.Stop();

        protected override void OnStartRecording() => m_source.Play();

        protected override void OnStopRecording() => m_source.Stop();

        public void Mute() => m_source.volume = 0f;
        public void Unmute() => m_source.volume = 1f;
    }
}