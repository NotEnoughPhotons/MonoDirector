using NEP.MonoDirector.State;
using UnityEngine;

using Il2CppSLZ.Marrow;
using Il2CppTMPro;

namespace NEP.MonoDirector.Tools
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    public class OmniLight(IntPtr ptr) : PointToolEntity(ptr)
    {
        public static List<OmniLight> ComponentCache { get; private set; }

        public float Range { get; private set; }
        public float Intensity { get; private set; }

        private Light m_light;
        private LightRadiusGizmo m_radiusGizmo;
        private LineRenderer m_lineRenderer;

        protected override void Awake()
        {
            base.Awake();
            ComponentCache = new List<OmniLight>();
            m_light = GetComponent<Light>();

            m_radiusGizmo = transform.Find("RadiusGizmo").GetComponent<LightRadiusGizmo>();
            m_lineRenderer = transform.Find("RadiusLine").GetComponent<LineRenderer>();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            ComponentCache.Add(this);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            ComponentCache.Remove(this);
        }

        protected virtual void Update()
        {
            m_lineRenderer.SetPosition(1, m_radiusGizmo.transform.localPosition);
            m_light.range = m_radiusGizmo.Distance;
        }

        protected override void OnHandAttached(Hand hand)
        {
            base.OnHandAttached(hand);
            m_radiusGizmo.Body.isKinematic = false;
            m_radiusGizmo.Joint.zMotion = ConfigurableJointMotion.Limited;
        }

        protected override void OnHandDetached(Hand hand)
        {
            if (GetAttachedHands() > 1)
            {
                return;
            }

            base.OnHandDetached(hand);
            m_radiusGizmo.Body.isKinematic = true;
            m_radiusGizmo.Joint.zMotion = ConfigurableJointMotion.Limited;
        }

        protected override void Hide()
        {
            base.Hide();
            m_radiusGizmo.Hide();
            m_lineRenderer.enabled = false;
        }

        protected override void Show()
        {
            base.Show();
            m_radiusGizmo.Show();
            m_lineRenderer.enabled = true;
        }
    }
}