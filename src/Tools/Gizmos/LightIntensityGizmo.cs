using Il2CppInterop.Runtime.InteropTypes.Fields;
using Il2CppSLZ.Marrow;
using Il2CppTMPro;
using MelonLoader;

using UnityEngine;

namespace NEP.MonoDirector.Tools
{
    [RegisterTypeInIl2Cpp]
    public class LightIntensityGizmo(IntPtr ptr) : ToolGizmo(ptr)
    {
        public float Intensity { get => m_intensity; }

        private float m_intensity = 1f;

        private Vector3 m_lastUp;

        private TextMeshPro m_intensityText;

        protected override void Awake()
        {
            base.Awake();

            m_intensityText = transform.Find("Text").GetComponent<TextMeshPro>();
            m_body = transform.GetComponentInParent<Rigidbody>();
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

            m_intensity += angle * (1f / 360f);

            m_intensity = Mathf.Max(0f, m_intensity);

            m_intensityText.text = m_intensity.ToString("0.00");
        }

        public override void Hide()
        {
            base.Hide();

            m_intensityText.gameObject.SetActive(false);
        }

        public override void Show()
        {
            base.Show();

            m_intensityText.gameObject.SetActive(true);
        }
    }
}
