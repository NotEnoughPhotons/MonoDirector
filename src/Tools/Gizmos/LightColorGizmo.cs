using Il2CppSLZ.Marrow;
using MelonLoader;

using UnityEngine;

namespace NEP.MonoDirector.Tools
{
    [RegisterTypeInIl2Cpp]
    public class LightColorGizmo(IntPtr ptr) : ToolGizmo(ptr)
    {
        public Color Color { get => m_color; }

        private Color m_color;
        private float m_sliderValue;
        private MeshRenderer m_meshRenderer;
        private Vector3 m_startPosition;
        private Vector3 m_maxPosition;

        private float m_sliderMax;
        private bool m_reset;

        protected override void Awake()
        {
            base.Awake();
            m_sliderValue = 0f;

            m_meshRenderer = GetComponentInChildren<MeshRenderer>();
            m_startPosition = transform.localPosition;
            m_maxPosition = new Vector3(-0.158f, 0f, 0f);

            float x1 = m_startPosition.x;
            float x2 = m_maxPosition.x;
            m_sliderMax = Mathf.Sqrt(Mathf.Pow(x1 - x2, 2));

            m_color = Color.white;
        }

        protected override void OnHandAttached(Hand hand)
        {
            m_body.isKinematic = false;
        }

        protected override void OnHandDetached(Hand hand)
        {
            m_body.isKinematic = true;
            m_reset = false;
        }

        protected override void OnHandAttachedUpdate(Hand hand)
        {
            base.OnHandAttachedUpdate(hand);

            if (!m_reset)
            {
                float x1 = transform.localPosition.x;
                float x2 = m_maxPosition.x;
                float target = Mathf.Sqrt(Mathf.Pow(x1 - x2, 2)) / m_sliderMax;

                m_sliderValue = target;
                m_sliderValue = Mathf.Max(0f, m_sliderValue);

                m_color = Color.HSVToRGB(m_sliderValue, 1f, 1f);
                m_meshRenderer.material.SetColor("_Emission", m_color);

                m_joint.targetPosition = transform.localPosition;
            }
        }

        protected override void OnBButtonDown()
        {
            m_color = Color.white;
            m_meshRenderer.material.SetColor("_Emission", m_color);
            m_joint.targetPosition = m_startPosition;
            m_reset = true;
        }
    }
}
