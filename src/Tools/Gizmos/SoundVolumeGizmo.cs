using Il2CppTMPro;
using UnityEngine;

using MelonLoader;

namespace NEP.MonoDirector.Tools
{
    [RegisterTypeInIl2Cpp]
    public class SoundVolumeGizmo(IntPtr ptr) : ToolGizmo(ptr)
    {
        public float Volume { get => m_volume; }

        private float m_volume = 1f;

        private Vector3 m_lastUp;

        private TextMeshPro m_volumeText;

        private GameObject m_line;

        protected override void Awake()
        {
            base.Awake();

            m_volumeText = transform.Find("Text").GetComponent<TextMeshPro>();
            m_line = transform.Find("Line").gameObject;
        }

        private void Update()
        {
            // Code from Camobiwon
            // Thanks for this fucking insane method that worked first try
            Transform dial = transform.parent;
            Vector3 up = dial.localRotation * Vector3.forward;
            Vector3 outVec = dial.localRotation * Vector3.up;

            float angle = Vector3.SignedAngle(m_lastUp, up, outVec);

            m_lastUp = up;

            m_volume += angle * (1f / 360f);

            m_volume = Mathf.Max(0f, Mathf.Min(1f, m_volume));

            m_volumeText.text = m_volume.ToString("0.00");
        }

        public override void Hide()
        {
            base.Hide();

            m_volumeText.gameObject.SetActive(false);
            m_line.SetActive(false);
        }

        public override void Show()
        {
            base.Show();

            m_volumeText.gameObject.SetActive(true);
            m_line.SetActive(true);
        }
    }
}
