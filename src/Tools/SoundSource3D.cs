using Il2CppSLZ.Marrow;
using Il2CppSLZ.Marrow.Interaction;

using UnityEngine;

namespace NEP.MonoDirector.Tools
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    public class SoundSource3D(IntPtr ptr) : SoundSource(ptr)
    {
        private SoundSourceTether m_tether;
        private LineRenderer m_lineRenderer;

        protected override void Awake()
        {
            base.Awake();
            m_source.spatialBlend = 1f;

            Transform tether = transform.Find("TetherGizmo");
            m_tether = tether.GetComponent<SoundSourceTether>();

            m_lineRenderer = transform.Find("Line").GetComponent<LineRenderer>();
        }

        protected void Update()
        {
            float distance = Vector3.Distance(m_tether.transform.position, transform.position);

            m_lineRenderer.SetPosition(1, m_tether.transform.localPosition);
        }

        protected override void Hide()
        {
            base.Hide();
            m_lineRenderer.enabled = false;
            m_tether.Hide();
        }

        protected override void Show()
        {
            base.Show();
            m_lineRenderer.enabled = true;
            m_tether.Show();
        }
    }
}