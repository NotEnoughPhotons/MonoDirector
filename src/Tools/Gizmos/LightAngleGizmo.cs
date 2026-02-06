using Il2CppSLZ.Marrow;
using Il2CppTMPro;
using MelonLoader;

using UnityEngine;

namespace NEP.MonoDirector.Tools
{
    [RegisterTypeInIl2Cpp]
    public class LightAngleGizmo(IntPtr ptr) : ToolGizmo(ptr)
    {
        public float Angle { get => m_angle; }

        private float m_angle;
        private TextMeshPro m_angleText;

        protected override void Awake()
        {
            base.Awake();

            m_angleText = transform.Find("AngleText").GetComponent<TextMeshPro>();
        }

        protected override void OnHandAttached(Hand hand)
        {
            m_body.isKinematic = false;
            m_joint.xMotion = ConfigurableJointMotion.Limited;
            m_joint.zMotion = ConfigurableJointMotion.Limited;
            m_joint.angularYMotion = ConfigurableJointMotion.Free;
        }

        protected override void OnHandDetached(Hand hand)
        {
            m_joint.connectedAnchor = transform.localPosition;
            m_joint.targetPosition = transform.localPosition;
            m_body.isKinematic = true;
            m_joint.xMotion = ConfigurableJointMotion.Limited;
            m_joint.zMotion = ConfigurableJointMotion.Limited;
            m_joint.angularYMotion = ConfigurableJointMotion.Limited;
        }

        private void Update()
        {
            m_angle = Vector3.Angle(transform.localPosition, Vector3.forward) * 2f;
            m_angleText.text = m_angle.ToString("0.00") + " deg";
        }
    }
}
