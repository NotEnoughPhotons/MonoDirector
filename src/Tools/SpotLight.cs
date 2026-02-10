using Il2CppSLZ.Marrow;
using UnityEngine;

namespace NEP.MonoDirector.Tools
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    public class SpotLight(IntPtr ptr) : PointToolEntity(ptr)
    {
        public static List<SpotLight> ComponentCache { get; private set; }

        private Light m_light;
        private LightRadiusGizmo m_radiusGizmo;
        private LightAngleGizmo m_angleGizmo;
        private LightIntensityGizmo m_intensityGizmo;
        private LightColorGizmo m_colorGizmo;
        private LineRenderer m_lineRenderer;
        private MeshRenderer m_spriteRenderer;
        private GameObject m_dial;
        private GameObject m_rainbowStrip;

        protected override void Awake()
        {
            base.Awake();
            ComponentCache = new List<SpotLight>();

            m_light = GetComponent<Light>();

            m_spriteRenderer = transform.Find("Frame/Sprite").GetComponent<MeshRenderer>();
            m_radiusGizmo = transform.Find("RadiusGizmo").GetComponent<LightRadiusGizmo>();
            m_angleGizmo = transform.Find("LeftAngleGizmo").GetComponent<LightAngleGizmo>();
            m_lineRenderer = transform.Find("RadiusLine").GetComponent<LineRenderer>();
            m_intensityGizmo = transform.Find("IntensityDial/Gizmo").GetComponent<LightIntensityGizmo>();
            m_colorGizmo = transform.Find("ColorGizmo").GetComponent<LightColorGizmo>();
            m_dial = transform.Find("IntensityDial/Quad").gameObject;
            m_rainbowStrip = transform.Find("ColorSlider/Plane").gameObject;
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
            m_light.spotAngle = m_angleGizmo.Angle;
            m_light.innerSpotAngle = m_angleGizmo.Angle;
            m_light.intensity = m_intensityGizmo.Intensity;
            m_light.color = m_colorGizmo.Color;
            m_spriteRenderer.material.SetColor("_BaseColor", m_light.color);
        }

        protected override void OnHandAttached(Hand hand)
        {
            base.OnHandAttached(hand);
            m_radiusGizmo.Body.isKinematic = false;
            // m_angleGizmo.Body.isKinematic = false;
            m_colorGizmo.Body.isKinematic = false;

            m_radiusGizmo.Joint.zMotion = ConfigurableJointMotion.Limited;
            // m_angleGizmo.Joint.angularYMotion = ConfigurableJointMotion.Limited;
            m_colorGizmo.Joint.xMotion = ConfigurableJointMotion.Limited;
        }

        protected override void OnHandDetached(Hand hand)
        {
            if (GetAttachedHands() > 1)
            {
                return;
            }

            base.OnHandDetached(hand);
            m_radiusGizmo.Body.isKinematic = true;
            // m_angleGizmo.Body.isKinematic = true;
            m_colorGizmo.Body.isKinematic = true;

            m_radiusGizmo.Joint.zMotion = ConfigurableJointMotion.Limited;
            // m_angleGizmo.Joint.angularYMotion = ConfigurableJointMotion.Limited;
            m_colorGizmo.Joint.xMotion = ConfigurableJointMotion.Limited;
        }

        protected override void Hide()
        {
            base.Hide();
            m_lineRenderer.enabled = false;
            m_radiusGizmo.Hide();
            m_angleGizmo.Hide();
            m_colorGizmo.Hide();
            m_intensityGizmo.Hide();
            m_dial.SetActive(false);
            m_rainbowStrip.SetActive(false);
        }

        protected override void Show()
        {
            base.Show();
            m_lineRenderer.enabled = true;
            m_radiusGizmo.Show();
            m_angleGizmo.Show();
            m_colorGizmo.Show();
            m_intensityGizmo.Show();
            m_dial.SetActive(true);
            m_rainbowStrip.SetActive(true);
        }
    }
}