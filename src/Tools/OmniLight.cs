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
        private Grip m_radiusControlGrip;
        private Rigidbody m_radiusControlBody;
        private ConfigurableJoint m_radiusControlJoint;
        private LineRenderer m_lineRenderer;

        private TextMeshPro m_distanceText;

        private Action<Hand> m_OnRadiusControlGrabbed;
        private Action<Hand> m_OnRadiusControlReleased;

        protected override void Awake()
        {
            base.Awake();
            ComponentCache = new List<OmniLight>();

            m_OnRadiusControlGrabbed = OnRadiusControlGrabbed;
            m_OnRadiusControlReleased = OnRadiusControlReleased;

            m_light = GetComponent<Light>();

            Transform radiusControl = transform.Find("RadiusControl");
            m_radiusControlGrip = radiusControl.GetComponent<Grip>();
            m_radiusControlBody = radiusControl.GetComponent<Rigidbody>();
            m_radiusControlJoint = radiusControl.GetComponent<ConfigurableJoint>();
            m_distanceText = radiusControl.Find("DistanceText").GetComponent<TextMeshPro>();

            m_lineRenderer = transform.Find("Line").GetComponent<LineRenderer>();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            ComponentCache.Add(this);

            m_radiusControlGrip.attachedHandDelegate += m_OnRadiusControlGrabbed;
            m_radiusControlGrip.detachedHandDelegate += m_OnRadiusControlReleased;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            ComponentCache.Remove(this);

            m_radiusControlGrip.attachedHandDelegate -= m_OnRadiusControlGrabbed;
            m_radiusControlGrip.detachedHandDelegate -= m_OnRadiusControlReleased;
        }

        protected virtual void Update()
        {
            float distance = Vector3.Distance(m_radiusControlBody.position, transform.position);

            m_lineRenderer.SetPosition(1, m_radiusControlBody.transform.localPosition);
            Range = distance * 2f;
            m_light.range = Range;
            m_distanceText.text = distance.ToString("0.00") + "m";
        }

        protected override void OnHandAttached(Hand hand)
        {
            base.OnHandAttached(hand);
            m_radiusControlBody.isKinematic = false;
            m_radiusControlJoint.zMotion = ConfigurableJointMotion.Limited;
        }

        protected override void OnHandDetached(Hand hand)
        {
            base.OnHandDetached(hand);
            m_radiusControlBody.isKinematic = true;
            m_radiusControlJoint.zMotion = ConfigurableJointMotion.Limited;
        }

        protected override void Hide()
        {
            base.Hide();
            m_lineRenderer.enabled = false;
            m_radiusControlBody.gameObject.SetActive(false);
        }

        protected override void Show()
        {
            base.Show();
            m_lineRenderer.enabled = true;
            m_radiusControlBody.gameObject.SetActive(true);
        }

        private void OnRadiusControlGrabbed(Hand hand)
        {
            m_radiusControlBody.isKinematic = false;
            m_radiusControlJoint.zMotion = ConfigurableJointMotion.Free;
        }

        private void OnRadiusControlReleased(Hand hand)
        {
            m_radiusControlJoint.connectedAnchor = m_radiusControlBody.transform.localPosition;
            m_radiusControlJoint.targetPosition = m_radiusControlBody.transform.localPosition;
            m_radiusControlBody.isKinematic = true;
            m_radiusControlJoint.zMotion = ConfigurableJointMotion.Limited;
        }
    }
}